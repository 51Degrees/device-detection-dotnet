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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FiftyOne.DeviceDetection.RobotsTxt.Services;

/// <summary>
/// Reads tdlSources.json into a lookup keyed by source id. Three
/// entry points: <see cref="LoadDefault"/> for the file embedded in
/// the engine assembly, <see cref="LoadFromFile"/> for a host-supplied
/// file path, and <see cref="LoadFromJson"/> for an in-memory string.
/// Validation happens up front so any config mistake fails fast
/// rather than slipping into runtime.
/// </summary>
public static class TdlSourcesLoader
{
    private const string DefaultResourceSuffix = "tdlSources.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Loads the default sources shipped with the engine
    /// (embedded JSON resource). Lookups against the returned
    /// dictionary are case-insensitive.
    /// </summary>
    public static IReadOnlyDictionary<string, TdlSource> LoadDefault()
    {
        var asm = typeof(TdlSourcesLoader).Assembly;
        var resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(
                DefaultResourceSuffix, StringComparison.Ordinal))
            ?? throw new InvalidOperationException(
                $"Embedded resource '{DefaultResourceSuffix}' not " +
                $"found in {asm.FullName}.");

        using var stream = asm.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Failed to open embedded resource '{resourceName}'.");
        using var reader = new StreamReader(stream);
        return Parse(reader.ReadToEnd(), $"embedded:{resourceName}");
    }

    /// <summary>
    /// Loads sources from a JSON file at the given path. Use this to
    /// replace the default list with your own.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty.</exception>
    /// <exception cref="FileNotFoundException">File does not exist.</exception>
    /// <exception cref="InvalidOperationException">JSON is malformed or any entry is invalid.</exception>
    public static IReadOnlyDictionary<string, TdlSource> LoadFromFile(
        string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "File path must be supplied.", nameof(filePath));
        }
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"TDL sources file not found at '{filePath}'.", filePath);
        }
        return Parse(File.ReadAllText(filePath), filePath);
    }

    /// <summary>
    /// Loads sources from an in-memory JSON string. Handy for tests
    /// and for fully in-process configuration where no file system
    /// is involved.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is null.</exception>
    /// <exception cref="InvalidOperationException">JSON is malformed or any entry is invalid.</exception>
    public static IReadOnlyDictionary<string, TdlSource> LoadFromJson(
        string json)
    {
        if (json == null)
        {
            throw new ArgumentNullException(nameof(json));
        }
        return Parse(json, "<inline>");
    }

    private static IReadOnlyDictionary<string, TdlSource> Parse(
        string json, string source)
    {
        List<TdlSource> entries;
        try
        {
            entries = JsonSerializer.Deserialize<List<TdlSource>>(
                json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"TDL sources at '{source}' is not valid JSON.", ex);
        }

        if (entries == null)
        {
            throw new InvalidOperationException(
                $"TDL sources at '{source}' must contain a JSON array.");
        }

        var result = new Dictionary<string, TdlSource>(
            StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            Validate(entry, i, source);
            if (result.ContainsKey(entry.Id))
            {
                throw new InvalidOperationException(
                    $"Duplicate TDL source id '{entry.Id}' in '{source}'.");
            }
            result.Add(entry.Id, entry);
        }
        return result;
    }

    private static void Validate(
        TdlSource entry, int index, string source)
    {
        if (entry == null)
        {
            throw new InvalidOperationException(
                $"TDL source at index {index} in '{source}' is null.");
        }

        RequireNonEmpty(entry.Id, nameof(entry.Id), index, source);
        RequireNonEmpty(entry.Repository, nameof(entry.Repository), index, source);
        RequireNonEmpty(entry.Directory, nameof(entry.Directory), index, source);
        RequireNonEmpty(entry.FilePattern, nameof(entry.FilePattern), index, source);
        RequireNonEmpty(entry.UrlTemplate, nameof(entry.UrlTemplate), index, source);

        Regex regex;
        try
        {
            regex = new Regex(entry.FilePattern);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException(
                $"TDL source '{entry.Id}' in '{source}' has an " +
                $"invalid FilePattern.", ex);
        }

        if (!regex.GetGroupNames().Contains(TdlSource.VersionGroupName))
        {
            throw new InvalidOperationException(
                $"TDL source '{entry.Id}' in '{source}' FilePattern " +
                $"must declare a named group " +
                $"'(?<{TdlSource.VersionGroupName}>...)'.");
        }

        if (!entry.UrlTemplate.Contains(TdlSource.NamePlaceholder))
        {
            throw new InvalidOperationException(
                $"TDL source '{entry.Id}' in '{source}' UrlTemplate " +
                $"must include the '{TdlSource.NamePlaceholder}' " +
                $"placeholder.");
        }
    }

    private static void RequireNonEmpty(
        string value, string field, int index, string source)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"TDL source at index {index} in '{source}' is " +
                $"missing '{field}'.");
        }
    }
}
