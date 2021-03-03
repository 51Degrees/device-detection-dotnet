/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL) 
 * v.1.2 and is subject to its terms as set out below.
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

using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Constants = FiftyOne.DeviceDetection.TestHelpers.Constants;

namespace FiftyOne.DeviceDetection.Hash.Tests
{
    [TestCategory("DeviceDetection")]
    [TestCategory("Hash")]
    public class TestsBase
    {
        private static Semaphore _lock = new Semaphore(1, 1);
        protected WrapperHash Wrapper { get; private set; } = null;
        protected UserAgentGenerator UserAgents { get; private set; }

        [TestInitialize]
        public virtual void Init()
        {
            // If the test is running in x86 then we need to take some 
            // extra precautions to prevent occasionally running out
            // of memory.
            if (IntPtr.Size == 4)
            {
                // Ensure that only one integration test is running at once.
                _lock.WaitOne();
                // Force garbage collection
                GC.Collect();
            }
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            Wrapper?.Dispose();
            if (IntPtr.Size == 4)
            {
                _lock.Release();
            }
        }

        protected void TestInitialize(PerformanceProfiles profile)
        {
            Wrapper = new WrapperHash(
                Utils.GetFilePath(Constants.HASH_DATA_FILE_NAME),
                profile);
            UserAgents = new UserAgentGenerator(
                Utils.GetFilePath(Constants.UA_FILE_NAME));
        }

    }
}
