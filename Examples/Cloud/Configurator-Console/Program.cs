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

using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

/// <summary>
/// This example is displayed at the end of the [Configurator](https://configure.51degrees.com/)
/// process, which is used to create resource keys for use with the 51Degrees cloud service.
/// 
/// It shows how to call the cloud with the newly created key and how to access the values 
/// of the selected properties.
///
/// See [Getting Started](https://51degrees.com/documentation/_examples__device_detection__getting_started__console__cloud.html)
/// for a fuller example.
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// - Microsoft.Extensions.DependencyInjection
/// - Microsoft.Extensions.Logging.Console
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Cloud.Configurator
{
    public class Program
    {
        public class Example
        {
            /// <summary>
            /// Holds a reference to the pipeline instances that is used to perform 
            /// device detection.
            /// </summary>
            private IPipeline _pipeline;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pipeline"></param>
            public Example(IPipeline pipeline)
            {
                _pipeline = pipeline;
            }

            /// <summary>
            /// Run the example
            /// </summary>
            /// <param name="pipeline"></param>
            /// <param name="output"></param>
            public void Run(TextWriter output)
            {
                // Evidence values from a windows 11 device using a browser that supports
                // User-Agent Client Hints.
                var evidence = new Dictionary<string, object>()
                {
                    
                };

                // get a flow data from the singleton pipeline for each detection
                // it's important to free the flowdata when done
                using (var data = _pipeline.CreateFlowData())
                {
                    StringBuilder message = new StringBuilder();

                    // Add the evidence values to the flow data
                    data.AddEvidence(
                        "header.user-agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                        "(KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36");
                    data.AddEvidence(
                        "header.sec-ch-ua-mobile",
                        "?0");
                    data.AddEvidence(
                        "header.sec-ch-ua",
                        "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"98\", " +
                        "\"Google Chrome\";v=\"98\"");
                    data.AddEvidence(
                        "header.sec-ch-ua-platform",
                        "\"Windows\"");
                    data.AddEvidence(
                        "header.sec-ch-ua-platform-version",
                        "\"14.0.0\"");

                    // Process the flow data.
                    data.Process();

                    // Get the results.
                    var device = data.Get<IDeviceData>();

                    output.WriteLine($"device.IsMobile: {device.IsMobile.Value}");
                }
            }

            /// <summary>
            /// This Run method is called by the example test to avoid the need to duplicate the 
            /// service provider setup logic.
            /// </summary>
            /// <param name="options"></param>
            public static void Run(string resourceKey, TextWriter output)
            {
                // Initialize a service collection which will be used to create the services
                // required by the Pipeline and manage their lifetimes.
                using (var serviceProvider = new ServiceCollection()
                    // Add the configuration to the services collection
                    // Make sure we're logging to the console.
                    .AddLogging(l => l.AddConsole())
                    // Add an HttpClient instance. This is used for making requests to the
                    // cloud service.
                    .AddSingleton<HttpClient>()
                    .AddTransient<DeviceDetectionPipelineBuilder>()
                    // Add a factory to create the singleton IPipeline instance
                    .AddSingleton((x) =>
                        x.GetRequiredService<DeviceDetectionPipelineBuilder>()
                            .UseCloud(resourceKey)
                            .SetCloudRequestOrigin("www.51degrees-example.com")
                            .Build())
                    .AddTransient<Example>()
                    .BuildServiceProvider())
                {
                    // If we don't have a resource key then log an error.
                    if (string.IsNullOrWhiteSpace(resourceKey))
                    {
                        serviceProvider.GetRequiredService<ILogger<Program>>().LogError(
                            $"No resource key specified on the command line or the environment " +
                            $"variable '{ExampleUtils.RESOURCE_KEY_ENV_VAR}'. The 51Degrees " +
                            $"cloud service is accessed using a 'ResourceKey'. For more " +
                            $"information see " +
                            $"https://51degrees.com/documentation/_info__resource_keys.html. " +
                            $"A resource key with the properties required by this example can be " +
                            $"created for free at https://configure.51degrees.com/1QWJwHxl. " +
                            $"Once complete, pass the key as a command line argument or " +
                            $"populate the environment variable mentioned at the start of this " +
                            $"message");
                    }
                    else
                    {
                        serviceProvider.GetRequiredService<Example>().Run(output);
                    }
                }

            }
        }

        static void Main(string[] args)
        {
            // Use the command line args to get the resource key if present.
            // Otherwise, get it from the environment variable.
            string resourceKey = args.Length > 0 ? args[0] :
                Environment.GetEnvironmentVariable(
                    ExampleUtils.RESOURCE_KEY_ENV_VAR);

            Example.Run(resourceKey, Console.Out);
        }
    }
}
