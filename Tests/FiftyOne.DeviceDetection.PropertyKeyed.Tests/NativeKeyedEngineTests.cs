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
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        /// A NativeModel that is too short must add an error.
        /// </summary>
        [TestMethod]
        [DataRow("X")]
        public void InvalidNativeModel_AddsError(string nativeModel)
        {
            _data.AddEvidence("query.nativemodel", nativeModel);
            _data.Process();
            Assert.IsNotNull(_data.Errors,
                "Expected an error for invalid NativeModel.");
        }

        /// <summary>
        /// An invalid NativeModel is caller input, so it must be surfaced via
        /// <see cref="FiftyOne.Pipeline.Core.Data.IFlowData.Errors"/> without
        /// being logged at Error level - an Error log carrying the exception is
        /// recorded as an AppInsights exception (see cloud #201).
        /// </summary>
        [TestMethod]
        [DataRow("X")]
        public void InvalidNativeModel_DoesNotLogError(string nativeModel)
        {
            _data.AddEvidence("query.nativemodel", nativeModel);
            _data.Process();
            Assert.IsNotNull(_data.Errors,
                "Expected an error for invalid NativeModel.");
            AssertNoErrorLevelLog();
        }
    }
}
