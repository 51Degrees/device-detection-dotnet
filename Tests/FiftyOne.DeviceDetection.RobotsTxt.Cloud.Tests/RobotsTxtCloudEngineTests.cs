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

using FiftyOne.DeviceDetection.RobotsTxt.Cloud.FlowElements;
using FiftyOne.DeviceDetection.RobotsTxt.Data;
using FiftyOne.DeviceDetection.RobotsTxt.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.Data;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.RobotsTxt.Cloud.Tests
{
    [TestClass]
    public class RobotsTxtCloudEngineTests
    {
        private ILoggerFactory _lf;
        private const string _resource_key_env_variable = "RESOURCE_KEY_CLOUD_V5_BESPOKE";

        [TestInitialize]
        public void Init()
        {
            _lf = NullLoggerFactory.Instance;
        }

        /// <summary>
        /// Check that a cloud response with robots included is validly
        /// unpacked into <see cref="IRobotsTxtData"/>.
        /// This is an integration test that mocks the response from
        /// the cloud service.
        /// </summary>
        [TestMethod]
        public void ValidResponseUnpackedFromCloud()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            // Mock the properties endpoint
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.RequestUri.ToString().Contains("accessibleproperties")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("""
                    {
                        "Products": {
                            "robotstxt": {
                                "DataTier": "CloudV4TAC",
                                "Properties": [
                                    { "Name": "PlainText", "Type": "String", "Category": "RobotsTxt" },
                                    { "Name": "AnnotatedText", "Type": "String", "Category": "RobotsTxt" }
                                ]
                            }
                        }
                    }
                    """)
                });

            // Mock the data endpoint
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(
                        i => !i.RequestUri.ToString()
                            .Contains("accessibleproperties")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("""
                    {
                        "robotstxt": {
                            "plaintext": "# robots.txt copyright 51Degrees\nUser-agent: *\nAllow: /",
                            "annotatedtext": "# This Original Work is copyright of 51 Degrees\nUser-agent: *\nAllow: /"
                        }
                    }
                    """)
                });

            var httpClient = new HttpClient(handlerMock.Object);

            // Build the cloud engine with the mocked HttpClient
            var cloudEngine = new CloudRequestEngineBuilder(_lf, httpClient)
                .SetResourceKey("fake-resource-key")
                .Build();

            var robotsEngine = new RobotsTxtCloudEngineBuilder(_lf).Build();

            var pipeline = new PipelineBuilder(_lf)
                .AddFlowElement(cloudEngine)
                .AddFlowElement(robotsEngine)
                .Build();

            // Act
            var fd = pipeline.CreateFlowData();
            fd.AddEvidence("query.robotstxt.analytics", "allow");
            fd.Process();

            // Assert
            var data = fd.Get<IRobotsTxtData>();
            Assert.IsNotNull(data);
            Assert.IsTrue(data.PlainText.HasValue);
            Assert.IsTrue(data.AnnotatedText.HasValue);
            Assert.Contains("# robots.txt copyright 51Degrees", data.PlainText.Value);
            Assert.Contains("# This Original Work is copyright of 51 Degrees", data.AnnotatedText.Value);
        }
    }
}
