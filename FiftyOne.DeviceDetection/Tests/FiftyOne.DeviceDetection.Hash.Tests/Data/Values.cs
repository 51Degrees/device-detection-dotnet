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

using FiftyOne.DeviceDetection.TestHelpers.Data;
using FiftyOne.Pipeline.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiftyOne.DeviceDetection.Hash.Tests.Core.Data
{
    [TestClass]
    [TestCategory("Core")]
    [TestCategory("Values")]
    public class ValuesHashCoreTests : TestsBase
    {

        [DataTestMethod]
        [DataRow(PerformanceProfiles.HighPerformance)]
        [DataRow(PerformanceProfiles.MaxPerformance)]
        [DataRow(PerformanceProfiles.LowMemory)]
        [DataRow(PerformanceProfiles.Balanced)]
        public void Values_Hash_Core_ValueTypes(PerformanceProfiles profile)
        {
            TestInitialize(profile);
            ValueTests.ValueTypes(Wrapper);
        }

        [DataTestMethod]
        [DataRow(PerformanceProfiles.HighPerformance)]
        [DataRow(PerformanceProfiles.MaxPerformance)]
        [DataRow(PerformanceProfiles.LowMemory)]
        [DataRow(PerformanceProfiles.Balanced)]
        public void Values_Hash_Core_AvailableProperties(PerformanceProfiles profile)
        {
            TestInitialize(profile);
            ValueTests.AvailableProperties(Wrapper);
        }

        [DataTestMethod]
        [DataRow(PerformanceProfiles.HighPerformance)]
        [DataRow(PerformanceProfiles.MaxPerformance)]
        [DataRow(PerformanceProfiles.LowMemory)]
        [DataRow(PerformanceProfiles.Balanced)]
        public void Values_Hash_Core_TypedGetters(PerformanceProfiles profile)
        {
            TestInitialize(profile);
            ValueTests.TypedGetters(Wrapper);
        }

        [DataTestMethod]
        [DataRow(PerformanceProfiles.HighPerformance)]
        [DataRow(PerformanceProfiles.MaxPerformance)]
        [DataRow(PerformanceProfiles.LowMemory)]
        [DataRow(PerformanceProfiles.Balanced)]
        public void Values_Hash_Core_DeviceId(PerformanceProfiles profile)
        {
            TestInitialize(profile);
            ValueTests.DeviceId(Wrapper);
        }

        [DataTestMethod]
        [DataRow(PerformanceProfiles.HighPerformance)]
        [DataRow(PerformanceProfiles.MaxPerformance)]
        [DataRow(PerformanceProfiles.LowMemory)]
        [DataRow(PerformanceProfiles.Balanced)]
        public void Values_Hash_Core_MatchedUserAgents(PerformanceProfiles profile)
        {
            TestInitialize(profile);
            ValueTests.MatchedUserAgents(Wrapper);
        }
    }
}
