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
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// @example Hash/GettingStarted/Program.cs
/// 
/// @include{doc} example-getting-started-hash.txt
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/FiftyOne.DeviceDetection/Examples/Framework/Hash/GettingStarted/Program.cs). 
/// 
/// @include{doc} example-require-datafile.txt
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Hash.GettingStarted
{
    public class Program
    {
        public class Example : ExampleBase
        {
            private const string mobileUserAgent =
               "Mozilla/5.0 (Linux; Android 9; SAMSUNG SM-G960U) " +
               "AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/10.1 " +
               "Chrome/71.0.3578.99 Mobile Safari/537.36";
            private const string desktopUserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";

            /// <summary>
            /// User Agent Client Hint headers for platform name and version
            /// for Windows 11.
            /// </summary>
            private static readonly KeyValuePair<string, string>[] platformUach
                = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("User-Agent", "NOT SET"),
                new KeyValuePair<string, string>(
                    "Sec-CH-UA-Platform",
                    "Windows"),
                new KeyValuePair<string, string>(
                    "Sec-CH-UA-Platform-Version",
                    "14.0.0"),
            };

            public void Run(string dataFile)
            {
                FileInfo f = new FileInfo(dataFile);
                Console.WriteLine($"Using data file at '{f.FullName}'");
                // Use the DeviceDetectionPipelineBuilder to build a new Pipeline 
                // to use an on-premise Hash engine with the low memory
                // performance profile.
                using (var pipeline = new DeviceDetectionPipelineBuilder()
                    .UseOnPremise(dataFile, null, false)
                    // Prefer low memory profile where all data streamed 
                    // from disk on-demand. Experiment with other profiles.
                    //.SetPerformanceProfile(PerformanceProfiles.HighPerformance)
                    .SetPerformanceProfile(PerformanceProfiles.LowMemory)
                    //.SetPerformanceProfile(PerformanceProfiles.Balanced)
                    .Build())
                {
                    // First try a desktop User-Agent.
                    AnalyseUserAgent(desktopUserAgent, pipeline);
                    Console.WriteLine();
                    // Now try a mobile User-Agent.
                    AnalyseUserAgent(mobileUserAgent, pipeline);
                    Console.WriteLine();
                    // Now try the User Agent Client Hints.
                    AnalyseUach(platformUach, pipeline);
                }
            }

            private void AnalyseUach(
                KeyValuePair<string, string>[] evidence, 
                IPipeline pipeline)
            {
                // Create the FlowData instance.
                using (var data = pipeline.CreateFlowData())
                {
                    // Add a User-Agent from a desktop as evidence.
                    Console.WriteLine(
                        "Get the platform name and version for;");
                    foreach (var item in evidence)
                    {
                        data.AddEvidence(
                            Pipeline.Core.Constants.EVIDENCE_HTTPHEADER_PREFIX + 
                            "." + item.Key, item.Value);
                        Console.WriteLine(item.Key + "; " + item.Value);
                    }
                    // Process the supplied evidence.
                    data.Process();
                    // Get device data from the flow data.
                    var device = data.Get<IDeviceData>();
                    // Output the value of the 'PlatformName' property.
                    if (device.PlatformName.HasValue)
                    {
                        Console.WriteLine($"\t{device.PlatformName.Value}");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"\t{device.PlatformName.NoValueMessage}");
                    }
                    // Output the value of the 'PlatformVersion' property.
                    if (device.PlatformVersion.HasValue)
                    {
                        Console.WriteLine($"\t{device.PlatformVersion.Value}");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"\t{device.PlatformVersion.NoValueMessage}");
                    }
                }
            }

            private void AnalyseUserAgent(string userAgent, IPipeline pipeline)
            {
                // Create the FlowData instance.
                using (var data = pipeline.CreateFlowData())
                {
                    // Add a User-Agent provided as evidence.
                    data.AddEvidence(
                        Pipeline.Core.Constants.EVIDENCE_QUERY_USERAGENT_KEY, 
                        userAgent);
                    // Process the supplied evidence.
                    data.Process();
                    // Get device data from the flow data.
                    var device = data.Get<IDeviceData>();
                    var isMobile = device.IsMobile;
                    Console.WriteLine($"Does the User-Agent '{userAgent}' " +
                        $"represent a mobile device?");
                    // Output the value of the 'IsMobile' property.
                    if (isMobile.HasValue)
                    {
                        Console.WriteLine($"\t{isMobile.Value}");
                    }
                    else
                    {
                        Console.WriteLine($"\t{isMobile.NoValueMessage}");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
#if NETCORE
            var defaultDataFile = "..\\..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV4.1.hash";
#else
            var defaultDataFile = "..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV4.1.hash";
#endif
            var dataFile = args.Length > 0 ? args[0] : defaultDataFile;
            new Example().Run(dataFile);
#if (DEBUG)
            Console.WriteLine("Complete. Press key to exit.");
            Console.ReadKey();
#endif
        }
    }
}