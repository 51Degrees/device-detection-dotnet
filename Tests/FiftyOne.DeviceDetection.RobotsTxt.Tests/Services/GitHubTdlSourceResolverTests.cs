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
using FiftyOne.DeviceDetection.RobotsTxt.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.RobotsTxt.Tests.Services
{
    /// <summary>
    /// Tests for <see cref="GitHubTdlSourceResolver"/>. The resolver is
    /// the one piece that actually talks to GitHub, so the tests focus
    /// on what we promise externally: pick the latest file, do not
    /// hammer the API, and ride out transient failures by serving the
    /// last known good URL.
    /// </summary>
    [TestClass]
    public class GitHubTdlSourceResolverTests
    {
        private const string SourceId = "MOW-SOCW";
        private const string ApiBase = "https://api.github.com/";

        private static readonly TdlSource Source = new TdlSource
        {
            Id = SourceId,
            Repository = "movementforanopenweb/terms-documents",
            Directory = "socw",
            FilePattern = @"^(?<n>\d+)\.txt$",
            UrlTemplate = "https://m4ow.uk/socw/{name}",
        };

        /// <summary>
        /// IsKnown is a thin wrapper around the configured dictionary;
        /// matched ids report true regardless of casing.
        /// </summary>
        [TestMethod]
        public void IsKnown_ReturnsTrueForConfiguredSource()
        {
            using var resolver = CreateResolver(NeverCalledHandler());

            Assert.IsTrue(resolver.IsKnown(SourceId));
            Assert.IsTrue(resolver.IsKnown("mow-socw"));
        }

        /// <summary>
        /// Anything not in the configured map is unknown — including
        /// null and empty inputs.
        /// </summary>
        [TestMethod]
        public void IsKnown_ReturnsFalseForUnknownOrEmpty()
        {
            using var resolver = CreateResolver(NeverCalledHandler());

            Assert.IsFalse(resolver.IsKnown("UNKNOWN"));
            Assert.IsFalse(resolver.IsKnown(""));
            Assert.IsFalse(resolver.IsKnown(null));
        }

        /// <summary>
        /// Unknown ids must not generate any HTTP traffic.
        /// </summary>
        [TestMethod]
        public void Resolve_UnknownId_ReturnsNullWithoutHttp()
        {
            var handler = NeverCalledHandler();
            using var resolver = CreateResolver(handler);

            var result = resolver.Resolve("UNKNOWN");

            Assert.IsNull(result);
            VerifyNoHttp(handler);
        }

        /// <summary>
        /// Among files matching the pattern the largest version number
        /// wins, regardless of the order GitHub returned them in.
        /// </summary>
        [TestMethod]
        public void Resolve_PicksHighestNumberedFile()
        {
            var handler = HandlerReturning(
                File("1.txt"), File("4.txt"), File("2.txt"));
            using var resolver = CreateResolver(handler);

            var result = resolver.Resolve(SourceId);

            Assert.AreEqual("https://m4ow.uk/socw/4.txt", result.ToString());
        }

        /// <summary>
        /// Files that do not match the regex are ignored when picking
        /// the winner.
        /// </summary>
        [TestMethod]
        public void Resolve_IgnoresNonMatchingNames()
        {
            var handler = HandlerReturning(
                File("README.md"), File("1.txt"), File("draft.txt"));
            using var resolver = CreateResolver(handler);

            var result = resolver.Resolve(SourceId);

            Assert.AreEqual("https://m4ow.uk/socw/1.txt", result.ToString());
        }

        /// <summary>
        /// Directory entries are skipped even if their name happens to
        /// match the pattern — we only care about real files.
        /// </summary>
        [TestMethod]
        public void Resolve_IgnoresDirectories()
        {
            var handler = HandlerReturning(
                Dir("9.txt"), File("3.txt"));
            using var resolver = CreateResolver(handler);

            var result = resolver.Resolve(SourceId);

            Assert.AreEqual("https://m4ow.uk/socw/3.txt", result.ToString());
        }

        /// <summary>
        /// Empty/no-match listings yield null and do not poison the
        /// cache (a later success should still be picked up).
        /// </summary>
        [TestMethod]
        public void Resolve_NoMatchingFiles_ReturnsNull()
        {
            var handler = HandlerReturning(File("README.md"));
            using var resolver = CreateResolver(handler);

            var result = resolver.Resolve(SourceId);

            Assert.IsNull(result);
        }

        /// <summary>
        /// 4xx/5xx with nothing in the cache means we have nothing to
        /// hand back. Drop the value so the engine sees no TDL.
        /// </summary>
        [TestMethod]
        public void Resolve_GitHub403_NoCache_ReturnsNull()
        {
            var handler = HandlerWithStatus(HttpStatusCode.Forbidden);
            using var resolver = CreateResolver(handler);

            var result = resolver.Resolve(SourceId);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Network errors are treated like any other transient failure:
        /// no cache yet, so null.
        /// </summary>
        [TestMethod]
        public void Resolve_NetworkException_NoCache_ReturnsNull()
        {
            var handler = HandlerThrowing(new HttpRequestException("boom"));
            using var resolver = CreateResolver(handler);

            var result = resolver.Resolve(SourceId);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Once we have a successful result, a later GitHub failure
        /// must not break callers — we keep handing back the last
        /// known good URL.
        /// </summary>
        [TestMethod]
        public void Resolve_GitHub403AfterCachedSuccess_ReturnsCached()
        {
            var handler = SequencedHandler(
                JsonResponse(File("4.txt")),
                StatusResponse(HttpStatusCode.Forbidden));

            var clock = new FakeTimeProvider(DateTimeOffset.UtcNow);
            var ttl = TimeSpan.FromHours(1);
            using var resolver = CreateResolver(handler, clock, ttl);

            var first = resolver.Resolve(SourceId);
            clock.Advance(ttl + TimeSpan.FromMinutes(1));
            var second = resolver.Resolve(SourceId);

            Assert.AreEqual("https://m4ow.uk/socw/4.txt", first.ToString());
            Assert.AreEqual("https://m4ow.uk/socw/4.txt", second.ToString());
        }

        /// <summary>
        /// Within the TTL window we hold the value and never call
        /// GitHub again — that's the whole point of the cache.
        /// </summary>
        [TestMethod]
        public void Resolve_CacheHitWithinTtl_NoSecondHttpCall()
        {
            var handler = HandlerReturning(File("4.txt"));
            using var resolver = CreateResolver(handler);

            resolver.Resolve(SourceId);
            resolver.Resolve(SourceId);
            resolver.Resolve(SourceId);

            VerifyHttpCount(handler, 1);
        }

        /// <summary>
        /// After the TTL elapses we revalidate; a fresh response
        /// replaces the cached URL.
        /// </summary>
        [TestMethod]
        public void Resolve_CacheExpired_RefreshesAndReturnsNew()
        {
            var handler = SequencedHandler(
                JsonResponse(File("4.txt")),
                JsonResponse(File("5.txt")));

            var clock = new FakeTimeProvider(DateTimeOffset.UtcNow);
            var ttl = TimeSpan.FromHours(1);
            using var resolver = CreateResolver(handler, clock, ttl);

            var first = resolver.Resolve(SourceId);
            clock.Advance(ttl + TimeSpan.FromMinutes(1));
            var second = resolver.Resolve(SourceId);

            Assert.AreEqual("https://m4ow.uk/socw/4.txt", first.ToString());
            Assert.AreEqual("https://m4ow.uk/socw/5.txt", second.ToString());
            VerifyHttpCount(handler, 2);
        }

        /// <summary>
        /// Many parallel callers should result in exactly one trip
        /// to GitHub — the rest should ride along on the in-flight
        /// request and the populated cache.
        /// </summary>
        [TestMethod]
        public void Resolve_ConcurrentCalls_SingleHttpCall()
        {
            using var gate = new ManualResetEventSlim(initialState: false);
            var callCount = 0;

            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns<HttpRequestMessage, CancellationToken>((req, ct) =>
                {
                    Interlocked.Increment(ref callCount);
                    gate.Wait();
                    return JsonResponse(File("4.txt"));
                });

            using var resolver = CreateResolver(handler.Object);

            const int N = 50;
            var tasks = Enumerable.Range(0, N)
                .Select(_ => Task.Run(() => resolver.Resolve(SourceId)))
                .ToArray();

            // Give all callers a chance to queue at the semaphore.
            Thread.Sleep(50);

            gate.Set();
            Task.WaitAll(tasks);

            Assert.AreEqual(1, callCount);
            foreach (var t in tasks)
            {
                Assert.AreEqual("https://m4ow.uk/socw/4.txt", t.Result.ToString());
            }
        }

        private static GitHubTdlSourceResolver CreateResolver(
            HttpMessageHandler handler,
            FakeTimeProvider clock = null,
            TimeSpan? ttl = null)
        {
            var http = new HttpClient(handler) { BaseAddress = new Uri(ApiBase) };
            var sources = new Dictionary<string, TdlSource>(
                StringComparer.OrdinalIgnoreCase) { [SourceId] = Source };
            return new GitHubTdlSourceResolver(
                http,
                sources,
                NullLogger<GitHubTdlSourceResolver>.Instance,
                clock,
                ttl);
        }

        private static (string Name, string Type) File(string name) => (name, "file");
        private static (string Name, string Type) Dir(string name) => (name, "dir");

        private static HttpResponseMessage JsonResponse(
            params (string Name, string Type)[] entries)
        {
            var json = JsonSerializer.Serialize(
                entries.Select(e => new { name = e.Name, type = e.Type }));
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    json, Encoding.UTF8, "application/json"),
            };
        }

        private static HttpResponseMessage StatusResponse(HttpStatusCode status)
            => new HttpResponseMessage(status);

        private static HttpMessageHandler HandlerReturning(
            params (string Name, string Type)[] entries)
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns<HttpRequestMessage, CancellationToken>(
                    (req, ct) => JsonResponse(entries));
            return handler.Object;
        }

        private static HttpMessageHandler HandlerWithStatus(HttpStatusCode status)
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns<HttpRequestMessage, CancellationToken>(
                    (req, ct) => StatusResponse(status));
            return handler.Object;
        }

        private static HttpMessageHandler HandlerThrowing(Exception ex)
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<HttpResponseMessage>(
                    "Send",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Throws(ex);
            return handler.Object;
        }

        private static HttpMessageHandler SequencedHandler(
            params HttpResponseMessage[] responses)
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var setup = handler.Protected()
                .SetupSequence<HttpResponseMessage>(
                    "Send",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
            foreach (var r in responses)
            {
                setup = setup.Returns(r);
            }
            return handler.Object;
        }

        private static HttpMessageHandler NeverCalledHandler()
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            return handler.Object;
        }

        private static void VerifyNoHttp(HttpMessageHandler handler)
        {
            Mock.Get(handler).Protected()
                .Verify<HttpResponseMessage>(
                    "Send",
                    Times.Never(),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        private static void VerifyHttpCount(HttpMessageHandler handler, int expected)
        {
            Mock.Get(handler).Protected()
                .Verify<HttpResponseMessage>(
                    "Send",
                    Times.Exactly(expected),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Tiny manual <see cref="TimeProvider"/> stand-in. Avoids
        /// pulling in Microsoft.Extensions.TimeProvider.Testing.
        /// </summary>
        private sealed class FakeTimeProvider : TimeProvider
        {
            private DateTimeOffset _now;
            public FakeTimeProvider(DateTimeOffset start) { _now = start; }
            public override DateTimeOffset GetUtcNow() => _now;
            public void Advance(TimeSpan delta) { _now += delta; }
        }
    }
}
