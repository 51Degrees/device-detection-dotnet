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
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace FiftyOne.DeviceDetection.RobotsTxt.Tests.Services
{
    /// <summary>
    /// Live network smoke test that calls the real GitHub API.
    /// Tagged so CI can skip it via <c>--filter TestCategory!=LiveNetwork</c>.
    /// </summary>
    [TestClass]
    public class GitHubTdlSourceResolverLiveTests
    {
        /// <summary>
        /// Hits the real api.github.com and checks the resolved URL
        /// matches the friendly m4ow.uk pattern. Burns one request
        /// from the unauthenticated 60/hour pool.
        /// </summary>
        [TestMethod]
        [TestCategory("LiveNetwork")]
        public void Resolve_AgainstRealGitHub_ReturnsMowSocwUrl()
        {
            using var http = new HttpClient
            {
                BaseAddress = new Uri("https://api.github.com/"),
                Timeout = TimeSpan.FromSeconds(10),
            };
            http.DefaultRequestHeaders.UserAgent.ParseAdd("51Degrees-CloudService");
            http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
            http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

            var sources = new Dictionary<string, TdlSource>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["MOW-SOCW"] = new TdlSource
                {
                    Id = "MOW-SOCW",
                    Repository = "movementforanopenweb/terms-documents",
                    Directory = "socw",
                    FilePattern = @"^(?<n>\d+)\.txt$",
                    UrlTemplate = "https://m4ow.uk/socw/{name}",
                },
            };

            using var resolver = new GitHubTdlSourceResolver(
                http, sources, NullLogger<GitHubTdlSourceResolver>.Instance);

            var url = resolver.Resolve("MOW-SOCW");

            Assert.IsNotNull(url,
                "Resolver returned null — GitHub may be down or rate-limited.");
            var pattern = new Regex(@"^https://m4ow\.uk/socw/\d+\.txt$");
            Assert.IsTrue(pattern.IsMatch(url.ToString()),
                $"URL '{url}' did not match expected friendly form.");
            TestContext.WriteLine($"Resolved live URL: {url}");
        }

        /// <summary>
        /// Set by MSTest; used to write the resolved URL to test output.
        /// </summary>
        public TestContext TestContext { get; set; }
    }
}
