using FiftyOne.DeviceDetection.Examples;
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
    public class GettingStartedOnPremiseTest : WebExampleTestBase
    {
        private string DATA_FILE_PATH { get; set; }

        /// <summary>
        /// Test that the 'GettingStartedWeb' example is able to run without crashing
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task VerifyExample_OnPremise()
        {
            GetKeyFromEnv("DEVICE_DETECTION_DATAFILE", v => DATA_FILE_PATH = v);
            await VerifyExample<Examples.OnPremise.GettingStartedWeb.Startup>(DATA_FILE_PATH);
        }

        private async Task VerifyExample<T>(
            string dataFilePath) where T : class
        {
            if(string.IsNullOrWhiteSpace(dataFilePath))
            {
                dataFilePath = ExampleUtils.FindFile("51Degrees-LiteV4.1.hash");
            }

            if (string.IsNullOrWhiteSpace(dataFilePath))
            {
                Assert.Inconclusive("Unable to run this test as no data file path was passed " +
                    "from the relevant environment variables. " +
                    "(See ClientHintsExampleTestBase.GetEnvVars)");
            }

            // Determine the path to the app settings file we need to use
            string appSettingsPath = Environment.CurrentDirectory;
            var pos = appSettingsPath.IndexOf("FiftyOne.DeviceDetection");
            appSettingsPath = appSettingsPath.Remove(pos);
            appSettingsPath = Path.Combine(
                appSettingsPath,
                "Examples",
                "OnPremise",
                "GettingStarted-Web",
                "appsettings.json");

            // Create the web server to handle the requests
            using (var server = InitializeOnPremTestServer<T>(
                appSettingsPath, null, dataFilePath))
            using (var http = server.CreateClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get
                };
                request.Headers.Add("User-Agent", "abc");
                var response = await http.SendAsync(request);

                // Verify the request was successful.
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                if (request != null) { request.Dispose(); }
                if (response != null) { response.Dispose(); }
            }
        }

    }

}
