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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FiftyOne.DeviceDetection.RobotsTxt.Tests
{
    /// <summary>
    /// Direct tests for <see cref="GeneratorService"/> output. The model is
    /// built by hand so these run without a data file.
    /// </summary>
    [TestClass]
    public class GeneratorServiceTests
    {
        private static GeneratorService BuildGenerator()
        {
            var model = new RobotsTxtModel
            {
                Usages = new[]
                {
                    new UsageModel { Name = "Search", Order = 0 },
                    new UsageModel { Name = "AI", Order = 1 },
                    new UsageModel { Name = "Monitoring", Order = 2 },
                },
                Crawlers = new[]
                {
                    new CrawlerModel
                    {
                        Name = "Googlebot",
                        Usages = new[] { "Search" },
                        ProductTokens = new[] { "Googlebot" },
                        ReferenceUris = new[]
                        {
                            new Uri("https://developers.google.com/search/docs/crawling-indexing/googlebot"),
                        },
                    },
                    new CrawlerModel
                    {
                        Name = "GPTBot",
                        Usages = new[] { "AI" },
                        ProductTokens = new[] { "GPTBot" },
                        ReferenceUris = new[] { new Uri("https://platform.openai.com/docs/gptbot") },
                    },
                    new CrawlerModel
                    {
                        Name = "BingBot",
                        Usages = new[] { "Search", "Monitoring" },
                        ProductTokens = new[] { "bingbot", "BingPreview" },
                        ReferenceUris = null,
                    },
                    new CrawlerModel
                    {
                        Name = "AhrefsBot",
                        Usages = new[] { "Monitoring" },
                        ProductTokens = new[] { "AhrefsBot" },
                        ReferenceUris = new[] { new Uri("https://ahrefs.com/robot") },
                    },
                },
            };
            return new GeneratorService(model);
        }

        private static string Generate(
            GeneratorService gen,
            HashSet<string> allowed,
            IReadOnlyList<Uri> tdls,
            bool annotations)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                gen.Write(writer, allowed, tdls, annotations, CancellationToken.None);
            }
            return sb.ToString().Replace("\r\n", "\n");
        }

        // The wildcard catch-all is the only block that uses "User-Agent: *".
        private static string WildcardBlock(string text)
        {
            var i = text.IndexOf("User-Agent: *", StringComparison.Ordinal);
            Assert.IsGreaterThanOrEqualTo(0, i, "Expected a wildcard 'User-Agent: *' block");
            return text.Substring(i);
        }

        // Non-comment "records": User-Agent / Disallow / Allow / TDL lines only.
        private static string Records(string text) =>
            string.Join(
                "\n",
                text.Split('\n')
                    .Select(line => line.Trim())
                    .Where(line => line.Length > 0 && line.StartsWith("#") == false));

        [TestMethod]
        public void AnnotatedText_AllowedCrawlers_LumpedInWildcardBlock()
        {
            var gen = BuildGenerator();

            var text = Generate(
                gen,
                new HashSet<string> { "Search" },
                Array.Empty<Uri>(),
                annotations: true);

            var wildcard = WildcardBlock(text);

            Assert.Contains("# N: BingBot", wildcard);
            Assert.Contains("# N: Googlebot", wildcard);
            Assert.DoesNotContain("Disallow:", wildcard);

            var bing = wildcard.IndexOf("# N: BingBot", StringComparison.Ordinal);
            var google = wildcard.IndexOf("# N: Googlebot", StringComparison.Ordinal);
            var allow = wildcard.IndexOf("Allow: /", StringComparison.Ordinal);
            Assert.IsLessThan(google, bing, "Allowed crawlers should be ordered by name");
            Assert.IsLessThan(allow, google, "Allowed-crawler comments must precede the Allow record");

            // Disallowed crawlers stay above the wildcard block (not mixed in).
            Assert.DoesNotContain("# N: AhrefsBot", wildcard);
            Assert.DoesNotContain("# N: GPTBot", wildcard);
        }

        [TestMethod]
        public void PlainText_AllowedCrawlers_NotAnnotated()
        {
            var gen = BuildGenerator();

            var text = Generate(
                gen,
                new HashSet<string> { "Search" },
                Array.Empty<Uri>(),
                annotations: false);

            Assert.DoesNotContain("# N:", text, "PlainText must not annotate crawlers");
            Assert.AreEqual(
                "User-Agent: *\nAllow: /\n",
                WildcardBlock(text),
                "PlainText wildcard block must stay a bare Allow");
        }

        [TestMethod]
        public void AnnotatedText_AllowedLump_PrecedesTdlPairs()
        {
            var gen = BuildGenerator();
            var tdls = new[]
            {
                new Uri("https://example.com/terms-v1"),
                new Uri("https://example.com/terms-v2"),
            };

            var text = Generate(gen, new HashSet<string> { "Search" }, tdls, annotations: true);
            var wildcard = WildcardBlock(text);

            Assert.Contains("# N: BingBot", wildcard);
            Assert.Contains("# N: Googlebot", wildcard);

            var google = wildcard.IndexOf("# N: Googlebot", StringComparison.Ordinal);
            var terms1 = wildcard.IndexOf("# Terms https://example.com/terms-v1", StringComparison.Ordinal);
            var tdl1 = wildcard.IndexOf("TDL: https://example.com/terms-v1", StringComparison.Ordinal);
            var terms2 = wildcard.IndexOf("# Terms https://example.com/terms-v2", StringComparison.Ordinal);
            var allow = wildcard.IndexOf("Allow: /", StringComparison.Ordinal);

            Assert.IsLessThan(terms1, google, "Allowed-crawler comments precede TDL lines");
            Assert.IsLessThan(tdl1, terms1, "Each # Terms comment precedes its TDL record (as in PlainText)");
            Assert.IsLessThan(terms2, tdl1, "TDL entries preserve input order");
            Assert.IsLessThan(allow, terms2, "TDL lines precede the Allow record");
        }

        [TestMethod]
        public void Records_IdenticalBetweenPlainAndAnnotated()
        {
            var gen = BuildGenerator();
            var allowed = new HashSet<string> { "Search" };
            var tdls = new[]
            {
                new Uri("https://example.com/terms-v1"),
                new Uri("https://example.com/terms-v2"),
            };

            var plain = Generate(gen, allowed, tdls, annotations: false);
            var annotated = Generate(gen, allowed, tdls, annotations: true);

            Assert.AreEqual(
                Records(plain),
                Records(annotated),
                "Non-comment records must match between PlainText and AnnotatedText");
        }

        [TestMethod]
        public void AnnotatedText_NoAllowedCrawlers_NoLump()
        {
            var gen = BuildGenerator();

            // Nothing allowed -> every crawler becomes a Disallow block; the
            // wildcard block has no allowed crawlers to list.
            var text = Generate(
                gen,
                new HashSet<string>(),
                Array.Empty<Uri>(),
                annotations: true);

            Assert.AreEqual(
                "User-Agent: *\nAllow: /\n",
                WildcardBlock(text),
                "With no allowed crawlers the wildcard block is a bare Allow");
        }
    }
}
