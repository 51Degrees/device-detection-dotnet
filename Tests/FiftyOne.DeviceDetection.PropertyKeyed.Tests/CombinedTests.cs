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
using FiftyOne.DeviceDetection.PropertyKeyed.Data;
using FiftyOne.DeviceDetection.PropertyKeyed.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.DeviceDetection.TestHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SharedConstants = FiftyOne.DeviceDetection.Shared.Constants;
using TestConstants = FiftyOne.DeviceDetection.TestHelpers.Constants;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    /// <summary>
    /// Verifies that TAC and NativeModel lookups can share the same
    /// "hardware" element data key without losing profiles when both forms
    /// of evidence are present in the same request.
    /// </summary>
    [TestClass]
    public class CombinedTests
    {
        private const string ValidTac = "35925406";
        private const string ValidNativeModel = "iPhone11,8";

        private static ILoggerFactory _loggerFactory;
        private static TacEngine _tacEngine;
        private static NativeEngine _nativeEngine;
        private static IPipeline _pipeline;
        private IFlowData _data;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var ddFile = Utils.GetFilePath(
                TestConstants.TAC_HASH_DATA_FILE_NAME).FullName;

            _loggerFactory = LoggerFactory.Create(b => { });

            var hashEngine = new DeviceDetectionHashEngineBuilder(
                _loggerFactory)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .Build(ddFile, false);

            _tacEngine = new TacEngineBuilder(_loggerFactory).Build();
            _nativeEngine = new NativeEngineBuilder(_loggerFactory).Build();

            _pipeline = new PipelineBuilder(_loggerFactory)
                .AddFlowElement(hashEngine)
                .AddFlowElement(_tacEngine)
                .AddFlowElement(_nativeEngine)
                .SetSuppressProcessExceptions(true)
                .SetAutoDisposeElements(true)
                .Build();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _pipeline?.Dispose();
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
        public void BothEngines_UseSameElementDataKey()
        {
            Assert.AreEqual("hardware", _tacEngine.ElementDataKey);
            Assert.AreEqual("hardware", _nativeEngine.ElementDataKey);
        }

        [TestMethod]
        public void BothEvidence_AggregatesProfilesUnderHardware()
        {
            var tacOnly = ExecuteLookup(
                new KeyValuePair<string, string>(
                    SharedConstants.EVIDENCE_QUERY_TAC_KEY,
                    ValidTac));
            var nativeOnly = ExecuteLookup(
                new KeyValuePair<string, string>(
                    SharedConstants.EVIDENCE_QUERY_NATIVE_MODEL_KEY,
                    ValidNativeModel));

            _data.AddEvidence(SharedConstants.EVIDENCE_QUERY_TAC_KEY, ValidTac);
            _data.AddEvidence(
                SharedConstants.EVIDENCE_QUERY_NATIVE_MODEL_KEY,
                ValidNativeModel);
            _data.Process();

            AssertNoErrors(_data);

            var combined = _data.Get<IMultiDeviceData>();
            Assert.IsNotNull(combined);
            Assert.IsTrue(
                combined.Engines.Contains(_tacEngine),
                "The shared hardware aspect should contain the TAC engine.");
            Assert.IsTrue(
                combined.Engines.Contains(_nativeEngine),
                "The shared hardware aspect should contain the Native engine.");

            var combinedProfiles = combined.Profiles
                .Select(GetProfileSignature)
                .ToHashSet();
            var expectedProfiles = tacOnly.ProfileSignatures.ToHashSet();
            expectedProfiles.UnionWith(nativeOnly.ProfileSignatures);

            CollectionAssert.AreEquivalent(
                expectedProfiles.OrderBy(i => i).ToList(),
                combinedProfiles.OrderBy(i => i).ToList(),
                "The shared hardware aspect should contain the union of the " +
                "profiles returned by the TAC-only and NativeModel-only " +
                "lookups.");
        }

        private static LookupResult ExecuteLookup(
            params KeyValuePair<string, string>[] evidence)
        {
            using var data = _pipeline.CreateFlowData();

            foreach (var pair in evidence)
            {
                data.AddEvidence(pair.Key, pair.Value);
            }

            data.Process();
            AssertNoErrors(data);

            var result = data.Get<IMultiDeviceData>();
            Assert.IsNotNull(result);
            Assert.AreNotEqual(
                0,
                result.Profiles.Count,
                "Expected the lookup to return at least one profile.");

            return new LookupResult(
                result.Profiles.Select(GetProfileSignature).ToHashSet());
        }

        private static string GetProfileSignature(IDeviceData profile)
        {
            return string.Join(
                "||",
                new[]
                {
                    GetValue(profile.HardwareVendor),
                    GetListValue(profile.HardwareName),
                    GetValue(profile.HardwareModel),
                    GetValue(profile.PlatformName),
                    GetValue(profile.PlatformVersion),
                    GetValue(profile.BrowserName),
                    GetValue(profile.BrowserVersion),
                    GetValue(profile.IsMobile),
                    GetListValue(profile.TAC),
                    GetListValue(profile.NativeModel)
                });
        }

        private static string GetValue<T>(IAspectPropertyValue<T> value)
        {
            if (value == null || value.HasValue == false)
            {
                return "<no-value>";
            }

            return value.Value?.ToString() ?? "<null>";
        }

        private static string GetListValue(
            IAspectPropertyValue<IReadOnlyList<string>> value)
        {
            if (value == null || value.HasValue == false)
            {
                return "<no-value>";
            }

            return string.Join("|", value.Value.OrderBy(i => i));
        }

        private static void AssertNoErrors(IFlowData data)
        {
            Assert.IsNull(
                data.Errors,
                data.Errors == null
                    ? ""
                    : string.Join(
                        "; ",
                        data.Errors.Select(e => e.ExceptionData.Message)));
        }

        private sealed class LookupResult
        {
            public LookupResult(HashSet<string> profileSignatures)
            {
                ProfileSignatures = profileSignatures;
            }

            public HashSet<string> ProfileSignatures { get; }
        }
    }
}
