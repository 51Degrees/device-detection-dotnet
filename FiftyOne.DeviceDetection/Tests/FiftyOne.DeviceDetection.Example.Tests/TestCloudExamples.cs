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
using System;

namespace FiftyOne.DeviceDetection.Example.Tests
{
    /// <summary>
    /// This test class ensures that the cloud examples execute successfully.
    /// </summary>
    /// <remarks>
    /// Note that these test do not generally ensure the correctness 
    /// of the example, only that the example will run without 
    /// crashing or throwing any unhandled exceptions.
    /// </remarks>
    [TestClass]
    public class TestCloudExamples
    {
        private string ResourceKey;
        private string CloudEndPoint;

        /// <summary>
        /// Init method. Specify Resource Key to run examples here or 
        /// set a Resource Key in an environment variable called 'ResourceKey'.
        /// Get cloud endpoint url from environment variables or use default.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            var resourceKey = Environment.GetEnvironmentVariable("DEVICEDETECTIONRESOURCEKEY");
            ResourceKey = string.IsNullOrWhiteSpace(resourceKey) == false ?
                resourceKey : "!!YOUR_RESOURCE_KEY!!";

            if (string.IsNullOrWhiteSpace(ResourceKey) == true ||
                ResourceKey.StartsWith("!!") == true)
            {
                Assert.Fail("ResourceKey must be specified in the Init method" +
                    " or as an Environment variable");
            }

            var cloudEndPoint = Environment.GetEnvironmentVariable("51D_CLOUD_ENDPOINT");
            if(string.IsNullOrWhiteSpace(cloudEndPoint) == false)
            {
                CloudEndPoint = cloudEndPoint;
            } 
            else
            {
                CloudEndPoint = string.Empty;
            }
        }

        /// <summary>
        /// Test the GetAllProperties Example
        /// </summary>
        [TestMethod]
        public void Example_Cloud_GetAllProperties()
        {
            var example = new Examples.Cloud.GetAllProperties.Program.Example();
            example.Run(ResourceKey, CloudEndPoint);
        }

        /// <summary>
        /// Tests the GettingStarted Example
        /// </summary>
        [TestMethod]
        public void Example_Cloud_GettingStarted()
        {
            var example = new Examples.Cloud.GettingStarted.Program.Example();
            example.Run(ResourceKey, CloudEndPoint);
        }

        /// <summary>
        /// Test the NativeModelLookup Example
        /// </summary>
        [TestMethod]
        public void Example_Cloud_NativeModelLookup()
        {
            var example = new Examples.Cloud.NativeModelLookup.Program.Example();
            example.Run(ResourceKey, CloudEndPoint);
        }

        /// <summary>
        /// Test the TacLookup Example
        /// </summary>
        [TestMethod]
        public void Example_Cloud_TacLookup()
        {
            var example = new Examples.Cloud.TacLookup.Program.Example();
            example.Run(ResourceKey, CloudEndPoint);
        }
    }
}
