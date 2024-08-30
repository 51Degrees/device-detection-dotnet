/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2023 51 Degrees Mobile Experts Limited, Davidson House,
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
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Hash.Tests.FlowElements
{
    [TestClass]
    [TestCategory("Core")]
    [TestCategory("Process")]
    public class EngineTests : TestsBase
    {

        [TestMethod]
        public void Engine_DataDownloadType()
        {
            InitWrapperAndUserAgents(PerformanceProfiles.MaxPerformance);
            Assert.AreEqual(
                "HashV41",
                Wrapper.Engine.GetDataDownloadType("None"));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Engine_DataSourceTier_Lite(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(
                profile,
                TestHelpers.Constants.LITE_HASH_DATA_FILE_NAME);
            Assert.AreEqual(
                "Lite",
                Wrapper.Engine.DataSourceTier);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Engine_DataSourceTier_TAC(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(
                profile,
                TestHelpers.Constants.TAC_HASH_DATA_FILE_NAME);
            Assert.AreEqual(
                "TAC",
                Wrapper.Engine.DataSourceTier);
        }

        /// <summary>
        /// Performs a basic device detection operation to verify that the 
        /// engine is usable.
        /// </summary>
        [TestMethod]
        private void TestDetection()
        {
            using(var flowData = Wrapper.Pipeline.CreateFlowData())
            {
                flowData.AddEvidence(
                    "user-agent", 
                    TestHelpers.Constants.MobileUserAgent);
                flowData.Process();
                var map = flowData.Get<IDeviceData>().AsDictionary();
                Assert.IsTrue(map.Count > 0);
                foreach(var item in map)
                {
                    Assert.IsNotNull(item.Key);
                    Assert.IsNotNull(item.Value);
                }
            }
        }

        [TestMethod]
        [DataRow(TestHelpers.Constants.TAC_HASH_DATA_FILE_NAME, TestHelpers.Constants.JsonOutputTAC)]
        [DataRow(TestHelpers.Constants.LITE_HASH_DATA_FILE_NAME, TestHelpers.Constants.JsonOutputLite)]
        public void TestParallelSerialization(string fileName, string expectedOutput)
        {
            var dataFileInfo = TestHelpers.Utils.GetFilePath(fileName);
            var wrapper = new WrapperHash(dataFileInfo, 
                PerformanceProfiles.MaxPerformance,
                TestHelpers.Constants.RequiredProperties
                );
            const int N = 20;
            string[] results = new string[N];

            Parallel.For(0, N, (int i) =>
            {
                Console.Out.WriteLine(i);
                using (var flowData = wrapper.Pipeline.CreateFlowData())
                {
                    flowData.AddEvidence(
                        "header.user-agent",
                        TestHelpers.Constants.MobileUserAgentiOS17);
                    flowData.Process();
                    var deviceData = flowData.Get<IDeviceDataHash>();
                    Assert.IsNotNull(deviceData);
                    string json = deviceData.GetAllValuesJson();
                    Assert.AreEqual(json, expectedOutput);
                    results[i] = json;
                }
                using (var flowData = wrapper.Pipeline.CreateFlowData())
                {
                    flowData.AddEvidence("nonevidence", "none");
                    flowData.Process();
                    var deviceData = flowData.Get<IDeviceDataHash>();
                    Assert.IsNotNull(deviceData);
                    string json = deviceData.GetAllValuesJson();
                    Assert.AreEqual(json, "{}");
                }
            });

            foreach (var result in results)
            {
                Assert.AreEqual(result, expectedOutput);
            }
        }
    }
}
