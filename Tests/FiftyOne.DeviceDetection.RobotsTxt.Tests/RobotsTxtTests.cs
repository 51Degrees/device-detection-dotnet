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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.RobotsTxt.Data;
using FiftyOne.DeviceDetection.RobotsTxt.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]

namespace FiftyOne.DeviceDetection.RobotsTxt.Tests
{
    /// <summary>
    /// Tests for <see cref="RobotsEngine"/> with RobotsTxt configuration.
    /// </summary>
    [TestClass]
    public class RobotsTxtTests
    {
        private static ILoggerFactory _loggerFactory;
        private static RobotsTxtEngine _engine;
        private static IPipeline _pipeline;
        private IFlowData _data;

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _pipeline?.Dispose();
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var ddFile = Helper.GetDeviceDetectionFiles().FirstOrDefault();
            if (ddFile == null)
            {
                Assert.Inconclusive(
                    "No .hash data file found in device-detection-data.");
                return;
            }

            _loggerFactory = LoggerFactory.Create(b => { });

            // Build DeviceDetectionHashEngine first
            var hashEngine = new DeviceDetectionHashEngineBuilder(_loggerFactory)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .Build(ddFile, false);

            // Build PropertyKeyedDeviceEngine configured for RobotsTxt
            _engine = new RobotsTxtEngineBuilder(_loggerFactory).Build();

            // Create a pipeline containing both engines.
            _pipeline = new PipelineBuilder(_loggerFactory)
                .AddFlowElement(hashEngine)
                .AddFlowElement(_engine)
                .SetSuppressProcessExceptions(true)
                .SetAutoDisposeElements(true)
                .Build();

            // AddPipeline must be called for RobotsTxt explicitly so that the
            // robots.txt engine can get the data it needs from the HashEngine.
            _engine.AddPipeline(_pipeline);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _data = _pipeline.CreateFlowData();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _data?.Dispose();
        }

        [TestMethod]
        [DataRow("query.robotstxt.search", "true")]
        [DataRow("query.robotstxt.search", "on")]
        [DataRow("query.robotstxt.search", "allow")]
        [DataRow("query.robotstxt.search", "yes")]
        [DataRow("query.robotstxt.search", "enabled")]
        public void AllowTest(string key, string value)
        {
            // Arrange
            _data.AddEvidence(key, value);

            // Act
            _data.Process();

            // Assert
            var result = _data.Get<IRobotsTxtData>();
            Assert.IsNotNull(result.AnnotatedText);
            Assert.IsNotNull(result.PlainText);
            Assert.IsTrue(result.AnnotatedText.HasValue);
            Assert.IsTrue(result.PlainText.HasValue);
            Assert.Contains("Allow: /", 
                result.PlainText.Value, 
                "Expect some crawlers to be allowed for search");
            Assert.MatchesRegex("# U: Search", 
                result.AnnotatedText.Value, 
                "Expect annotations to include search");
        }

        /// <summary>
        /// Checks that when disallow is set for at least one usage that the
        /// robots.txt is returned without any allowed usages.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [TestMethod]
        [DataRow("query.robotstxt.search", "false")]
        [DataRow("query.robotstxt.search", "off")]
        [DataRow("query.robotstxt.search", "disallow")]
        [DataRow("query.robotstxt.search", "no")]
        [DataRow("query.robotstxt.search", "disabled")]
        public void DisallowTest(string key, string value)
        {
            // Arrange
            _data.AddEvidence(key, value);

            // Act
            _data.Process();

            // Assert
            var result = _data.Get<IRobotsTxtData>();
            Assert.IsNotNull(result.AnnotatedText);
            Assert.IsNotNull(result.PlainText);
            Assert.IsTrue(result.AnnotatedText.HasValue);
            Assert.IsTrue(result.PlainText.HasValue);
            Assert.Contains(
                "Disallow: /",
                result.PlainText.Value,
                "Expect no crawlers to be allowed");
            Assert.DoesNotContain(
                "Allow: /",
                result.PlainText.Value);
            Assert.MatchesRegex(
                "# U: Search",
                result.AnnotatedText.Value,
                "Expect annotations to include search");
        }

        /// <summary>
        /// Checks that if no evidence is provided the engine doesn't add the
        /// aspect data.
        /// </summary>
        [TestMethod]
        public void Empty()
        {
            // Act
            _data.Process();

            // Assert
            Assert.ThrowsExactly<PipelineDataException>(
                () => _data.Get<IRobotsTxtData>());
        }
    }
}
