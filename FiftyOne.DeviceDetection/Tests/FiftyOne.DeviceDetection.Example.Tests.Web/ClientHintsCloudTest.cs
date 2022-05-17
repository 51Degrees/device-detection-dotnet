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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    /// <summary>
    /// </summary>
    [TestClass]
    public class ClientHintsCloudTest : ClientHintsExampleTestBase
    {
        private string CloudEndPoint { get; set; }

        [TestInitialize]
        public void Init()
        {
            var cloudEndPoint = Environment.GetEnvironmentVariable("51D_CLOUD_ENDPOINT");
            if (string.IsNullOrWhiteSpace(cloudEndPoint) == false)
            {
                CloudEndPoint = cloudEndPoint;
            }
            else
            {
                CloudEndPoint = string.Empty;
            }
        }

        /// <summary>
        /// Test that the 'Cloud_Client_Hints_NetCore_31' example is 
        /// returning the expected Accept-CH header values for various
        /// different scenarios.
        /// </summary>
        /// <param name="resourceKey">
        /// The resource key to use when creating the web server 
        /// running the example.
        /// </param>
        /// <param name="userAgent">
        /// The value to set the User-Agent header to when making 
        /// a request to the example web server.
        /// </param>
        /// <param name="expectedAcceptCH">
        /// A list of the values that are expected in the Accept-CH 
        /// header in the response from the example web server.
        /// </param>
        /// <returns></returns>
        [DataTestMethod]
        [DynamicData(nameof(VerifyCloudExample_DATA), DynamicDataSourceType.Method)]
        public async Task VerifyExample_Cloud(
            string resourceKey, 
            string userAgent, 
            List<string> expectedAcceptCH)
        {
            await VerifyExample<Cloud_Client_Hints_NetCore_31.Startup>(
                resourceKey,
                userAgent,
                expectedAcceptCH,
                "Cloud-AspNetCore3.1-UACH");
        }

        /// <summary>
        /// Test that the 'Cloud_Client_Hints_Not_Integrated_NetCore_31' 
        /// example is returning the expected Accept-CH header values 
        /// for various different scenarios.
        /// </summary>
        /// <param name="resourceKey">
        /// The resource key to use when creating the web server 
        /// running the example.
        /// </param>
        /// <param name="userAgent">
        /// The value to set the User-Agent header to when making 
        /// a request to the example web server.
        /// </param>
        /// <param name="expectedAcceptCH">
        /// A list of the values that are expected in the Accept-CH 
        /// header in the response from the example web server.
        /// </param>
        /// <returns></returns>
        [DataTestMethod]
        [DynamicData(nameof(VerifyCloudExample_DATA), DynamicDataSourceType.Method)]
        public async Task VerifyNonIntegratedExample_Cloud(
            string resourceKey,
            string userAgent,
            List<string> expectedAcceptCH)
        {
            await VerifyExample<Cloud_Client_Hints_Not_Integrated_NetCore_31.Startup>(
                resourceKey,
                userAgent,
                expectedAcceptCH,
                "Cloud-AspNetCore3.1-UACH-manual");
        }

        private async Task VerifyExample<T>(
            string resourceKey,
            string userAgent,
            List<string> expectedAcceptCH,
            string exampleDirName) where T : class
        {
            if(string.IsNullOrWhiteSpace(resourceKey))
            {
                Assert.Inconclusive("Unable to run this test as no " +
                    "resource key was passed from the relevant environment " +
                    "variables. (See ClientHintsExampleTestBase.GetEnvVars)");
            }

            // Determine the path to the app settings file we need to used
            string appSettingsPath = Environment.CurrentDirectory;
            var pos = appSettingsPath.IndexOf("FiftyOne.DeviceDetection");
            appSettingsPath = appSettingsPath.Remove(pos + "FiftyOne.DeviceDetection".Length);
            appSettingsPath = Path.Combine(
                appSettingsPath,
                "Examples",
                exampleDirName,
                "appsettings.json");

            // Create the web server to handle the requests
            using (var server = InitializeCloudTestServer<T>(
                appSettingsPath, resourceKey, CloudEndPoint))
            using (var http = server.CreateClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get
                };
                request.Headers.Add("User-Agent", userAgent);
                var response = await http.SendAsync(request);

                // Verify the request was successful.
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                VerifyAcceptCH(response.Headers, expectedAcceptCH);

                if (request != null) { request.Dispose(); }
                if (response != null) { response.Dispose(); }
            }
        }

        /// <summary>
        /// Used to supply parameters to the VerifyExample tests for
        /// cloud-backed examples.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> VerifyCloudExample_DATA()
        {
            GetEnvVars();
            List<string> ResourceKeys = new List<string>()
            {
                SUPER_KEY,
                BROWSER_KEY,
                HARDWARE_KEY,
                PLATFORM_KEY,
                NO_ACCEPTCH_KEY
            };

            List<string> UserAgents = new List<string>()
            {
                CHROME_UA,
                // TODO - Edge removed from test until cloud has been updated
                // with new data file.
                //EDGE_UA,
                FIREFOX_UA,
                SAFARI_UA,
                CURL_UA
            };

            // Get all combinations of keys and uas
            var joined = ResourceKeys
                .Join(UserAgents, (x) => true, (x) => true,
                    (a, b) => new { ResourceKey = a, UserAgent = b });

            foreach (var permutation in joined)
            {
                var expectedAcceptCH = new List<string>();

                // Determine which values we are expecting to
                // see in Accept-CH.
                if (permutation.UserAgent == CHROME_UA ||
                    permutation.UserAgent == EDGE_UA)
                {
                    if (permutation.ResourceKey == BROWSER_KEY ||
                        permutation.ResourceKey == SUPER_KEY)
                    {
                        expectedAcceptCH.AddRange(BROWSER_ACCEPT_CH);
                    }
                    if (permutation.ResourceKey == HARDWARE_KEY ||
                        permutation.ResourceKey == SUPER_KEY)
                    {
                        expectedAcceptCH.AddRange(HARDWARE_ACCEPT_CH);
                    }
                    if (permutation.ResourceKey == PLATFORM_KEY ||
                        permutation.ResourceKey == SUPER_KEY)
                    {
                        expectedAcceptCH.AddRange(PLATFORM_ACCEPT_CH);
                    }
                }

                yield return new object[]
                {
                    permutation.ResourceKey,
                    permutation.UserAgent,
                    expectedAcceptCH
                };
            }
        }

    }

}
