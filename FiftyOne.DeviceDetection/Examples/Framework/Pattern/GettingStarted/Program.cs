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
using System.IO;

/// <summary>
/// @example Pattern/GettingStarted/Program.cs
/// 
/// Getting started example of using 51Degrees device detection.
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/release/v4.1.0/FiftyOne.DeviceDetection/Examples/Framework/Pattern/GettingStarted/Program.cs). 
/// (During the beta period, this repository will be private. 
/// [Contact us](mailto:support.51degrees.com) to request access)
/// 
/// This example requires a local data file. If you don't already have one, 
/// you can obtain one from the 
/// [device-detection-data](https://github.com/51Degrees/device-detection-data) 
/// GitHub repository.
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// 
/// 1. Build a new Pipeline to use an on-premise Pattern engine with the low memory
/// performance profile.
/// ```
/// var pipeline = new DeviceDetectionPipelineBuilder()
///     .UseOnPremise("51Degrees-LiteV3.2.dat", false)
///     .SetAutoUpdate(false)
///     .SetDataFileSystemWatcher(false)
///     .SetDataUpdateOnStartUp(false)
///     .SetPerformanceProfile(PerformanceProfiles.LowMemory)
///     .Build();
/// ```
///
/// 2. Create a new FlowData instance ready to be populated with evidence for the
/// Pipeline.
/// ```
/// var data = pipeline.CreateFlowData();
/// ```
///
/// 3. Process a single HTTP User-Agent string to retrieve the values associated
/// with the User-Agent for the selected properties.
/// ```
/// data.AddEvidence("header.user-agent", mobileUserAgent)
///     .Process();
/// ```
///
/// 4. Extract the value of a property as a string from the results.
/// ```
/// var isMobile = data.Get<IDeviceData>().IsMobile;
/// if (isMobile.HasValue)
/// {
///     Console.WriteLine("IsMobile: " + isMobile.Value);
/// }
/// ```
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Hash.GettingStarted
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

            public void Run(string dataFile)
            {
                FileInfo f = new FileInfo(dataFile);
                Console.WriteLine($"Using data file at '{f.FullName}'");
                // Create a simple pipeline to access the engine with.
                using (var pipeline = new DeviceDetectionPipelineBuilder()
                    .UseOnPremise(dataFile, false)
                    .SetAutoUpdate(false)
                    .SetDataFileSystemWatcher(false)
                    .SetDataUpdateOnStartUp(false)
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
                }
            }

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

        static void Main(string[] args)
        {
#if NETCORE
            var defaultDataFile = "..\\..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV3.2.dat";
#else
            var defaultDataFile = "..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV3.2.dat";
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