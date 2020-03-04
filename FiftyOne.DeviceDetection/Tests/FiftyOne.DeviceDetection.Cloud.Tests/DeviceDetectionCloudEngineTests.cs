/***********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL)
 * v.1.2 and is subject to its terms as set out below.
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

namespace FiftyOne.DeviceDetection.Cloud.Tests
{
    [TestClass]
    public class DeviceDetectionCloudEngineTests
    {
        private IPipeline _pipeline;

        [TestInitialize] 
        public void Init()
        {

        }

        [TestMethod]
        public void Test()
        {
            _pipeline = new DeviceDetectionPipelineBuilder(
                new LoggerFactory(), new System.Net.Http.HttpClient())
                .UseCloud("AQS5HKcyy086O2Kw10g")
                .Build();
            var data = _pipeline.CreateFlowData();
            data.AddEvidence("header.user-agent", "Mozilla/5.0 (Linux; Android 8.0.0; Pixel 2 XL Build/OPD1.170816.004) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.106 Mobile Safari/537.36");
            data.Process();

            var deviceData = data.Get<IDeviceData>();
            Assert.IsNotNull(deviceData);
            Assert.AreEqual("SmartPhone", deviceData.DeviceType.Value);
        }


        // TODO - The commented out section below is more how we should be testing. 
        // i.e. not relying on the cloud request engine actually making a request.
        // Unfortunatley, everything its too tightly coupled for this to work right now.

//        private DeviceDetectionCloudEngine _engine;
//        private TestLoggerFactory _loggerFactory;
//        private Mock<IPipeline> _pipeline;

//        private int _maxWarnings = 0;
//        private int _maxErrors = 0;
//        private string _testJson = "";

//        [TestInitialize]
//        public void Init()
//        {
//            _loggerFactory = new TestLoggerFactory();
//            _engine = new DeviceDetectionCloudEngine(
//                _loggerFactory.CreateLogger<DeviceDetectionCloudEngine>(),
//                CreateDeviceData);
//            _pipeline = new Mock<IPipeline>();
//        }

//        private DeviceDataCloud CreateDeviceData(IPipeline pipeline, FlowElementBase<DeviceDataCloud, IAspectPropertyMetaData> engine)
//        {
//            return new DeviceDataCloud(
//                _loggerFactory.CreateLogger<DeviceDataCloud>(),
//                pipeline,
//                (IAspectEngine)engine,
//                MissingPropertyService.Instance);
//        }
//        private CloudRequestData CreateTestData(IPipeline pipeline)
//        {
//            var data = new CloudRequestData(_loggerFactory.CreateLogger<CloudRequestData>(), pipeline, null);
//            data.JsonResponse = _testJson;
//            return data;
//        }

//        [TestCleanup]
//        public void Cleanup()
//        {
//            _loggerFactory.AssertMaxErrors(_maxErrors);
//            _loggerFactory.AssertMaxWarnings(_maxWarnings);
//        }

//        [TestMethod]
//        public void TestMethod1()
//        {
//            _testJson = @"{
//    ""device"": {
//    ""devicetype"": null,
//    ""hardwarename"": null,
//    ""hardwarevendor"": null,
//    ""javascripthardwareprofile"": null,
//    ""platformname"": null,
//    ""platformvendor"": null,
//    ""platformversion"": null,
//    ""browsername"": null,
//    ""browservendor"": null,
//    ""browserversion"": null
//  },
//  ""javascriptProperties"": [
//    ""device.javascripthardwareprofile""
//  ],
//  ""nullValueReasons"": {
//    ""device.devicetype"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.hardwarename"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.hardwarevendor"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.javascripthardwareprofile"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.platformname"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.platformvendor"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.platformversion"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.browsername"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.browservendor"": ""There were no values because the difference limit was exceeded so the results are invalid."",
//    ""device.browserversion"": ""There were no values because the difference limit was exceeded so the results are invalid.""
//  }
//}";
//            TestPipeline pipeline = new TestPipeline(_pipeline.Object);
//            TestFlowData testData = new TestFlowData(_loggerFactory.CreateLogger<TestFlowData>(), pipeline);
            
//            testData.GetOrAdd("cloudrequestdata", CreateTestData);
//            _engine.Process(testData);

//            var result = testData.Get<IDeviceData>();
//            Assert.IsNotNull(result);
//        }
    }
}
