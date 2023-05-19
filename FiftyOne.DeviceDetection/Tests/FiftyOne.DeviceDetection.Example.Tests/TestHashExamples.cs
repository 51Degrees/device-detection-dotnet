/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, Davidson House,
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using FiftyOne.DeviceDetection.Examples;

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
        private string EvidenceFile;
        //private int Count = 20000;

        /// <summary>
        /// Init method - specify License Key to run examples here or 
        /// set a License Key in an environment variable called 'ResourceKey'.
        /// Set data file for hash examples and additionally a User-Agents file
        /// for the performance example.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            // Set license key for autoupdate examples.
            var licenseKey = Environment.GetEnvironmentVariable(Constants.LICENSE_KEY_ENV_VAR);
            LicenseKey = string.IsNullOrWhiteSpace(licenseKey) == false ?
                licenseKey : "!!YOUR_LICENSE_KEY!!";

            // Set Device Data file
            DataFile = Environment.GetEnvironmentVariable("DEVICEDETECTIONDATAFILE");
            if (string.IsNullOrWhiteSpace(DataFile))
            {
                DataFile = ExampleUtils.FindFile(Constants.LITE_HASH_DATA_FILE_NAME);
            }

            // Set evidence file for offline processing example.
            EvidenceFile = Environment.GetEnvironmentVariable("EVIDENCEFILE");
            if (string.IsNullOrWhiteSpace(EvidenceFile))
            {
                EvidenceFile = ExampleUtils.FindFile(Constants.YAML_EVIDENCE_FILE_NAME);
            }
        }

        /// <summary>
        /// Test the GettingStarted Example
        /// </summary>
        [TestMethod]
        public void Example_OnPremise_GettingStarted()
        {
            var example = new Examples.OnPremise.GettingStartedConsole.Program.Example();
            example.Run(DataFile, new LoggerFactory(), TextWriter.Null);
        }

        /// <summary>
        /// Test the GettingStarted Example
        /// </summary>
        [TestMethod]
        public void Example_OnPremise_OfflineProcessing()
        {
            var example = new Examples.OnPremise.OfflineProcessing.Program.Example();
            using (var reader = new StreamReader(File.OpenRead(EvidenceFile)))
            {
                example.Run(DataFile, reader, new LoggerFactory(), TextWriter.Null);
            }
        }

        /// <summary>
        /// Test the Metadata Example
        /// </summary>
        [TestMethod]
        public void Example_OnPremise_Metadata()
        {
            var example = new Examples.OnPremise.Metadata.Program.Example();
            example.Run(DataFile, new LoggerFactory(), TextWriter.Null);
        }

        /// <summary>
        /// Test the UpdateDataFile Example
        /// </summary>
        [TestMethod]
        public void Example_OnPremise_UpdateDataFile()
        {
            VerifyLicenseKeyAvailable();
            Examples.OnPremise.UpdateDataFile.Program.Initialize(
                Path.GetFileName(DataFile), LicenseKey, false);
        }

        /// <summary>
        /// Test the match metrics Example
        /// </summary>
        [TestMethod]
        public void Example_OnPremise_MatchMetrics()
        {
            Examples.OnPremise.MatchMetrics.Program.Initialize(
                Path.GetFileName(DataFile), TextWriter.Null);
        }

        private void VerifyLicenseKeyAvailable()
        {
            if (string.IsNullOrWhiteSpace(LicenseKey) == true ||
                LicenseKey.StartsWith("!!") == true)
            {
                Assert.Inconclusive("This test requires a 51Degrees license key. This can be " +
                    "specified in the TestHashExamples.Init method or by setting an Environment " +
                    $"variable called '{Constants.LICENSE_KEY_ENV_VAR}'");
            }
        }
    }
}
