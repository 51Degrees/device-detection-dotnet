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

using FiftyOne.DeviceDetection;
using FiftyOne.Pipeline.Core.FlowElements;
using System;
/// <summary>
/// @example Cloud/GettingStarted/Program.cs
///
/// Getting started example of using 51Degrees device detection.
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/release/v4.1.0/FiftyOne.DeviceDetection/Examples/Core/Cloud/GettingStarted/Program.cs). 
/// (During the beta period, this repository will be private. 
/// [Contact us](mailto:support.51degrees.com) to request access) 
/// 
/// To run this example, you will need to create a **resource key**. 
/// The resource key is used as short-hand to store the particular set of 
/// properties you are interested in as well as any associated license keys 
/// that entitle you to increased request limits and/or paid-for properties.
/// 
/// You can create a resource key using the 51Degrees [Configurator](https://configure.51degrees.com).
///
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// 
/// The example shows how to:
///
/// 1. Build a new Pipeline with a cloud-based device detection engine.
/// ```
/// var pipeline = new DeviceDetectionPipelineBuilder()
///     .UseCloud(resourceKey)
///     .Build();
/// ```
///
/// 2. Create a new FlowData instance ready to be populated with evidence for the
/// Pipeline.
/// ```
/// var data = pipeline.CreateFlowData();
/// ```
///
/// 3. Add a User-Agent string to the evidence collection and process it.
/// ```
/// data.AddEvidence("header.user-agent", userAgent)
///     .Process();
/// ```
///
/// 4. Extract the value of a property as a string from the results.
/// ```
/// Console.WriteLine("IsMobile: " + data.Get<IDeviceData>().IsMobile.Value);
/// ```
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Cloud.GettingStarted
{
    class Program
    {
        private static string mobileUserAgent =
            "Mozilla/5.0 (Linux; Android 9; SAMSUNG SM-G960U) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/10.1 " +
            "Chrome/71.0.3578.99 Mobile Safari/537.36";
        private static string desktopUserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";

        static void Main(string[] args)
        {
            // Obtain a resource key for free at https://configure.51degrees.com
            // Make sure to include the 'IsMobile' property as it is used by this example.
            string resourceKey = "!!YOUR_RESOURCE_KEY!!";

            if (resourceKey.StartsWith("!!"))
            {
                Console.WriteLine("You need to create a resource key at " +
                    "https://configure.51degrees.com and paste it into this example.");
                Console.WriteLine("Make sure to include the 'IsMobile' " +
                    "property as it is used by this example.");
            }
            else
            {
                // Create the pipeline
                using (var pipeline = new DeviceDetectionPipelineBuilder()
                    // Tell it that we want to use cloud and pass our resource key.
                    .UseCloud(resourceKey)
                    .Build())
                {
                    // First try a desktop User-Agent.
                    AnalyseUserAgent(desktopUserAgent, pipeline);
                    // Now try a mobile User-Agent.
                    AnalyseUserAgent(mobileUserAgent, pipeline);
                }
            }
#if (DEBUG)
            Console.WriteLine("Done. Press any key to exit.");
            Console.ReadKey();
#endif
        }

            static void AnalyseUserAgent(string userAgent, IPipeline pipeline)
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
}
