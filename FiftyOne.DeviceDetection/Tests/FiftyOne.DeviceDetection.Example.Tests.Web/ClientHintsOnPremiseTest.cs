using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    [TestClass]
    public class ClientHintsOnPremiseTest : ClientHintsExampleTestBase
    {
        private string DataFilePath { get; set; }

        [TestInitialize]
        public void Init()
        {
            var dataFilePath = Environment.GetEnvironmentVariable(
                "DEVICE_DETECTION_DATAFILE");
            if (string.IsNullOrWhiteSpace(dataFilePath) == false)
            {
                DataFilePath = dataFilePath;
            }
            else
            {
                DataFilePath = string.Empty;
            }
        }

        /// <summary>
        /// Test that the 'Client_Hints_NetCore_31' example is 
        /// returning the expected Accept-CH header values for various
        /// different scenarios.
        /// </summary>
        /// <param name="requestedProperties">
        /// The properties that will be requested by the device 
        /// detection engine running on the example web server.
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
        [DynamicData(nameof(VerifyOnPremExample_DATA), DynamicDataSourceType.Method)]
        public async Task VerifyExample_OnPremise(
            string requestedProperties,
            string userAgent,
            List<string> expectedAcceptCH)
        {
            await VerifyExampleAsync<Client_Hints_NetCore_31.Startup>(
                requestedProperties,
                userAgent,
                expectedAcceptCH,
                "AspNetCore3.1-UACH");
        }

        /// <summary>
        /// Test that the 'Client_Hints_NetCore_31' example is 
        /// returning the expected Accept-CH header values for various
        /// different scenarios.
        /// </summary>
        /// <param name="requestedProperties">
        /// The properties that will be requested by the device 
        /// detection engine running on the example web server.
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
        [DynamicData(nameof(VerifyOnPremExample_DATA), DynamicDataSourceType.Method)]
        public async Task VerifyNonIntegratedExample_OnPremise(
            string requestedProperties,
            string userAgent,
            List<string> expectedAcceptCH)
        {
            await VerifyExampleAsync<Client_Hints_Not_Integrated_NetCore_31.Startup>(
                requestedProperties,
                userAgent,
                expectedAcceptCH,
                "AspNetCore3.1-UACH-manual");
        }

        private async Task VerifyExampleAsync<T>(
            string requestedProperties,
            string userAgent,
            List<string> expectedAcceptCH,
            string exampleDirName) where T : class
        {
            if (string.IsNullOrEmpty(DataFilePath))
            {
                Assert.Inconclusive(
                    "These tests will not pass when using the free 'lite' " +
                    "data file that is included with the source code. " +
                    "A paid-for data file can be obtained from " +
                    "http://51degrees.com/pricing You can then configure " +
                    "the DEVICE_DETECTION_DATAFILE environment variable " +
                    "with the full path to the file.");
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
            using (var server = InitializeOnPremTestServer<T>(
                appSettingsPath, requestedProperties, DataFilePath))
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
        /// on-premise examples.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> VerifyOnPremExample_DATA()
        {
            List<string> Properties = new List<string>()
            {
                ALL_PROPERTIES,
                BROWSER_PROPERTIES,
                HARDWARE_PROPERTIES,
                PLATFORM_PROPERTIES,
                BASE_PROPERTIES
            };

            List<string> UserAgents = new List<string>()
            {
                CHROME_UA,
                EDGE_UA,
                FIREFOX_UA,
                SAFARI_UA,
                CURL_UA
            };

            // Get all combinations of properties and uas
            var joined = Properties
                .Join(UserAgents, (x) => true, (x) => true,
                    (a, b) => new { Properties = a, UserAgent = b });

            foreach (var permutation in joined)
            {
                var expectedAcceptCH = new List<string>();

                // Determine which values we are expecting to
                // see in Accept-CH.
                if (permutation.UserAgent == CHROME_UA ||
                    permutation.UserAgent == EDGE_UA)
                {
                    if (permutation.Properties == BROWSER_PROPERTIES ||
                        permutation.Properties == ALL_PROPERTIES)
                    {
                        expectedAcceptCH.AddRange(BROWSER_ACCEPT_CH);
                    }
                    if (permutation.Properties == HARDWARE_PROPERTIES ||
                        permutation.Properties == ALL_PROPERTIES)
                    {
                        expectedAcceptCH.AddRange(HARDWARE_ACCEPT_CH);
                    }
                    if (permutation.Properties == PLATFORM_PROPERTIES ||
                        permutation.Properties == ALL_PROPERTIES)
                    {
                        expectedAcceptCH.AddRange(PLATFORM_ACCEPT_CH);
                    }
                }

                yield return new object[]
                {
                    permutation.Properties,
                    permutation.UserAgent,
                    expectedAcceptCH
                };
            }
        }
    }
}
