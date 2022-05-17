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

using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

/// <summary>
/// @example Cloud/GettingStarted-Console/Program.cs
///
/// @include{doc} example-getting-started-cloud.txt
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/Examples/Cloud/GettingStarted-Console/Program.cs). 
/// 
/// @include{doc} example-require-resourcekey.txt
///
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// - Microsoft.Extensions.Configuration.Json
/// - Microsoft.Extensions.DependencyInjection
/// - Microsoft.Extensions.Logging.Console
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Cloud.GettingStartedConsole
{
    public class Program
    {
        public class Example
        {
            public void Run(IServiceProvider serviceProvider, TextWriter output)
            {
                var pipelineOptions = serviceProvider.GetRequiredService<PipelineOptions>();
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                // In this example, we use the FiftyOnePipelineBuilder and configure it from a file.
                // For more information about builders in general see the documentation at
                // http://51degrees.com/documentation/_concepts__configuration__builders__index.html

                // Create the pipeline using the service provider and the configured options.
                using (var pipeline = new FiftyOnePipelineBuilder(loggerFactory, serviceProvider)
                    .BuildFromConfiguration(pipelineOptions))
                {
                    // Carry out some sample detections
                    foreach (var values in EvidenceValues)
                    {
                        AnalyseEvidence(values, pipeline, output);
                    }
                }
            }

            private void AnalyseEvidence(
                Dictionary<string, object> evidence,
                IPipeline pipeline,
                TextWriter output)
            {
                // FlowData is a data structure that is used to convey information required for
                // detection and the results of the detection through the pipeline.
                // Information required for detection is called "evidence" and usually consists
                // of a number of HTTP Header field values, in this case represented by a
                // Dictionary<string, object> of header name/value entries.
                //
                // FlowData is wrapped in a using block in order to ensure that the resources
                // are freed in a timely manner.
                using (var data = pipeline.CreateFlowData())
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
                    OutputValue("Platform Name", device.PlatformName, message);
                    OutputValue("Platform Version", device.PlatformVersion, message);
                    OutputValue("Browser Name", device.BrowserName, message);
                    OutputValue("Browser Version", device.BrowserVersion, message);
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
            public void Run(PipelineOptions options, TextWriter output)
            {
                // Initialize a service collection which will be used to create the services
                // required by the Pipeline and manage their lifetimes.
                using (var serviceProvider = new ServiceCollection()
                    // Add the configuration to the services collection.
                    .AddSingleton(options)
                    // Make sure we're logging to the console.
                    .AddLogging(l => l.AddConsole())
                    // Add an HttpClient instance. This is used for making requests to the
                    // cloud service.
                    .AddSingleton<HttpClient>()
                    // Add the builders that will be needed to create the engines specified in the 
                    // configuration file.
                    .AddSingleton<CloudRequestEngineBuilder>()
                    .AddSingleton<DeviceDetectionCloudEngineBuilder>()
                    .BuildServiceProvider())
                {
                    // Get the resource key setting from the config file. 
                    var resourceKey = options.GetResourceKey();

                    // If we don't have a resource key then log an error.
                    if (string.IsNullOrWhiteSpace(resourceKey))
                    {
                        serviceProvider.GetRequiredService<ILogger<Program>>().LogError(
                            $"No resource key specified in the configuration file " +
                            $"'appsettings.json' or the environment variable " +
                            $"'{ExampleUtils.RESOURCE_KEY_ENV_VAR}'. The 51Degrees cloud " +
                            $"service is accessed using a 'ResourceKey'. For more information " +
                            $"see " +
                            $"http://51degrees.com/documentation/_info__resource_keys.html. " +
                            $"A resource key with the properties required by this example can be " +
                            $"created for free at https://configure.51degrees.com/1QWJwHxl. " +
                            $"Once complete, populate the config file or environment variable " +
                            $"mentioned at the start of this message with the key.");
                    }
                    else
                    {
                        new Example().Run(serviceProvider, output);
                    }
                }
            }
        }

        /// <summary>
        /// This collection contains the various input values that will 
        /// be passed to the device detection algorithm.
        /// </summary>
        private static readonly List<Dictionary<string, object>>
            EvidenceValues = new List<Dictionary<string, object>>()
        {
            // A User-Agent from a mobile device.
            new Dictionary<string, object>()
            {
                { "header.user-agent",
                    "Mozilla/5.0 (Linux; Android 9; SAMSUNG SM-G960U) AppleWebKit/537.36 " +
                    "(KHTML, like Gecko) SamsungBrowser/10.1 Chrome/71.0.3578.99 Mobile " +
                    "Safari/537.36" }
            },
            // A User-Agent from a desktop device.
            new Dictionary<string, object>()
            {
                { "header.user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                    "(KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36" }
            },
            // Evidence values from a windows 11 device using a browser
            // that supports User-Agent Client Hints.
            new Dictionary<string, object>()
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
            }
        };

        static void Main(string[] args)
        {
            // Use the command line args to get the resource key if present.
            // Otherwise, get it from the environment variable.
            string resourceKey = args.Length > 0 ? args[0] :
                Environment.GetEnvironmentVariable(
                    ExampleUtils.RESOURCE_KEY_ENV_VAR);

            // Load the configuration file
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Bind the configuration to a pipeline options instance
            PipelineOptions options = new PipelineOptions();
            config.Bind("PipelineOptions", options);

            // Get the resource key setting from the config file. 
            var resourceKeyFromConfig = options.GetResourceKey();
            var configHasKey = string.IsNullOrWhiteSpace(resourceKeyFromConfig) == false &&
                    resourceKeyFromConfig.StartsWith("!!") == false;

            // If no resource key is specified in the config file then override it with the key
            // from the environment variable / command line. 
            if (configHasKey == false)
            {
                options.SetResourceKey(resourceKey);
            }

            new Example().Run(options, Console.Out);
        }
    }
}
