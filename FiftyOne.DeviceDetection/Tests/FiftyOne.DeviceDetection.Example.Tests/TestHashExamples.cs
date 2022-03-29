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
        //private string LicenseKey;

        private string DataFile;
        //private string UserAgentsFile;
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
            //// Set license key for autoupdate examples.
            //var licenseKey = Environment.GetEnvironmentVariable("DEVICEDETECTIONLICENSEKEY");
            //LicenseKey = string.IsNullOrWhiteSpace(licenseKey) == false ?
            //    licenseKey : "!!YOUR_LICENSE_KEY!!";

            //if (string.IsNullOrWhiteSpace(LicenseKey) == true ||
            //    LicenseKey.StartsWith("!!") == true)
            //{
            //    Assert.Fail("LicenseKey must be specified in the Init method" +
            //        " or as an Environment variable");
            //}

            // Set Device Data file
            DataFile = Environment.GetEnvironmentVariable("DEVICEDETECTIONDATAFILE");
            if (string.IsNullOrWhiteSpace(DataFile))
            {
                DataFile = ExampleUtils.FindFile("51Degrees-LiteV4.1.hash");
            }

            //// Set User-Agents file for performance example.
            //UserAgentsFile = Environment.GetEnvironmentVariable("USERAGENTSFILE");
            //if (string.IsNullOrWhiteSpace(UserAgentsFile))
            //{
            //    UserAgentsFile = ExampleUtils.FindFile("20000 User Agents.csv");
            //}
        }

        /// <summary>
        /// Test the GettingStarted Example
        /// </summary>
        [TestMethod]
        public void Example_Hash_GettingStarted()
        {
            var example = new Examples.OnPremise.GettingStartedConsole.Program.Example();
            example.Run(DataFile, new LoggerFactory(), TextWriter.Null);
        }
    }
}
