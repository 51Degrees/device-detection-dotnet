/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
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

using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;

/// <summary>
/// @example Hash/ConfigureFromFile/Program.cs
/// 
/// @include{doc} example-configure-from-file-hash.txt
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/FiftyOne.DeviceDetection/Examples/Framework/Hash/ConfigureFromFile/Program.cs). 
/// 
/// @include{doc} example-require-datafile.txt
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// - Microsoft.Extensions.Configuration.Json OR Microsoft.Extensions.Configuration.Xml
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Hash.ConfigureFromFile
{
    public class Program
    {
        public class Example : ExampleBase
        {
            private static string mobileUserAgent =
                "Mozilla/5.0 (Linux; Android 9; SAMSUNG SM-G960U) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/10.1 " +
                "Chrome/71.0.3578.99 Mobile Safari/537.36";
            private static string desktopUserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";

            public void Run()
            {
                Console.WriteLine($"Constructing pipeline from configuration file.");

                // Load the assembly with the Hash engine.
                // This is required because nothing else in the example 
                // references any types from this assembly and it will
                // need to be loaded into the app domain in order for 
                // the PipelineBuilder to find the engine builder to use 
                // when creating the device detection engine.
                // (i.e. the 'DeviceDetectionHashEngineBuilder'
                // specified in appsettings.json)
                Assembly.Load("FiftyOne.DeviceDetection.Hash.Engine.OnPremise");

                // Create the configuration object
                var config = new ConfigurationBuilder()
#if NETCORE
                    .AddJsonFile("appsettings.json")
#else
                    .AddXmlFile("App.config")
#endif
                    .Build();
                // Bind the configuration to a pipeline options instance
                PipelineOptions options = new PipelineOptions();
                config.Bind("PipelineOptions", options);

                // Create the pipeline using the options object
                using (var pipeline = new FiftyOnePipelineBuilder()
                    .BuildFromConfiguration(options))
                {
                    // First try a desktop User-Agent.
                    AnalyseUserAgent(desktopUserAgent, pipeline);
                    Console.WriteLine();
                    // Now try a mobile User-Agent.
                    AnalyseUserAgent(mobileUserAgent, pipeline);
                }
            }

            /// <summary>
            /// Process a single HTTP User-Agent string to retrieve the values associated
            /// with the User-Agent for the selected properties.
            /// </summary>
            /// <param name="userAgent"></param>
            /// <param name="pipeline"></param>
            private void AnalyseUserAgent(string userAgent, IPipeline pipeline)
            {
                // Create the FlowData instance.
                var data = pipeline.CreateFlowData();
                // Add a User-Agent from a desktop as evidence.
                data.AddEvidence(FiftyOne.Pipeline.Core.Constants.EVIDENCE_QUERY_USERAGENT_KEY, userAgent);
                // Process the supplied evidence.
                data.Process();
                // Get device data from the flow data.
                var device = data.Get<IDeviceData>();
                Console.WriteLine($"Does the User-Agent '{userAgent}' " +
                    $"represent a mobile device?");
                // Output the result of the 'IsMobile' property.
                if (device.IsMobile.HasValue)
                {
                    Console.WriteLine($"\t{device.IsMobile.Value}");
                }
                else
                {
                    Console.WriteLine($"\t{device.IsMobile.NoValueMessage}");
                }
            }
        }

        static void Main(string[] args)
        {
            new Example().Run();
#if (DEBUG)
            Console.WriteLine("Complete. Press key to exit.");
            Console.ReadKey();
#endif
        }
    }
}