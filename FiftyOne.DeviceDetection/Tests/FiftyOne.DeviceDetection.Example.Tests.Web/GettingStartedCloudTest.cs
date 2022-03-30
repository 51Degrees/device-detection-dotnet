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
    public class GettingStartedCloudTest : WebExampleTestBase
    {
        public static string SUPER_KEY { get; private set; }

        /// <summary>
        /// Test that the 'GettingStartedWeb' example is able to run without crashing
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task VerifyExample_Cloud()
        {
            GetKeyFromEnv("SUPER_RESOURCE_KEY", v => SUPER_KEY = v);
            await VerifyExample<Examples.Cloud.GettingStartedWeb.Startup>(SUPER_KEY);
        }

        private async Task VerifyExample<T>(
            string resourceKey) where T : class
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
            {
                Assert.Inconclusive("Unable to run this test as no resource key was passed " +
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
                "Cloud",
                "GettingStarted-Web",
                "appsettings.json");

            // Create the web server to handle the requests
            using (var server = InitializeCloudTestServer<T>(
                appSettingsPath, resourceKey))
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
