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

using FiftyOne.DeviceDetection.PropertyKeyed.Data;
using FiftyOne.DeviceDetection.PropertyKeyed.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    /// <summary>
    /// Tests for <see cref="NativePropertiesEngine"/>.
    /// Uses IsMobile and PlatformName as indexed properties with the
    /// Lite data file to verify the base engine behaviour without 
    /// TAC-specific validation.
    /// </summary>
    [TestClass]
    public class NativePropertiesEngineTests
    {
        private static ILoggerFactory _loggerFactory;
        private static NativePropertiesEngine _engine;
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
            _engine = new NativePropertiesEngineBuilder(
                    _loggerFactory,
                    new Mock<IDataUpdateService>().Object)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .SetProperty("IsMobile")
                .SetProperty("PlatformName")
                .Build(ddFile, false);
            _pipeline = new PipelineBuilder(_loggerFactory)
                .AddFlowElement(_engine)
                .SetSuppressProcessExceptions(true)
                .SetAutoDisposeElements(true)
                .Build();
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

        /// <summary>
        /// Valid evidence for IsMobile should return at least one profile.
        /// </summary>
        [TestMethod]
        [DataRow("IsMobile", "True")]
        [DataRow("IsMobile", "False")]
        [DataRow("IsMobile", "true")]
        [DataRow("IsMobile", "false")]
        [DataRow("PlatformName", "Windows")]
        [DataRow("PlatformName", "iOS")]
        public void GoodEvidence_ReturnsProfiles(
            string property, string value)
        {
            _data.AddEvidence("query." + property, value);
            _data.Process();
            Assert.IsNull(_data.Errors,
                _data.Errors == null ? "" :
                    string.Join("; ", _data.Errors.Select(
                        e => e.ExceptionData.Message)));
            var mdd = _data.Get<IMultiDeviceData>();
            Assert.IsNotNull(mdd);
            Assert.AreNotEqual(0, mdd.Profiles.Count,
                $"Expected at least one profile for {property}={value}.");
        }

        /// <summary>
        /// A property value not found in the index should produce an error.
        /// </summary>
        [TestMethod]
        [DataRow("IsMobile", "Missing")]
        [DataRow("IsMobile", "")]
        public void MissingValue_AddsError(string property, string value)
        {
            _data.AddEvidence("query." + property, value);
            _data.Process();
            Assert.IsNotNull(_data.Errors);
            var message = _data.Errors.First().ExceptionData.Message;
            StringAssert.Contains(
                message, property,
                $"Error message should contain property '{property}'.");
            StringAssert.Contains(
                message, value,
                $"Error message should contain value '{value}'.");
            var mdd = _data.Get<IMultiDeviceData>();
            Assert.IsNotNull(mdd);
        }

        /// <summary>
        /// No evidence at all — engine is not processed.
        /// </summary>
        [TestMethod]
        public void NoEvidence_NoProcessing()
        {
            _data.Process();
            Assert.IsNull(_data.Errors);
            Assert.ThrowsExactly<PipelineDataException>(() =>
                _data.Get<IMultiDeviceData>());
        }

        /// <summary>
        /// Engine properties should not be empty.
        /// </summary>
        [TestMethod]
        public void Properties_NotEmpty()
        {
            Assert.AreNotEqual(0, _engine.Properties.Count);
        }

        /// <summary>
        /// Refreshing data should fail.
        /// </summary>
        [TestMethod]
        public void RefreshData_Throws()
        {
            Assert.ThrowsExactly<Exception>(() =>
                _engine.RefreshData(""));
        }
    }
}
