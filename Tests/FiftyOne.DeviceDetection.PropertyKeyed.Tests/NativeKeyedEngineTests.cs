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

using FiftyOne.DeviceDetection.PropertyKeyed.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    /// <summary>
    /// Tests for <see cref="NativeEngine"/> with NativeModel configuration.
    /// </summary>
    [TestClass]
    public class NativeConfiguredEngineTests : BaseEngineTests<NativeEngine>
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) =>
            ClassInitializeInternal(
                context,
                () => new NativeEngineBuilder(_loggerFactory).Build());

        [ClassCleanup]
        public static void ClassCleanup() => ClassCleanupInternal();

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        /// <summary>
        /// A NativeModel that is too short must be reported as a warning and
        /// must not reach
        /// <see cref="FiftyOne.Pipeline.Core.Data.IFlowData.Errors"/>, since
        /// an error there stops the caller receiving a response at all.
        /// </summary>
        [TestMethod]
        [DataRow("X")]
        [DataRow("")]
        public void InvalidNativeModel_AddsWarning(string nativeModel)
        {
            _data.AddEvidence("query.nativemodel", nativeModel);
            _data.Process();
            AssertReportedAsWarning(_data);
        }

        /// <summary>
        /// An invalid NativeModel is caller input, so it must not be logged
        /// at Error level - an Error log carrying the exception can be
        /// surfaced as exception telemetry.
        /// </summary>
        [TestMethod]
        [DataRow("X")]
        [DataRow("")]
        public void InvalidNativeModel_DoesNotLogError(string nativeModel)
        {
            _data.AddEvidence("query.nativemodel", nativeModel);
            _data.Process();
            AssertNoErrorLevelLog();
        }

        /// <summary>
        /// Reproduces the reported defect. The cloud runs the pipeline
        /// without suppressing process exceptions, so recording an unusable
        /// NativeModel as an error made Process throw an
        /// <see cref="AggregateException"/>. A caller batching lookups then
        /// lost the whole job to a single unusable row. Processing must
        /// complete and simply return no profiles.
        /// </summary>
        [TestMethod]
        [DataRow("X")]
        [DataRow("")]
        public void InvalidNativeModel_DoesNotThrowWhenNotSuppressed(
            string nativeModel)
        {
            using (var data = CreateUnsuppressedFlowData())
            {
                data.AddEvidence("query.nativemodel", nativeModel);
                data.Process();
                AssertReportedAsWarning(data);
            }
        }

        /// <summary>
        /// A well formed NativeModel that matches nothing already returns no
        /// profiles rather than an error, and must keep doing so - it is the
        /// behaviour a malformed value has now been aligned with.
        /// </summary>
        [TestMethod]
        public void UnknownNativeModel_ReportsNothing()
        {
            using (var data = CreateUnsuppressedFlowData())
            {
                data.AddEvidence("query.nativemodel", "ZZUNKNOWNMODEL999");
                data.Process();
                Assert.IsNull(data.Errors,
                    "An unmatched NativeModel is not an error.");
                Assert.IsEmpty(data.GetWarnings(),
                    "An unmatched NativeModel is well formed, so there is " +
                    "nothing to warn the caller about.");
            }
        }
    }
}
