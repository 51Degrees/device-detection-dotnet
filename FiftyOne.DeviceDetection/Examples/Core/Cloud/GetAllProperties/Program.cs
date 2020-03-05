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
using FiftyOne.Pipeline.Engines.Data;
using System;
using System.Collections.Generic;
/// <summary>
/// @example Cloud/GetAllProperties/Program.cs
/// </summary>
namespace GetAllProperties
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
            string resourceKey = "!!YOUR_RESOURCE_KEY!!";

            if (resourceKey.StartsWith("!!"))
            {
                Console.WriteLine("You need to create a resource key at " +
                    "https://configure.51degrees.com and paste it into this example.");
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
            // Add a User-Agent as evidence.
            data.AddEvidence(FiftyOne.Pipeline.Core.Constants.EVIDENCE_QUERY_USERAGENT_KEY, userAgent);
            // Process the supplied evidence.
            data.Process();
            // Get device data from the flow data.
            var device = data.Get<IDeviceData>();
            Console.WriteLine($"What property values are associated with " +
                $"the User-Agent '{userAgent}'?");

            // Iterate through device data results, displaying all values.
            foreach (var entry in device.AsDictionary())
            {
                Console.Write(entry.Key ?? "NULL");
                Console.Write(" = ");
                try
                {
                    bool valueWritten = false;
                    if (entry.Value != null)
                    {
                        if (typeof(IReadOnlyList<string>).IsAssignableFrom(entry.Value.GetType()))
                        {
                            var list = entry.Value as IReadOnlyList<string>;
                            Console.WriteLine(string.Join(", ", list));
                            valueWritten = true;
                        }

                        if (valueWritten == false)
                        {
                            // Truncate any long strings to 200 characters
                            var str = entry.Value.ToString();
                            if(str.Length > 200)
                            {
                                str = str.Remove(200);
                                str += "...";
                            }
                            Console.WriteLine(str);
                        }
                    }
                    else
                    {
                        Console.WriteLine("NULL");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
            }
        }
    }
}
