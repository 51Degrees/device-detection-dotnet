/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Core.Data;
using System.Text.Json;

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
        public void TestDetection()
        {
            InitWrapperAndUserAgents(PerformanceProfiles.MaxPerformance);
            using (var flowData = Wrapper.Pipeline.CreateFlowData())
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
        public void TestParallelSerialization(string fileName, string expectedJsonExample)
        {
            var dataFileInfo = TestHelpers.Utils.GetFilePath(fileName);
            var wrapper = new WrapperHash(dataFileInfo,
                PerformanceProfiles.MaxPerformance,
                TestHelpers.Constants.RequiredProperties
                );
            const int N = 20;
            var flowDatas = new List<IFlowData>();
            string[] results = new string[N];
            var deviceDatas = new List<IDeviceDataHash>();
            var deviceDatasNone = new List<IDeviceDataHash>();
            for (int i = 0; i < N; i++)
            {
                {
                    var flowData = wrapper.Pipeline.CreateFlowData();
                    flowData.AddEvidence(
                            "header.user-agent",
                            TestHelpers.Constants.MobileUserAgentiOS17);
                    flowData.Process();
                    var deviceData = flowData.Get<DeviceDataHash>();
                    Assert.IsNotNull(deviceData);
                    deviceDatas.Add(deviceData);
                    flowDatas.Add(flowData); //prevent disposing
                }
                {
                    var flowData = wrapper.Pipeline.CreateFlowData();
                    flowData.AddEvidence("nonevidence", "none");
                    flowData.Process();
                    var deviceData = flowData.Get<DeviceDataHash>();
                    Assert.IsNotNull(deviceData);
                    deviceDatasNone.Add(deviceData);
                }
            }

            // Parse the expected example JSON to get the property names
            JsonDocument expectedDoc = JsonDocument.Parse(expectedJsonExample);
            var expectedProperties = new HashSet<string>(
                expectedDoc.RootElement.EnumerateObject().Select(p => p.Name));

            Parallel.For(0, N, (int i) =>
            {
                string json = deviceDatas[i].GetAllValuesJson();

                // Validate JSON is well-formed
                JsonDocument jsonDoc = null;
                try
                {
                    jsonDoc = JsonDocument.Parse(json);
                }
                catch (JsonException ex)
                {
                    Assert.Fail($"Invalid JSON produced: {ex.Message}");
                }

                // Check that JSON has properties
                Assert.IsTrue(jsonDoc.RootElement.EnumerateObject().Any(),
                    "JSON should contain properties");

                // Validate all expected properties are present
                var actualProperties = new HashSet<string>(
                    jsonDoc.RootElement.EnumerateObject().Select(p => p.Name));

                foreach (var expectedProperty in expectedProperties)
                {
                    Assert.IsTrue(actualProperties.Contains(expectedProperty),
                        $"Expected property '{expectedProperty}' is missing from JSON");
                }

                // Validate all properties have non-null, non-empty values
                foreach (var property in jsonDoc.RootElement.EnumerateObject())
                {
                    Assert.IsFalse(string.IsNullOrEmpty(property.Name),
                        "Property name should not be null or empty");

                    // Check that property has a value (not undefined/null)
                    Assert.AreNotEqual(JsonValueKind.Null, property.Value.ValueKind,
                        $"Property '{property.Name}' should not be null");
                    Assert.AreNotEqual(JsonValueKind.Undefined, property.Value.ValueKind,
                        $"Property '{property.Name}' should not be undefined");

                    // Validate the value is not empty
                    if (property.Value.ValueKind == JsonValueKind.String)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(property.Value.GetString()),
                            $"Property '{property.Name}' should not have an empty string value");
                    }
                    else if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        Assert.IsTrue(property.Value.GetArrayLength() > 0,
                            $"Property '{property.Name}' should not have an empty array");
                    }
                }

                results[i] = json;

                // Validate empty evidence produces empty JSON object
                string noneJson = deviceDatasNone[i].GetAllValuesJson();
                Assert.AreEqual("{}", noneJson,
                    "Detection with no evidence should produce empty JSON object");
            });
        }

        [TestMethod]
        public void TestTransform()
        {
            var transform = new Transform();

            var result1 = transform.fromJsonGHEV("{\"architecture\":\"x86\",\"brands\":[{\"brand\":\"Chromium\",\"version\":\"128\"},{\"brand\":\"Not;A=Brand\",\"version\":\"24\"},{\"brand\":\"Google Chrome\",\"version\":\"128\"}],\"fullVersionList\":[{\"brand\":\"Chromium\",\"version\":\"128.0.6613.84\"},{\"brand\":\"Not;A=Brand\",\"version\":\"24.0.0.0\"},{\"brand\":\"Google Chrome\",\"version\":\"128.0.6613.84\"}],\"mobile\":false,\"model\":\"\",\"platform\":\"macOS\",\"platformVersion\":\"14.6.1\"}");
            Assert.AreEqual(result1["sec-ch-ua-arch"], "\"x86\"");
            Assert.AreEqual(result1["sec-ch-ua"], "\"Chromium\";v=\"128\", \"Not;A=Brand\";v=\"24\", \"Google Chrome\";v=\"128\"");
            Assert.AreEqual(result1["sec-ch-ua-full-version-list"], "\"Chromium\";v=\"128.0.6613.84\", \"Not;A=Brand\";v=\"24.0.0.0\", \"Google Chrome\";v=\"128.0.6613.84\"");
            Assert.AreEqual(result1["sec-ch-ua-platform"], "\"macOS\"");
            Assert.AreEqual(result1["sec-ch-ua-platform-version"], "\"14.6.1\"");
            Assert.AreEqual(result1["sec-ch-ua-mobile"], "?0");

            var result2 = transform.fromSUA("{\"browsers\":[{\"brand\":\"Chromium\",\"version\":[\"124\",\"0\",\"6367\",\"82\"]},{\"brand\":\"Google Chrome\",\"version\":[\"124\",\"0\",\"6367\",\"82\"]},{\"brand\":\"Not-A.Brand\",\"version\":[\"99\",\"0\",\"0\",\"0\"]}],\"platform\":{\"brand\":\"Android\",\"version\":[\"14\",\"0\",\"0\"]},\"mobile\":1,\"model\":\"SM-G998U\",\"source\":2}");
            Assert.AreEqual(result2["sec-ch-ua-model"], "\"SM-G998U\"");
            Assert.AreEqual(result2["sec-ch-ua-full-version-list"], "\"Chromium\";v=\"124.0.6367.82\", \"Google Chrome\";v=\"124.0.6367.82\", \"Not-A.Brand\";v=\"99.0.0.0\"");
            Assert.AreEqual(result2["sec-ch-ua-platform"], "\"Android\"");
            Assert.AreEqual(result2["sec-ch-ua-platform-version"], "\"14.0.0\"");
            Assert.AreEqual(result2["sec-ch-ua-mobile"], "?1");

            var result3 = transform.fromBase64GHEV("eyJhcmNoaXRlY3R1cmUiOiJ4ODYiLCJiaXRuZXNzIjoiNjQiLCJicmFuZHMiOlt7ImJyYW5kIjoiQ2hyb21pdW0iLCJ2ZXJzaW9uIjoiMTI4In0seyJicmFuZCI6Ik5vdDtBPUJyYW5kIiwidmVyc2lvbiI6IjI0In0seyJicmFuZCI6Ikdvb2dsZSBDaHJvbWUiLCJ2ZXJzaW9uIjoiMTI4In1dLCJmdWxsVmVyc2lvbkxpc3QiOlt7ImJyYW5kIjoiQ2hyb21pdW0iLCJ2ZXJzaW9uIjoiMTI4LjAuNjYxMy44NCJ9LHsiYnJhbmQiOiJOb3Q7QT1CcmFuZCIsInZlcnNpb24iOiIyNC4wLjAuMCJ9LHsiYnJhbmQiOiJHb29nbGUgQ2hyb21lIiwidmVyc2lvbiI6IjEyOC4wLjY2MTMuODQifV0sIm1vYmlsZSI6ZmFsc2UsIm1vZGVsIjoiTWFjQm9vayBQcm8iLCJwbGF0Zm9ybSI6Im1hY09TIn0=");
            Assert.AreEqual(result3["sec-ch-ua-arch"], "\"x86\"");
            Assert.AreEqual(result3["sec-ch-ua-full-version-list"], "\"Chromium\";v=\"128.0.6613.84\", \"Not;A=Brand\";v=\"24.0.0.0\", \"Google Chrome\";v=\"128.0.6613.84\"");
            Assert.AreEqual(result3["sec-ch-ua-platform"], "\"macOS\"");
            Assert.AreEqual(result3["sec-ch-ua-mobile"], "?0");
        }
    }
}
