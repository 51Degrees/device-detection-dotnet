using FiftyOne.Common.TestHelpers;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Uach;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.Tests.Core
{
    [TestClass]
    public class UachJsConversionElementTests
    {
        private IPipeline _pipeline;

        [TestInitialize]
        public void Init()
        {
            var logFactory = LoggerFactory.Create(b => b.AddConsole());
            // Create the UA-CH JS conversion element
            var element = new UachJsConversionElementBuilder(logFactory).Build();
            // Create pipeline
            _pipeline = new PipelineBuilder(logFactory)
                .AddFlowElement(element)
                .Build();
        }

        [DataTestMethod]
        [DataRow(UACH_JSON_PIXEL5, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE, 
            DisplayName = "Not Base64 - Cookie")]
        [DataRow(UACH_JSON_PIXEL5, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_QUERY, 
            DisplayName = "Not Base64 - Query")]
        public void VerifyErrorHandling_NotB64Encoded(
            string uachJson,
            string evidenceKey)
        {
            using (var flowData = _pipeline.CreateFlowData())
            {
                // Supply evidence
                flowData.AddEvidence(evidenceKey, uachJson);
                var exceptionThrown = false;
                try
                {
                    flowData.Process();
                }
                catch (AggregateException)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown, "Expected exception to be thrown");
            }
        }

        [DataTestMethod]
        [DataRow(NOT_JSON, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE, 
            DisplayName = "Not JSON - Cookie")]
        [DataRow(NOT_JSON, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_QUERY, 
            DisplayName = "Not JSON - Query")]
        [DataRow(EMPTY_JSON, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE,
            DisplayName = "Empty JSON")]
        [DataRow(INVALID_JSON, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE,
            DisplayName = "Invalid JSON")]
        [DataRow(UNEXPECTED_JSON_STRUCTURE, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE,
            DisplayName = "Unexpected JSON structure")]        
        public void VerifyErrorHandling_UnexpectedEvidence(
            string uachJson,
            string evidenceKey)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(uachJson);
            string evidenceValue = Convert.ToBase64String(byteArray);

            using (var flowData = _pipeline.CreateFlowData())
            {
                // Supply evidence
                flowData.AddEvidence(evidenceKey, evidenceValue);
                var exceptionThrown = false;
                try
                {
                    flowData.Process();
                } 
                catch(AggregateException)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void VerifyErrorHandling_KeysAlreadyPresent()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(ONLY_PLATFORM);
            string evidenceValue = Convert.ToBase64String(byteArray);

            using (var flowData = _pipeline.CreateFlowData())
            {
                // Supply evidence
                flowData.AddEvidence(Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE, evidenceValue);
                // Add an existing evidence value for query.sec-ch-ua-platform.
                flowData.AddEvidence(Shared.Constants.EVIDENCE_SECCHUA_PLATFORM_QUERY_KEY,
                    @"""Existing Platform""");
                flowData.Process();

                // Make sure the pre-existing value has been overwritten.
                CheckEvidence(flowData, Shared.Constants.EVIDENCE_SECCHUA_PLATFORM_QUERY_KEY,
                    @"""Windows""");
            }
        }

        /// <summary>
        /// Test that the UA-CH JS conversion element is working correctly.
        /// </summary>
        [DataTestMethod]
        [DataRow(UACH_JSON_PIXEL5, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE,
            @"""Chromium"";v=""110"", ""Not A(Brand"";v=""24"", ""Google Chrome"";v=""110""",
            @"?1",
            @"""Pixel 5""",
            @"""Chromium"";v=""110.0.5481.178"", ""Not A(Brand"";v=""24.0.0.0"", ""Google Chrome"";v=""110.0.5481.178""",
            @"""Android""",
            @"""11""", DisplayName = "Pixel 5 - Cookie")]
        [DataRow(UACH_JSON_PIXEL5, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_QUERY,
            @"""Chromium"";v=""110"", ""Not A(Brand"";v=""24"", ""Google Chrome"";v=""110""",
            @"?1",
            @"""Pixel 5""",
            @"""Chromium"";v=""110.0.5481.178"", ""Not A(Brand"";v=""24.0.0.0"", ""Google Chrome"";v=""110.0.5481.178""",
            @"""Android""",
            @"""11""", DisplayName = "Pixel 5 - Query")]
        [DataRow(UACH_JSON_WIN10, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE,
            @"""Chromium"";v=""110"", ""Not A(Brand"";v=""24"", ""Google Chrome"";v=""110""",
            @"?0",
            @"""""",
            @"""Chromium"";v=""110.0.5481.178"", ""Not A(Brand"";v=""24.0.0.0"", ""Google Chrome"";v=""110.0.5481.178""",
            @"""Windows""",
            @"""10.0.0""", DisplayName = "Win 10")]
        [DataRow(ONLY_FULL_VERSION_LIST, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE,
            null,
            null,
            null,
            @"""Chromium"";v=""110.0.5481.178"", ""Not A(Brand"";v=""24.0.0.0"", ""Google Chrome"";v=""110.0.5481.178""",
            null,
            null, DisplayName = "Only full version list")]
        [DataRow(ONLY_PLATFORM, Constants.EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE,
            null,
            null,
            null,
            null,
            @"""Windows""",
            null, DisplayName = "Only platform")]
        public void VerifyRealWorldValues(
            string uachJson,
            string evidenceKey,
            string expectedSecChUa,
            string expectedSecChUaMobile,
            string expectedSecChUaModel,
            string expectedSecChUaFullVersionList,
            string expectedSecChUaPlatform,
            string expectedSecChUaPlatformVersion)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(uachJson);
            string encoded = Convert.ToBase64String(byteArray);

            using (var flowData = _pipeline.CreateFlowData())
            {
                // Supply evidence
                flowData.AddEvidence(evidenceKey, encoded);
                flowData.Process();

                // Check that the evidence has been updated as expected.
                CheckEvidence(flowData, Shared.Constants.EVIDENCE_SECCHUA_QUERY_KEY, 
                    expectedSecChUa);
                CheckEvidence(flowData, Shared.Constants.EVIDENCE_SECCHUA_FULLVERSIONLIST_QUERY_KEY, 
                    expectedSecChUaFullVersionList);
                CheckEvidence(flowData, Shared.Constants.EVIDENCE_SECCHUA_MOBILE_QUERY_KEY, 
                    expectedSecChUaMobile);
                CheckEvidence(flowData, Shared.Constants.EVIDENCE_SECCHUA_MODEL_QUERY_KEY, 
                    expectedSecChUaModel);
                CheckEvidence(flowData, Shared.Constants.EVIDENCE_SECCHUA_PLATFORM_QUERY_KEY,
                    expectedSecChUaPlatform);
                CheckEvidence(flowData, Shared.Constants.EVIDENCE_SECCHUA_PLATFORM_VERSION_QUERY_KEY, 
                    expectedSecChUaPlatformVersion);
            }
        }

        private void CheckEvidence(IFlowData flowData, string expectedKey, string expectedValue)
        {
            if (expectedValue != null)
            {
                Assert.IsTrue(flowData.TryGetEvidence(expectedKey, out string actualValue),
                    $"Missing expected evidence key '{expectedKey}'");
                Assert.AreEqual(expectedValue, actualValue,
                    $"Expected value miss-match for evidence key '{expectedKey}'");
            }
            else
            {
                // Expected value is null so we want to make sure there *isn't* any entry for
                // the expected key.
                Assert.IsFalse(flowData.TryGetEvidence(expectedKey, out string actualValue),
                    $"Evidence key '{expectedKey}' should not have been populated");
            }
        }

        // Constants containing evidence values that are used in the tests above.
        private const string UACH_JSON_PIXEL5 = @"{
""brands"":[
    {""brand"":""Chromium"",""version"":""110""},
    {""brand"":""Not A(Brand"",""version"":""24""},
    {""brand"":""Google Chrome"",""version"":""110""}],
""fullVersionList"":[
    {""brand"":""Chromium"",""version"":""110.0.5481.178""},
    {""brand"":""Not A(Brand"",""version"":""24.0.0.0""},
    {""brand"":""Google Chrome"",""version"":""110.0.5481.178""}],
""mobile"":true,
""model"":""Pixel 5"",
""platform"":""Android"",
""platformVersion"":""11""}";

        private const string UACH_JSON_WIN10 = @"{
""brands"":[
    {""brand"":""Chromium"",""version"":""110""},
    {""brand"":""Not A(Brand"",""version"":""24""},
    {""brand"":""Google Chrome"",""version"":""110""}],
""fullVersionList"":[
    {""brand"":""Chromium"",""version"":""110.0.5481.178""},
    {""brand"":""Not A(Brand"",""version"":""24.0.0.0""},
    {""brand"":""Google Chrome"",""version"":""110.0.5481.178""}],
""mobile"":false,
""model"":"""",
""platform"":""Windows"",
""platformVersion"":""10.0.0""}";

        private const string ONLY_FULL_VERSION_LIST = @"{
""fullVersionList"":[
    {""brand"":""Chromium"",""version"":""110.0.5481.178""},
    {""brand"":""Not A(Brand"",""version"":""24.0.0.0""},
    {""brand"":""Google Chrome"",""version"":""110.0.5481.178""}]}";

        private const string ONLY_PLATFORM = @"{
""platform"":""Windows""}";

        private const string NOT_JSON = @"not json";
        private const string EMPTY_JSON = @"{}";
        private const string INVALID_JSON = @"{ a""b: 123}";

        private const string UNEXPECTED_JSON_STRUCTURE = @"{ values: {
""platform"":""Windows""}}";
    }
}
