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
/// @example Hash/UserAgentClientHints/Program.cs
/// 
/// @include{doc} example-user-agent-client-hints.txt
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/FiftyOne.DeviceDetection/Examples/Framework/Hash/UserAgentClientHints/Program.cs). 
/// 
/// @include{doc} example-require-datafile.txt
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// 
/// Expected output:
///
/// ---------------------------------------
/// This example demonstrates detection using user-agent client hints.
/// The sec-ch-ua value can be used to determine the browser of the connecting device, but not other components such as the hardware.
/// We show this by first performing detection with sec-ch-ua only.
/// We then repeat with the user-agent header only.
/// Finally, we use both sec-ch-ua and user-agent.Note that sec-ch-ua takes priority over the user-agent for detection of the browser.
/// ---------------------------------------
/// Sec-CH-UA = '"Google Chrome";v="89", "Chromium";v="89", ";Not A Brand";v="99"'
/// User-Agent = 'NOT_SET'
///         Browser = Chrome 89
///         IsMobile = No matching profiles could be found for the supplied evidence.A 'best guess' can be returned by configuring more lenient matching rules.See https://51degrees.com/documentation/_device_detection__features__false_positive_control.html
///
/// Sec-CH-UA = 'NOT_SET'
/// User-Agent = 'Mozilla/5.0 (Linux; Android 9; SAMSUNG SM-G960U) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/10.1 Chrome/71.0.3578.99 Mobile Safari/537.36'
///         Browser = Samsung Browser 10.1
///         IsMobile = True
///
/// Sec-CH-UA = '"Google Chrome";v="89", "Chromium";v="89", ";Not A Brand";v="99"'
/// User-Agent = 'Mozilla/5.0 (Linux; Android 9; SAMSUNG SM-G960U) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/10.1 Chrome/71.0.3578.99 Mobile Safari/537.36'
///         Browser = Chrome 89
///         IsMobile = True
/// Complete.Press key to exit.
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Hash.UserAgentClientHints
{
    public class Program
    {
        public class Example : ExampleBase
        {
            private static string mobileUserAgent =
               "Mozilla/5.0 (Linux; Android 9; SAMSUNG SM-G960U) " +
               "AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/10.1 " +
               "Chrome/71.0.3578.99 Mobile Safari/537.36";
            private static string secchuaValue =
                "\"Google Chrome\";v=\"89\", \"Chromium\";v=\"89\", \";Not A Brand\";v=\"99\"";

            public void Run(string dataFile)
            {
                FileInfo f = new FileInfo(dataFile);
                Console.WriteLine($"Using data file at '{f.FullName}'");
                Console.WriteLine($"---------------------------------------");
                Console.WriteLine($"This example demonstrates detection " +
                    $"using user-agent client hints.");
                Console.WriteLine($"The sec-ch-ua value can be used to " +
                    $"determine the browser of the connecting device, " +
                    $"but not other components such as the hardware.");
                Console.WriteLine($"We show this by first performing " +
                    $"detection with sec-ch-ua only.");
                Console.WriteLine($"We then repeat with the user-agent " +
                    $"header only.");
                Console.WriteLine($"Finally, we use both sec-ch-ua and " +
                    "user-agent. Note that sec-ch-ua takes priority " +
                    "over the user-agent for detection of the browser.");
                Console.WriteLine($"---------------------------------------");
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
                    // first try with just sec-ch-ua.
                    AnalyseClientHints(pipeline, false, true);
                    Console.WriteLine();
                    // Now with just user-agent.
                    AnalyseClientHints(pipeline, true, false);
                    Console.WriteLine();
                    // Finally, perform detection with both.
                    AnalyseClientHints(pipeline, true, true);
                }
            }

            private void AnalyseClientHints(IPipeline pipeline, bool setUserAgent, bool setSecChUa)
            {
                // Create the FlowData instance.
                using (var data = pipeline.CreateFlowData())
                {
                    // Add a value for the user-agent client hints header
                    // sec-ch-ua as evidence
                    if (setSecChUa)
                    {
                        data.AddEvidence(Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
                            Pipeline.Core.Constants.EVIDENCE_SEPERATOR + "sec-ch-ua", secchuaValue);
                    }
                    // Also add a standard user-agent if requested
                    if (setUserAgent)
                    {
                        data.AddEvidence(Pipeline.Core.Constants.EVIDENCE_QUERY_USERAGENT_KEY,
                            mobileUserAgent);
                    }

                    // Process the supplied evidence.
                    data.Process();
                    // Get device data from the flow data.
                    var device = data.Get<IDeviceData>();

                    var browserName = device.BrowserName;
                    var browserVersion = device.BrowserVersion;
                    var isMobile = device.IsMobile;

                    var secchua = setSecChUa ? secchuaValue : "NOT_SET";
                    Console.WriteLine($"Sec-CH-UA = '{secchua}'");
                    var ua = setUserAgent ? mobileUserAgent : "NOT_SET";
                    Console.WriteLine($"User-Agent = '{ua}'");

                    // Output the Browser.
                    if (browserName.HasValue && browserVersion.HasValue)
                    {
                        Console.WriteLine($"\tBrowser = {browserName.Value} {browserVersion.Value}");
                    }
                    else if (browserName.HasValue)
                    {
                        Console.WriteLine($"\tBrowser = {browserName.Value} (version unknown)");
                    }
                    else
                    {
                        Console.WriteLine($"\tBrowser = {browserName.NoValueMessage}");
                    }
                    // Output the value of the 'IsMobile' property.
                    if (isMobile.HasValue)
                    {
                        Console.WriteLine($"\tIsMobile = {isMobile.Value}");
                    }
                    else
                    {
                        Console.WriteLine($"\tIsMobile = {isMobile.NoValueMessage}");
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