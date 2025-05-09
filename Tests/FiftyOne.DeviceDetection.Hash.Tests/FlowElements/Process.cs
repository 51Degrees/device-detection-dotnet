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

using FiftyOne.DeviceDetection.Hash.Tests.Data;
using FiftyOne.DeviceDetection.TestHelpers.FlowElements;
using FiftyOne.Pipeline.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiftyOne.DeviceDetection.Hash.Tests.FlowElements
{
    [TestClass]
    [TestCategory("Core")]
    [TestCategory("Process")]
    public class ProcessHashCoreTests : TestsBase
    {
        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_NoEvidence(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.NoEvidence(Wrapper, new DataValidatorHash(Wrapper.Engine));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_EmptyUserAgent(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.EmptyUserAgent(Wrapper, new DataValidatorHash(Wrapper.Engine));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_NoHeaders(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.NoHeaders(Wrapper, new DataValidatorHash(Wrapper.Engine));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_NoUsefulHeaders(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.NoUsefulHeaders(Wrapper, new DataValidatorHash(Wrapper.Engine));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_CaseInsensitiveKeys(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.CaseInsensitiveEvidenceKeys(Wrapper, new DataValidatorHash(Wrapper.Engine));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_ProfileOverride(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.ProfileOverride(Wrapper, new DataValidatorHash(Wrapper.Engine));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_ProfileOverrideNoHeaders(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.ProfileOverrideNoHeaders(Wrapper, new DataValidatorHash(Wrapper.Engine));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_DeviceId(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.DeviceId(Wrapper, new DataValidatorHash(Wrapper.Engine));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_MetaDataService_DefaultProfilesIds(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.MetaDataService_DefaultProfilesIds(Wrapper);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_MetaDataService_ComponentIdForProfile(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.MetaDataService_ComponentIdForProfile(Wrapper);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetPerformanceProfiles), typeof(TestsBase), DynamicDataSourceType.Method)]
        public void Process_Hash_Core_MetaDataService_DefaultProfileIdForComponent(PerformanceProfiles profile)
        {
            InitWrapperAndUserAgents(profile);
            ProcessTests.MetaDataService_DefaultProfileIdForComponent(Wrapper);
        }
    }
}
