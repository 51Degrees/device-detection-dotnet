/***********************************************************************
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using System;
using System.IO;

namespace FiftyOne.DeviceDetection.Example.Tests
{
    /// <summary>
    /// This test class ensures that the hash examples execute successfully.
    /// </summary>
    /// <remarks>
    /// Note that these test do not generally ensure the correctness 
    /// of the example, only that the example will run without 
    /// crashing or throwing any unhandled exceptions.
    /// </remarks>
    [TestClass]
    public class TestHashExamples
    {
        private string LicenseKey;

        private string DataFile;
        private string UserAgentsFile;
        private int Count = 20000;

        /// <summary>
        /// Init method - specify License Key to run examples here or 
        /// set a License Key in an environment variable called 'ResourceKey'.
        /// Set data file for hash examples and aditionally a User-Agents file
        /// for the performance example.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            // Set license key for autoupdate examples.
            var licenseKey = Environment.GetEnvironmentVariable("DEVICEDETECTIONLICENSEKEY");
            LicenseKey = string.IsNullOrWhiteSpace(licenseKey) == false ?
                licenseKey : "!!YOUR_LICENSE_KEY!!";

            if (string.IsNullOrWhiteSpace(LicenseKey) == true ||
                LicenseKey.StartsWith("!!") == true)
            {
                Assert.Fail("LicenseKey must be specified in the Init method" +
                    " or as an Environment variable");
            }

            // Set Device Data file
            DataFile = Environment.GetEnvironmentVariable("DEVICEDETECTIONDATAFILE");
            if (string.IsNullOrWhiteSpace(DataFile))
            {
#if NETCORE
                DataFile = "..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV4.1.hash";
#else
                DataFile = "..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV4.1.hash";
#endif
            }

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				Console.WriteLine("Getting Linux Path.");
				DataFile = "../../../../../../device-detection-cxx/device-detection-data/51Degrees-LiteV4.1.hash";
			}

            // Set User-Agents file for performance example.
            UserAgentsFile = Environment.GetEnvironmentVariable("USERAGENTSFILE");
            if (string.IsNullOrWhiteSpace(UserAgentsFile))
            {
#if NETCORE
                UserAgentsFile = "..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\20000 User Agents.csv";
#else
                UserAgentsFile = "..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\20000 User Agents.csv";
#endif
            }	
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				Console.WriteLine("Getting Linux Path.");
				UserAgentsFile = "../../../../../../device-detection-cxx/device-detection-data/20000 User Agents.csv";
			}			
        }


        /// <summary>
        /// Test the ConfigureFromFile example.
        /// </summary>
        [TestMethod]
        public void Example_Hash_ConfigureFromFile()
        {
            var example = new Examples.Hash.ConfigureFromFile.Program.Example();
#if NETCORE
            example.Run("appsettings.json");
#else
            example.Run("App.config");
#endif
        }

        /// <summary>
        /// Test the GettingStarted Example
        /// </summary>
        [TestMethod]
        public void Example_Hash_GettingStarted()
        {
            var example = new Examples.Hash.GettingStarted.Program.Example();
            example.Run(DataFile);
        }

        /// <summary>
        /// Test the MetaData Example
        /// </summary>
        [TestMethod]
        public void Example_Hash_MetaData()
        {
            var example = new Examples.Hash.Metadata.Program.Example();
            example.Run(DataFile, true);
        }

        /// <summary>
        /// Test the Performance Example
        /// </summary>
        [TestMethod]
        public void Example_Hash_Performance()
        {
            var example = new Examples.Hash.Performance.Program.Example();
            example.Run(DataFile, UserAgentsFile, Count);
        }

        /// <summary>
        /// Test the UpdateOnStartUp Example
        /// </summary>
        [TestMethod]
        public void Example_Hash_UpdateOnStartUp()
        {
            var example = new Examples.Hash.AutomaticUpdates.UpdateOnStartUp.Program.Example();
            example.Run(DataFile, LicenseKey);
        }

        /// <summary>
        /// Test the UpdatePollingInterval Example
        /// </summary>
        [TestMethod]
        public void Example_Hash_UpdatePollingInterval()
        {
            var example = new Examples.Hash.AutomaticUpdates.UpdatePollingInterval.Program.Example();
            example.Run(DataFile, LicenseKey);
        }
    }
}
