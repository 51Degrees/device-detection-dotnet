/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.Common.TestHelpers;
using FiftyOne.DeviceDetection.Cloud.Data;
using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines.TestHelpers;
using FiftyOne.Pipeline.CloudRequestEngine.Data;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using System;

namespace FiftyOne.DeviceDetection.Cloud.Tests
{
    [TestClass]
    public class DeviceDetectionCloudEngineTests
    {
        private IPipeline _pipeline;
        private const string _resource_key_env_variable = "SUPER_RESOURCE_KEY";

        [TestInitialize] 
        public void Init()
        {

        }
        
        /// <summary>
        /// Perform a simple gross error check by calling the cloud service
        /// with a single User-Agent and validating the device type 
        /// property is correct.
        /// This is an integration test that uses the live cloud service
        /// so any problems with that service could affect the result
        /// of this test.
        /// </summary>
        [TestMethod]
        public void CloudIntegrationTest()
        {
            var resourceKey = System.Environment.GetEnvironmentVariable(
                _resource_key_env_variable);

            if (resourceKey != null)
            {
                _pipeline = new DeviceDetectionPipelineBuilder(
                    new LoggerFactory(), new System.Net.Http.HttpClient())
                    .UseCloud(resourceKey)
                    .Build();
                using (var data = _pipeline.CreateFlowData())
                {
                    data.AddEvidence("header.user-agent",
                        "Mozilla/5.0 (Linux; Android 8.0.0; Pixel 2 XL " +
                        "Build/OPD1.170816.004) AppleWebKit/537.36 " +
                        "(KHTML, like Gecko) Chrome/80.0.3987.106 " +
                        "Mobile Safari/537.36");
                    data.Process();

                    var deviceData = data.Get<IDeviceData>();
                    Assert.IsNotNull(deviceData);
                    Assert.AreEqual("SmartPhone", deviceData.DeviceType.Value);
                }
            }
            else
            {
                Assert.Inconclusive($"No resource key supplied in " +
                    $"environment variable '{_resource_key_env_variable}'");
            }
        }


        /// <summary>
        /// Verify that making requests using a resource key that
        /// is limited to particular origins will fail or succeed
        /// in the expected scenarios. 
        /// This is an integration test that uses the live cloud service
        /// so any problems with that service could affect the result
        /// of this test.
        /// </summary>
        [DataTestMethod]
        [DataRow(null, true)]
        [DataRow("test.com", true)]
        [DataRow("51degrees.com", false)]
        public void ResourceKeyWithOrigin(
            string origin, 
            bool expectException)
        {
            bool exception = false;

            try
            {
                var resourceKey = "AQS5HKcyVj6B8wNG2Ug";

                _pipeline = new DeviceDetectionPipelineBuilder(
                    new LoggerFactory(), new System.Net.Http.HttpClient())
                    .UseCloud(resourceKey)
                    .SetCloudRequestOrigin(origin)
                    .Build();
                using (var data = _pipeline.CreateFlowData())
                {
                    data.AddEvidence("header.user-agent",
                        "Mozilla/5.0 (Linux; Android 8.0.0; Pixel 2 XL " +
                        "Build/OPD1.170816.004) AppleWebKit/537.36 " +
                        "(KHTML, like Gecko) Chrome/80.0.3987.106 " +
                        "Mobile Safari/537.36");
                    data.Process();
                }
            }
            catch(Exception ex)
            {
                exception = true;

                Assert.IsTrue(ex.Message.Contains(
                    $"This Resource Key is not authorized " +
                    $"for use with this domain: '{origin ?? ""}'."), 
                    $"Exception did not contain expected text " +
                    $"({ex.Message})");
            }

            Assert.AreEqual(expectException, exception);
        }
    }
}
