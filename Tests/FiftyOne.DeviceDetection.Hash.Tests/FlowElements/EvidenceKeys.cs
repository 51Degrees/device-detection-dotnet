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

using FiftyOne.DeviceDetection.TestHelpers.FlowElements;
using FiftyOne.Pipeline.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiftyOne.DeviceDetection.Hash.Tests.FlowElements
{
    [TestClass]
    [TestCategory("Core")]
    [TestCategory("EvidenceKeys")]
    public class EvidenceKeysHashCoreTests : TestsBase
    {

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void EvidenceKeys_Hash_Core_ContainsUserAgent(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            EvidenceKeyTests.ContainsUserAgent(Wrapper);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void EvidenceKeys_Hash_Core_ContainsHeaderNames(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            EvidenceKeyTests.ContainsHeaderNames(Wrapper);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void EvidenceKeys_Hash_Core_ContainsQueryParams(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            EvidenceKeyTests.ContainsQueryParams(Wrapper);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void EvidenceKeys_Hash_Core_ContainsOverrides(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            EvidenceKeyTests.ContainsOverrides(Wrapper);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void EvidenceKeys_Hash_Core_CaseInsensitiveKeys(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            EvidenceKeyTests.CaseInsensitiveKeys(Wrapper);
        }
    }
}
