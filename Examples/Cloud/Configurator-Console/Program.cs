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
                    { "header.user-agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                        "(KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36" },
                    { "header.sec-ch-ua-mobile", "?0" },
                    { "header.sec-ch-ua",
                        "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"98\", " +
                        "\"Google Chrome\";v=\"98\"" },
                    { "header.sec-ch-ua-platform", "\"Windows\"" },
                    { "header.sec-ch-ua-platform-version", "\"14.0.0\"" }
                };

                // FlowData is a data structure that is used to convey information required for
                // detection and the results of the detection through the pipeline.
                // Information required for detection is called "evidence" and usually consists
                // of a number of HTTP Header field values, in this case represented by a
                // Dictionary<string, object> of header name/value entries.
                //
                // FlowData is wrapped in a using block in order to ensure that the resources
                // are freed in a timely manner.
                using (var data = _pipeline.CreateFlowData())
                {
                    StringBuilder message = new StringBuilder();

                    // list the evidence
                    message.AppendLine("Input values:");
                    foreach (var entry in evidence)
                    {
                        message.AppendLine($"\t{entry.Key}: {entry.Value}");
                    }
                    output.WriteLine(message.ToString());

                    // Add the evidence values to the flow data
                    data.AddEvidence(evidence);

                    // Process the flow data.
                    data.Process();

                    message = new StringBuilder();
                    message.AppendLine("Results:");
                    // Now that it's been processed, the flow data will have been populated with
                    // the result. In this case, we want information about the device, which we
                    // can get by asking for a result matching the `IDeviceData` interface.
                    var device = data.Get<IDeviceData>();

                    // Display the results of the detection, which are called device properties.
                    // See the property dictionary at
                    // https://51degrees.com/developers/property-dictionary
                    // for details of all available properties.
                    OutputValue("Mobile Device", device.IsMobile, message);
                    output.WriteLine(message.ToString());
                }
            }

            private void OutputValue(string name,
                IAspectPropertyValue value,
                StringBuilder message)
            {
                // Individual result values have a wrapper called `AspectPropertyValue`.
                // This functions similarly to a null-able type. If the value has not been set
                // then trying to access the `Value` property will throw an exception.
                // `AspectPropertyValue` also includes the `NoValueMessage` property, which
                // describes why the value has not been set.
                message.AppendLine(value.HasValue ?
                    $"\t{name}: " + value.Value :
                    $"\t{name}: " + value.NoValueMessage);
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
