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

using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
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

        /// <summary>
        /// Get all the possible profiles including null.
        /// </summary>
        /// <returns></returns>
        protected static IEnumerable<object[]> GetPerformanceProfiles()
        {
            yield return new object[] { null };
            foreach (var profile in Enum.GetValues(typeof(PerformanceProfiles)))
            {
                yield return new object[] { profile };
            }
        }

        /// <summary>
        /// Initializes the Wrapper and UserAgents for the specific test.
        /// </summary>
        /// <param name="profile">
        /// Performance profile to use, or null for in memory
        /// </param>
        /// <param name="dataFileName">
        /// Source hash device detection data file
        /// </param>
        protected void InitWrapperAndUserAgents(
            PerformanceProfiles? profile,
            String dataFileName = Constants.LITE_HASH_DATA_FILE_NAME)
        {
            var filePath = TestHelpers.Utils.GetFilePath(dataFileName);
            if (profile == null)
            {
                using (var stream = filePath.OpenRead())
                {
                    Wrapper = new WrapperHash(stream);
                }
            }
            else
            {
                Wrapper = new WrapperHash(
                    filePath,
                    profile.Value);
            }
            UserAgents = new UserAgentGenerator(
                TestHelpers.Utils.GetFilePath(Constants.UA_FILE_NAME));
        }
    }
}
