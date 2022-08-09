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

using FiftyOne.DeviceDetection;
using FiftyOne.DeviceDetection.Apple;
using FiftyOne.DeviceDetection.Examples;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Constants = FiftyOne.DeviceDetection.Examples.Constants;

/// <summary>
/// @example OnPremise/AppleServerSideConfig-Console/Program.cs
/// 
/// Detection of the specific models of Apple devices being used to make a request is 
/// [more difficult](http://51degrees.com/documentation/4.4/_device_detection__features__apple_detection.html) 
/// than it is for other vendors.
/// 
/// Typically, the 51Degrees approach is to use dynamically generated JavaScript to determine the
/// model on the client-side and pass this back to the server. Some users are unable or unwilling 
/// to deploy dynamic JavaScript. In this case 51Degrees have an alternative approach in which 
/// the required data is gathered using static JavaScript. This is then passed to the server to
/// determine the Apple model.
/// 
/// This example demonstrates this alternative approach. 
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/Examples/OnPremise/AppleServerSide-Console/Program.cs). 
/// 
/// This example requires a paid-for data file as the device model properties are not available
/// in the free, 'lite' data file.
/// See our [pricing page](http://51degrees.com/pricing) for details on how to obtain one. 
/// 
/// This variation shows how to create the pipeline from a configuration file: 
/// @include OnPremise/AppleServerSideConfig-Console/appsettings.json
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// - Microsoft.Extensions.DependencyInjection
/// - Microsoft.Extensions.Logging.Console
/// - Microsoft.Extensions.Configuration.Json
/// </summary>
namespace AppleServerSide_Console
{
    class Program
    {
        public class Example : AppleExampleBase
        {
            /// <summary>
            /// The pipeline instance that will be used for device detection
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


            public void Run(TextWriter output)
            {
                // Set the evidence values that we're going to pass in for this example.
                // In the real world, you will need to use JavaScript to get the values and pass
                // them back to the server to be added as evidence.
                // The script at https://cloud.51degrees.com/cdn/apple-functions.js contains the
                // raw functions that are used to obtain these values.
                var data = new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>() 
                    {
                        { FiftyOne.Pipeline.Core.Constants.EVIDENCE_HEADER_USERAGENT_KEY,
                            "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.0 Mobile/15E148 Safari/604.1" },
                        { FiftyOne.DeviceDetection.Apple.Constants.EVIDENCE_SCREEN_HEIGHT_KEY, 1792 },
                        { FiftyOne.DeviceDetection.Apple.Constants.EVIDENCE_SCREEN_WIDTH_KEY, 828 },
                        { FiftyOne.DeviceDetection.Apple.Constants.EVIDENCE_SCREEN_RATIO_KEY, 2 },
                        { FiftyOne.DeviceDetection.Apple.Constants.EVIDENCE_SCREEN_GAMUT_KEY, "p3" },
                        { FiftyOne.DeviceDetection.Apple.Constants.EVIDENCE_HASH3D_KEY, 3335845976 },
                        { FiftyOne.DeviceDetection.Apple.Constants.EVIDENCE_HASH_KEY, 2206992415 },
                        { FiftyOne.DeviceDetection.Apple.Constants.EVIDENCE_RENDERER_KEY, "Apple GPU" },
                        { FiftyOne.DeviceDetection.Apple.Constants.EVIDENCE_FAMILY_KEY, "iPhone" }
                    }
                };

                // carry out some sample detections
                foreach (var values in data)
                {
                    AnalyseEvidence(values, output);
                }
            }

            private void AnalyseEvidence(
                Dictionary<string, object> evidence,
                TextWriter output)
            {
                // FlowData is a data structure that is used to convey information required for
                // detection and the results of the detection through the pipeline.
                // Information required for detection is called "evidence" and usually consists
                // of a number of HTTP Header field values, in this case represented by a
                // Dictionary<string, object> of header name/value entries.
                //
                // FlowData is wrapped in a using block in order to ensure that the unmanaged
                // resources allocated by the native device detection library are freed
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
                    var device = data.Get<IDeviceData>();

                    // Display the model name(s) of the devices matching the supplied evidence.
                    OutputValue("Model Name(s)", device.HardwareName, message);
                    output.WriteLine(message.ToString());
                }
            }

            private void OutputValue(string name,
                IAspectPropertyValue<IReadOnlyList<string>> value,
                StringBuilder message)
            {
                message.AppendLine(value.HasValue ?
                    $"\t{name}: " + string.Join(", ", value.Value) :
                    $"\t{name}: " + value.NoValueMessage);
            }

            /// <summary>
            /// This Run method is called by the example test to avoid the need to duplicate the 
            /// service provider setup logic.
            /// </summary>
            /// <param name="options"></param>
            public static void Run(PipelineOptions options, TextWriter output)
            {
                // Initialize a service collection which will be used to create the services
                // required by the Pipeline and manage their lifetimes.
                using (var serviceProvider = new ServiceCollection()
                    // Add the configuration to the services collection
                    // Make sure we're logging to the console.
                    .AddLogging(l => l.AddConsole())
                    // Add the configuration to the services collection.
                    // Add an HttpClient instance. This is used for making requests to the
                    // data update end point.
                    .AddSingleton<HttpClient>()
                    // We want to use the standard data update service
                    .AddSingleton<IDataUpdateService, DataUpdateService>()
                    .AddSingleton(options)
                    .AddTransient<AppleProfileEngineBuilder>()
                    .AddTransient<DeviceDetectionHashEngineBuilder>()
                    .AddTransient<PipelineBuilder>()
                    // Add a factory to create the singleton IPipeline instance
                    .AddSingleton((x) =>
                    {
                        // Build the pipeline
                        return x.GetRequiredService<PipelineBuilder>()
                            .BuildFromConfiguration(x.GetRequiredService<PipelineOptions>());
                    })
                    .AddTransient<Example>()
                    .BuildServiceProvider())
                {
                    var engine = serviceProvider.GetRequiredService<IPipeline>()
                        .GetElement<DeviceDetectionHashEngine>();

                    // If we've passed all the checks then run the example.
                    if (CheckDataFiles(
                        serviceProvider.GetRequiredService<ILogger<Program>>(),
                        engine,
                        options.GetHashDataFile(),
                        options.GetAppleDataFile()))
                    {
                        serviceProvider.GetRequiredService<Example>().Run(output);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            // Load the configuration file
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Bind the configuration to a pipeline options instance
            PipelineOptions options = new PipelineOptions();
            config.Bind("PipelineOptions", options);

            // Get the hash data file setting from the config file. 
            var hashFileFromConfig = options.GetHashDataFile();
            // Make sure we've got a complete path to the data file, then update the configuration.
            options.SetHashDataFile(GetCompleteFilename(hashFileFromConfig, 
                Constants.ENTERPRISE_HASH_DATA_FILE_NAME));

            // Get the Apple data file setting from the config file. 
            var appleFileFromConfig = options.GetAppleDataFile();
            // Make sure we've got a complete path to the data file, then update the configuration.
            options.SetAppleDataFile(GetCompleteFilename(appleFileFromConfig,
                Constants.APPLE_DATA_FILE_NAME));

            Example.Run(options, Console.Out);
        }

        private static string GetCompleteFilename(string filenameFromConfig, string defaultName)
        {
            // If no data file is specified in the config file, or we don't have a complete path,
            // then file the complete path and use it to override the one in the config file.
            if (string.IsNullOrWhiteSpace(filenameFromConfig))
            {
                return ExampleUtils.FindFile(defaultName);
            }
            else if (Path.IsPathRooted(filenameFromConfig) == false)
            {
                return ExampleUtils.FindFile(filenameFromConfig);
            }

            return filenameFromConfig;
        }
    }
}
