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

using FiftyOne.Pipeline.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        protected static void GetKeyFromEnv(string envVarName, Action<string> setValue)
        {
            var superKey = Environment.GetEnvironmentVariable(envVarName);
            if (string.IsNullOrWhiteSpace(superKey) == false)
            {
                setValue(superKey);
            }
            else
            {
                setValue(string.Empty);
            }
        }

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
        protected TestServer InitializeCloudTestServer<TStartup>(
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
            if (string.IsNullOrEmpty(cloudEndpoint) == false)
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
                    // Log to console so we can see what happens in the event of a failure.
                    .ConfigureLogging(l =>
                    {
                        l.ClearProviders().AddConsole();
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
