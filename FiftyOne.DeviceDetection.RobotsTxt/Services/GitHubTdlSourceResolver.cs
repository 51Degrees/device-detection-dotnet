/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2026 51 Degrees Mobile Experts Limited, Davidson House,
 * Forbury Square, Reading, Berkshire, United Kingdom RG1 3EU.
 *
 * This Original Work is licensed under the European Union Public Licence
 * (EUPL) v.1.2 and is subject to its terms as set out below.
 *
 * If a copy of the EUPL was not distributed with this file, You can obtain
 * one at https://opensource.org/licenses/EUPL-1.2.
 *
 * The 'Compatible Licences' set out in the Appendix to the EUPL (as may be
 * amended by the European Commission) shall be deemed incompatible for
 * the purposes of the Work and the provisions of the compatibility
 * clause in Article 5 of the EUPL shall not apply.
 *
 * If using the Work as, or as part of, a network application, by
 * including the attribution notice(s) required under Article 5 of the EUPL
 * in the end user terms of the application under an appropriate heading,
 * such notice(s) shall fulfill the requirements of that article.
 * ********************************************************************* */

using FiftyOne.DeviceDetection.RobotsTxt.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace FiftyOne.DeviceDetection.RobotsTxt.Services;

/// <summary>
/// Resolves TDL ids by listing the configured GitHub directory and
/// picking the file with the highest version number. Results live
/// in an in-memory cache; if a refresh fails we keep returning the
/// previous URL.
/// </summary>
public sealed class GitHubTdlSourceResolver : ITdlSourceResolver, IDisposable
{
    /// <summary>
    /// Default time a cache entry stays fresh before we try to
    /// refresh it. 12 hours covers a 1-2/day publication cadence
    /// while staying well under the 60/hour unauthenticated GitHub
    /// API limit.
    /// </summary>
    public static readonly TimeSpan DefaultCacheTtl = TimeSpan.FromHours(12);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _http;
    private readonly IReadOnlyDictionary<string, TdlSource> _sources;
    private readonly ILogger<GitHubTdlSourceResolver> _logger;
    private readonly TimeProvider _clock;
    private readonly TimeSpan _ttl;

    private readonly ConcurrentDictionary<string, CacheEntry> _cache =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates the resolver.
    /// </summary>
    /// <param name="http">
    /// HttpClient configured with a BaseAddress pointing at the
    /// GitHub API and a User-Agent header (GitHub rejects requests
    /// that lack one).
    /// </param>
    /// <param name="sources">
    /// Map of id → source descriptor. Loaders are typically
    /// host-specific (JSON, env, etc.) and live outside the engine.
    /// </param>
    /// <param name="logger">Logger.</param>
    /// <param name="clock">Time source; defaults to <see cref="TimeProvider.System"/>.</param>
    /// <param name="ttl">Cache TTL; defaults to <see cref="DefaultCacheTtl"/>.</param>
    public GitHubTdlSourceResolver(
        HttpClient http,
        IReadOnlyDictionary<string, TdlSource> sources,
        ILogger<GitHubTdlSourceResolver> logger,
        TimeProvider clock = null,
        TimeSpan? ttl = null)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _sources = sources ?? throw new ArgumentNullException(nameof(sources));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clock = clock ?? TimeProvider.System;
        _ttl = ttl ?? DefaultCacheTtl;
    }

    /// <inheritdoc/>
    public bool IsKnown(string id) =>
        !string.IsNullOrEmpty(id) && _sources.ContainsKey(id);

    /// <inheritdoc/>
    public Uri Resolve(string id)
    {
        if (string.IsNullOrEmpty(id)
            || !_sources.TryGetValue(id, out var source))
        {
            return null;
        }

        var now = _clock.GetUtcNow();
        if (_cache.TryGetValue(id, out var cached) && cached.FreshUntil > now)
        {
            return cached.Url;
        }

        // Single-flight: only one refresh per id at a time. Other
        // callers wait on the semaphore and pick up the cached value
        // populated by the first one.
        var sem = _semaphores.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
        sem.Wait();
        try
        {
            now = _clock.GetUtcNow();
            if (_cache.TryGetValue(id, out cached) && cached.FreshUntil > now)
            {
                return cached.Url;
            }

            try
            {
                var fresh = FetchLatest(source);
                if (fresh != null)
                {
                    _cache[id] = new CacheEntry(fresh, now + _ttl);
                    return fresh;
                }
                _logger.LogWarning(
                    "TDL source '{Id}' has no files matching its pattern.",
                    id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "TDL source '{Id}' refresh failed; falling back to " +
                    "cached value if any.", id);
            }

            return cached?.Url;
        }
        finally
        {
            sem.Release();
        }
    }

    private Uri FetchLatest(TdlSource source)
    {
        var path = $"repos/{source.Repository}/contents/{source.Directory}";
        using var req = new HttpRequestMessage(HttpMethod.Get, path);
        using var resp = _http.Send(req);
        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "GitHub returned {Status} listing '{Repo}/{Dir}'.",
                (int)resp.StatusCode, source.Repository, source.Directory);
            return null;
        }

        List<GitHubContent> entries;
        using (var stream = resp.Content.ReadAsStream())
        {
            entries = JsonSerializer.Deserialize<List<GitHubContent>>(
                stream, JsonOptions);
        }
        if (entries == null)
        {
            return null;
        }

        var regex = new Regex(source.FilePattern);
        string winnerName = null;
        long winnerN = -1;
        foreach (var entry in entries)
        {
            if (entry == null) continue;
            if (!"file".Equals(entry.Type, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            if (string.IsNullOrEmpty(entry.Name)) continue;

            var match = regex.Match(entry.Name);
            if (!match.Success) continue;
            var nGroup = match.Groups[TdlSource.VersionGroupName];
            if (!nGroup.Success) continue;
            if (!long.TryParse(nGroup.Value, out var n)) continue;

            if (n > winnerN)
            {
                winnerN = n;
                winnerName = entry.Name;
            }
        }

        if (winnerName == null)
        {
            return null;
        }

        var url = source.UrlTemplate.Replace(
            TdlSource.NamePlaceholder, winnerName);
        return Uri.TryCreate(url, UriKind.Absolute, out var u) ? u : null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        foreach (var sem in _semaphores.Values)
        {
            sem.Dispose();
        }
    }

    private sealed record CacheEntry(Uri Url, DateTimeOffset FreshUntil);

    private sealed class GitHubContent
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
