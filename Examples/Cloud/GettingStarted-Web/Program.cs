/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
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

using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Examples.Cloud.GettingStartedWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Get any configuration overrides we need.
            var overrides = CreateConfigOverrides();
            CreateHostBuilder(overrides, args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(
            IDictionary<string, string> overrides, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(l =>
                    {
                        l.ClearProviders()
                            .AddConsole();
                    })
                    .ConfigureAppConfiguration(builder =>
                    {
                        builder.AddJsonFile("appsettings.json")
                            .AddInMemoryCollection(overrides);
                    })
                    .UseStartup<Startup>();
                });

        /// <summary>
        /// This section would not normally be needed. We're just checking for the resource key
        /// so that if it's not set, we can override it with the one from the environment variables
        /// or show a message that's very clear about what needs to be done in the content of 
        /// this example.
        /// </summary>
        private static Dictionary<string, string> CreateConfigOverrides()
        {
            var result = new Dictionary<string, string>();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            PipelineOptions options = new PipelineOptions();
            config.Bind("PipelineOptions", options);

            // Get the resource key setting from the config file. 
            var resourceKeyFromConfig = options.GetResourceKey();
            var configHasKey = string.IsNullOrWhiteSpace(resourceKeyFromConfig) == false &&
                    resourceKeyFromConfig.StartsWith("!!") == false;

            if(configHasKey == false)
            {
                // Get the index of the cloud request engine element in the config file so that
                // we can create an override key for it.
                var cloudEngineOptions = options.GetElementConfig(nameof(CloudRequestEngine));
                var cloudEngineIndex = options.Elements.IndexOf(cloudEngineOptions);
                var resourceKeyConfigKey = $"PipelineOptions:Elements:{cloudEngineIndex}" +
                    $":BuildParameters:ResourceKey";

                string resourceKey = Environment.GetEnvironmentVariable(
                        ExampleUtils.RESOURCE_KEY_ENV_VAR);

                if (string.IsNullOrEmpty(resourceKey) == false)
                {
                    result.Add(resourceKeyConfigKey, resourceKey);
                }
                else
                {
                    throw new Exception($"No resource key specified in the configuration file " +
                        $"'appsettings.json' or the environment variable " +
                        $"'{ExampleUtils.RESOURCE_KEY_ENV_VAR}'. The 51Degrees cloud " +
                        $"service is accessed using a 'ResourceKey'. For more information " +
                        $"see http://51degrees.com/documentation/4.3/_info__resource_keys.html. " +
                        $"A resource key with the properties required by this example can be " +
                        $"created for free at https://configure.51degrees.com/1QWJwHxl. " +
                        $"Once complete, populate the config file or environment variable " +
                        $"mentioned at the start of this message with the key.");
                }
            }

            return result;
        }
    }
}
