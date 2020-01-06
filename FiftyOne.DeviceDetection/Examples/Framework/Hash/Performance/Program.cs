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
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// @example Hash/Performance/Program.cs
///
/// Performance example of using 51Degrees device detection.
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/release/v4.1.0/FiftyOne.DeviceDetection/Examples/Framework/Hash/Performance/Program.cs). 
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
/// The example shows how to:
///
/// 1. Build a new on-premise Hash engine with the maximum performance profile.
/// ```
/// var engine = new DeviceDetectionHashEngineBuilder()
///     .SetAutoUpdate(false)
///     .SetPerformanceProfile(PerformanceProfiles.MaxPerformance)
///     .Build("51Degrees-LiteV3.4.trie", false);
/// ```
///
/// 2. Start multiple threads to process a set of User-Agents, making a note of
/// the time at which processing was started.
/// ```
/// var starts = DateTime.UtcNow;
///
/// Parallel.ForEach(Report(GetUserAgents(uaFile, count).ToList(), count, maxDistinctUAs, 40),
///     userAgent =>
/// {
/// ...
/// }
/// ```
///
/// 3. Wait for all processing to finish, and make a note of the time elapsed
/// since the processing was started.
/// ```
/// var time = DateTime.UtcNow - starts;
/// ```
///
/// 4. Output the average time to process a single User-Agent.
/// ```
/// Console.WriteLine($"Average {(double)time.TotalMilliseconds / (double)count} ms per User-Agent");
/// ```
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Hash.Performance
{
    public class Program
    {
        public class Example : ExampleBase
        {
            public void Run(string dataFile, string uaFile, int count)
            {
                FileInfo f = new FileInfo(dataFile);
                Console.WriteLine($"Using data file at '{f.FullName}'");
                // Create a simple pipeline to access the engine with.
                using (var pipeline = new DeviceDetectionPipelineBuilder()
                    .UseOnPremise(dataFile, false)
                    .SetAutoUpdate(false)
                    .SetDataFileSystemWatcher(false)
                    .SetDataUpdateOnStartUp(false)
                    .SetShareUsage(false)
                    // Prefer maximum performance profile where all data loaded 
                    // into memory. Experiment with other profiles.
                    .SetPerformanceProfile(PerformanceProfiles.MaxPerformance)
                    //.SetPerformanceProfile(PerformanceProfiles.LowMemory)
                    //.SetPerformanceProfile(PerformanceProfiles.Balanced)
                    //.UseResultsCache()
                    .Build())
                {
                    Run(uaFile, count, pipeline);
                }
            }

            private static void Run(
                string uaFile, 
                int count,
                IPipeline pipeline)
            {
                var isMobileTrue = 0;
                var isMobileFalse = 0;
                var isMobileUnknown = 0;
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                int maxDistinctUAs = 1000;

                var starts = DateTime.UtcNow;

                try
                {
                    Console.WriteLine($"Processing {count} User-Agents from {uaFile}");
                    Console.WriteLine($"The {count} process calls will use a " +
                        $"maximum of {maxDistinctUAs} distinct User-Agents");
                    Parallel.ForEach(Report(GetUserAgents(uaFile, count).ToList(), count, maxDistinctUAs, 40),
                        new ParallelOptions()
                        {
                            //MaxDegreeOfParallelism = 1,
                            CancellationToken = cancellationTokenSource.Token
                        },
                        userAgent =>
                    {
                        // Create a new flow data to add evidence to and get 
                        // device data back again.
                        var data = pipeline.CreateFlowData();
                        // Add the User-Agent as evidence to the flow data.
                        data.AddEvidence("header.User-Agent", userAgent)
                                    .Process();

                        // Get the device from the engine.
                        var device = data.Get<IDeviceData>();

                        // Update the counters depending on the IsMobile
                        // result.
                        var isMobile = device.IsMobile;
                        if (isMobile.HasValue)
                        {
                            if (isMobile.Value)
                            {
                                Interlocked.Increment(ref isMobileTrue);
                            }
                            else
                            {
                                Interlocked.Increment(ref isMobileFalse);
                            }
                        }
                        else
                        {
                            Interlocked.Increment(ref isMobileUnknown);
                        }
                        
                    });
                    var time = DateTime.UtcNow - starts;
                    Console.WriteLine($"Average {(double)time.TotalMilliseconds / (double)count} ms per User-Agent");
                    Console.WriteLine($"IsMobile = True  : {isMobileTrue}");
                    Console.WriteLine($"IsMobile = False : {isMobileFalse}");
                    Console.WriteLine($"IsMobile = Unknown : {isMobileUnknown}");
                }
                catch (OperationCanceledException) { }
            }
        }

        static void Main(string[] args)
        {
#if NETCORE
            var defaultDataFile = "..\\..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV3.4.trie";
            var defaultUaFile = "..\\..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\20000 User Agents.csv";
#else
            var defaultDataFile = "..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV3.4.trie";
            var defaultUaFile = "..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\20000 User Agents.csv";
#endif
            var dataFile = args.Length > 0 ? args[0] : defaultDataFile;
            var uaFile = args.Length > 1 ? args[1] : defaultUaFile;
            new Example().Run(dataFile, uaFile, 10000);
#if (DEBUG)
            Console.WriteLine("Complete. Press key to exit.");
            Console.ReadKey();
#endif
        }
    }
}