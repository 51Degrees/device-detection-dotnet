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

[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    /// <summary>
    /// Tests for <see cref="TacKeyedEngine"/>.
    /// </summary>
    [TestClass]
    public class TacKeyedEngineTests
    {
        private static ILoggerFactory _loggerFactory;
        private static TacKeyedEngine _engine;
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
            _engine = new TacKeyedEngineBuilder(
                    _loggerFactory,
                    new Mock<IDataUpdateService>().Object)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .SetProperty("TAC")
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
        /// A valid 8-digit TAC should return at least one profile.
        /// </summary>
        [TestMethod]
        public void ValidTac_ReturnsProfiles()
        {
            _data.AddEvidence("query.tac", "35925406");
            _data.Process();
            Assert.IsNull(_data.Errors,
                _data.Errors == null ? "" :
                    string.Join("; ", _data.Errors.Select(
                        e => e.ExceptionData.Message)));
            var mdd = _data.Get<IMultiDeviceData>();
            Assert.IsNotNull(mdd);
            Assert.AreNotEqual(0, mdd.Profiles.Count,
                "Expected at least one profile for a valid TAC.");
        }

        /// <summary>
        /// An invalid TAC (not 8 digits) should add an error.
        /// </summary>
        [TestMethod]
        [DataRow("1234")]
        [DataRow("ABCDEFGH")]
        [DataRow("123456789")]
        public void InvalidTac_AddsError(string tac)
        {
            _data.AddEvidence("query.tac", tac);
            _data.Process();
            Assert.IsNotNull(_data.Errors,
                "Expected an error for invalid TAC.");
        }

        /// <summary>
        /// No evidence — engine is not processed.
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
        /// An unlikely TAC should still be processable without crash.
        /// </summary>
        [TestMethod]
        public void MissingTacValue_NoCrash()
        {
            _data.AddEvidence("query.tac", "00000000");
            _data.Process();
            var mdd = _data.Get<IMultiDeviceData>();
            Assert.IsNotNull(mdd);
        }

        /// <summary>
        /// RefreshData should throw.
        /// </summary>
        [TestMethod]
        public void RefreshData_Throws()
        {
            Assert.ThrowsExactly<Exception>(() =>
                _engine.RefreshData(""));
        }

        /// <summary>
        /// Engine should expose at least one property.
        /// </summary>
        [TestMethod]
        public void Properties_NotEmpty()
        {
            Assert.AreNotEqual(0, _engine.Properties.Count,
                "Engine should have at least one property.");
        }
    }
}
