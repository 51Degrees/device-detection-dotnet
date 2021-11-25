using FiftyOne.Pipeline.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    public class WebExampleTestBase
    {
        /// <summary>
        /// Create a test server for a cloud-backed web example.
        /// </summary>
        /// <typeparam name="TStartup">
        /// The startup type to be used by the web host.
        /// </typeparam>
        /// <param name="appsettingsPath">
        /// Complete path to the appsettings.json file to use 
        /// when starting the example.
        /// </param>
        /// <param name="resourceKey">
        /// The resource key to add to the configuration for
        /// the example. 
        /// This key will be used when the example web server
        /// makes a request to the cloud service.
        /// </param>
        /// <param name="cloudRequestEngineConfigIndex">
        /// The index of the cloud request engine in the 
        /// appsettings.json file. 
        /// </param>
        /// <returns></returns>
        protected TestServer InitializeCloudTestServer<TStartup> (
            string appsettingsPath,
            string resourceKey,
            string cloudEndpoint = null,
            int cloudRequestEngineConfigIndex = 0) where TStartup : class
        {
            var requestEnginePrefix = $"PipelineOptions:Elements:" +
                $"{cloudRequestEngineConfigIndex}:BuildParameters:";
            var resourceKeyConfigKey = $"{requestEnginePrefix}ResourceKey";
            var endPointConfigKey = $"{requestEnginePrefix}EndPoint";
            var testConfigOverrides =
                new Dictionary<string, string>()
                {
                    { resourceKeyConfigKey, resourceKey },
                };
            if(cloudEndpoint != null)
            {
                testConfigOverrides.Add(endPointConfigKey, cloudEndpoint);
            }

            return InitializeTestServer<TStartup>(
                appsettingsPath,
                testConfigOverrides);
        }

        /// <summary>
        /// Create a test server for an on-premise web example.
        /// </summary>
        /// <typeparam name="TStartup">
        /// The startup type to be used by the web host.
        /// </typeparam>
        /// <param name="appsettingsPath">
        /// Complete path to the appsettings.json file to use 
        /// when starting the example.
        /// </param>
        /// <param name="properties">
        /// The properties to add to the configuration for
        /// the example. 
        /// These properties will be the ones returned by the device
        /// detection engine.
        /// </param>
        /// <param name="dataFilePath">
        /// Complete path to the data file to use in this test
        /// </param>
        /// <param name="ddEngineConfigIndex">
        /// The index of the device detection engine in the 
        /// appsettings.json file. 
        /// </param>
        /// <returns></returns>
        protected TestServer InitializeOnPremTestServer<TStartup>(
            string appsettingsPath,
            string properties,
            string dataFilePath,
            int ddEngineConfigIndex = 0) where TStartup : class
        {
            var ddEngineParametersPrefix = $"PipelineOptions:Elements:" +
                $"{ddEngineConfigIndex}:BuildParameters:";
            var propertiesConfigKey = $"{ddEngineParametersPrefix}Properties";
            var dataFileConfigKey = $"{ddEngineParametersPrefix}DataFile";
            var testConfigOverrides =
                new Dictionary<string, string>()
                {
                    { propertiesConfigKey, properties }
                };
            if(string.IsNullOrEmpty(dataFilePath) == false)
            {
                testConfigOverrides.Add(dataFileConfigKey, dataFilePath);
            }

            return InitializeTestServer<TStartup>(
                appsettingsPath, 
                testConfigOverrides);
        }

        private TestServer InitializeTestServer<TStartup>(
            string appSettingsPath,
            Dictionary<string, string> testConfigOverrides) 
            where TStartup : class
        {
            TestServer testServer = null;

            try
            {
                testServer = new TestServer(new WebHostBuilder()
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder
                            // First add the appsettings.json file for
                            // this example.
                            .AddJsonFile(appSettingsPath)
                            // Now override the configuration with any
                            // specific values we want to use for the test.
                            .AddInMemoryCollection(testConfigOverrides);
                    })
                    .UseStartup<TStartup>()
                    .UseEnvironment("IntegrationTest"));
            }
            catch(Exception ex)
            {
                // If we got an exception during startup then
                // check if it was due to a missing data file.
                // If so, return an inconclusive result.
                var dataFileEx = CheckForDataFileError(ex);
                if (dataFileEx != null)
                {
                    Assert.Inconclusive(
                        $"This test requires a local data file. " +
                        $"See exception message for details: " +
                        dataFileEx.Message);
                }
                else { throw; }
            }

            return testServer;
        }

        private Exception CheckForDataFileError(Exception ex)
        {
            if (ex.Message.Contains("The data file") &&
                ex.Message.Contains("could not be found"))
            {
                return ex;
            }
            else if (ex.InnerException != null)
            {
                return CheckForDataFileError(ex.InnerException);
            }

            return null;
        }
    }
}
