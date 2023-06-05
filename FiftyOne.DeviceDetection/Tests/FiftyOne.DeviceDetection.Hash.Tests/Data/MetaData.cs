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

using FiftyOne.DeviceDetection.Hash.Tests.Data;
using FiftyOne.DeviceDetection.TestHelpers.Data;
using FiftyOne.Pipeline.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Hash.Tests.Core.Data
{
    [TestClass]
    [TestCategory("Core")]
    [TestCategory("MetaData")]
    public class MetaDataHashCoreTests : TestsBase
    {
        private const int TEST_DELAY_MS = 3000;

        //[DataTestMethod]
        //[DataRow(PerformanceProfiles.HighPerformance)]
        //[DataRow(PerformanceProfiles.MaxPerformance)]
        //[DataRow(PerformanceProfiles.LowMemory)]
        //[DataRow(PerformanceProfiles.Balanced)]
        //public void MetaData_Hash_Core_Reload(PerformanceProfiles profile)
        //{
        //    TestInitialize(profile);
        //    MetaDataTests test = new MetaDataTests();
        //    test.Reload(Wrapper, new MetaDataHasher(), profile);
        //}

        // Note that the 'ReloadMemory' tests are split into separate methods
        // rather than using the 'DataTestMethod' approach because they 
        // often fail due to running out of memory.
        // This way, they do not run into the same problem for some reason.

        [TestMethod]
        public void MetaData_Hash_Core_Reload_LowMemory()
        {
            Task.Delay(TEST_DELAY_MS).Wait();
            TestInitialize(PerformanceProfiles.LowMemory);
            MetaDataTests test = new MetaDataTests();
            test.Reload(Wrapper, new MetaDataHasher(), PerformanceProfiles.LowMemory);
        }
        [TestMethod]
        public void MetaData_Hash_Core_Reload_Balanced()
        {
            Task.Delay(TEST_DELAY_MS).Wait();
            TestInitialize(PerformanceProfiles.Balanced);
            MetaDataTests test = new MetaDataTests();
            test.Reload(Wrapper, new MetaDataHasher(), PerformanceProfiles.Balanced);
        }
        [TestMethod]
        public void MetaData_Hash_Core_Reload_HighPerformance()
        {
            Task.Delay(TEST_DELAY_MS).Wait();
            TestInitialize(PerformanceProfiles.HighPerformance);
            MetaDataTests test = new MetaDataTests();
            test.Reload(Wrapper, new MetaDataHasher(), PerformanceProfiles.HighPerformance);
        }
        [TestMethod]
        public void MetaData_Hash_Core_Reload_MaxPerformance()
        {
            Task.Delay(TEST_DELAY_MS).Wait();
            TestInitialize(PerformanceProfiles.MaxPerformance);
            MetaDataTests test = new MetaDataTests();
            test.Reload(Wrapper, new MetaDataHasher(), PerformanceProfiles.MaxPerformance);
        }

        [TestMethod]
        public void MetaData_Hash_Core_ReloadMemory_LowMemory()
        {
            Task.Delay(TEST_DELAY_MS).Wait();
            TestInitialize(PerformanceProfiles.LowMemory);
            MetaDataTests test = new MetaDataTests();
            test.ReloadMemory(Wrapper, new MetaDataHasher(), PerformanceProfiles.LowMemory);
        }
        [TestMethod]
        public void MetaData_Hash_Core_ReloadMemory_Balanced()
        {
            Task.Delay(TEST_DELAY_MS).Wait();
            TestInitialize(PerformanceProfiles.Balanced);
            MetaDataTests test = new MetaDataTests();
            test.ReloadMemory(Wrapper, new MetaDataHasher(), PerformanceProfiles.Balanced);
        }
        [TestMethod]
        public void MetaData_Hash_Core_ReloadMemory_HighPerformance()
        {
            Task.Delay(TEST_DELAY_MS).Wait();
            TestInitialize(PerformanceProfiles.HighPerformance);
            MetaDataTests test = new MetaDataTests();
            test.ReloadMemory(Wrapper, new MetaDataHasher(), PerformanceProfiles.HighPerformance);
        }
        [TestMethod]
        public void MetaData_Hash_Core_ReloadMemory_MaxPerformance()
        {
            Task.Delay(TEST_DELAY_MS).Wait();
            TestInitialize(PerformanceProfiles.MaxPerformance);
            MetaDataTests test = new MetaDataTests();
            test.ReloadMemory(Wrapper, new MetaDataHasher(), PerformanceProfiles.MaxPerformance);
        }
    }
}
