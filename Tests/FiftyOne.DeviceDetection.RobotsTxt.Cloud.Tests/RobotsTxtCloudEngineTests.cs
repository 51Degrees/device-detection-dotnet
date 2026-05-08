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

using FiftyOne.DeviceDetection.RobotsTxt.Data;
using FiftyOne.DeviceDetection.RobotsTxt.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiftyOne.DeviceDetection.RobotsTxt.Cloud.Tests
{
    [TestClass]
    public class RobotsTxtCloudEngineTests
    {
        private ILoggerFactory _lf;

        [TestInitialize]
        public void Init()
        {
            _lf = NullLoggerFactory.Instance;
        }

        /// <summary>
        /// Check that a cloud response with robots included is validly 
        /// unpacked into <see cref="IRobotsTxtData"/>
        /// </summary>
        [TestMethod]
        public void ValidResponseUnpackedFromCloud()
        {
            var cloudEngine = new CloudRequestEngineBuilder(
                _lf,
                new System.Net.Http.HttpClient())
                .SetResourceKey("AQRVdgJ-qPrUPv-s3kg")
                .SetCloudRequestOrigin("test.com")
                .Build();
            var robotsEngine = new RobotsTxtCloudEngineBuilder(_lf).Build();
            var pipeline = new PipelineBuilder(_lf)
                .AddFlowElement(cloudEngine)
                .AddFlowElement(robotsEngine)
                .Build();

            var fd = pipeline.CreateFlowData();
            fd.AddEvidence("query.robotstxt.analytics", "allow");
            fd.Process();

            var data = fd.Get<IRobotsTxtData>();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.AnnotatedText.HasValue);
            Assert.IsTrue(data.PlainText.HasValue);

            Assert.Contains("# robots.txt copyright 51Degrees", data.PlainText.Value);
            Assert.Contains("# This Original Work is copyright of 51 Degrees", data.AnnotatedText.Value);
        }
    }
}
