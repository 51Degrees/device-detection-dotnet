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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Engines;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// @example Hash/Metadata/Program.cs
///
/// Metadata example of using 51Degrees device detection.
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/FiftyOne.DeviceDetection/Examples/Framework/Hash/Metadata/Program.cs). 
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
/// 1. Build a new on-premise Hash engine with the low memory performance profile.
/// ```
/// var engine = new DeviceDetectionHashEngineBuilder(loggerFactory)
///     .SetAutoUpdate(false)
///     .SetPerformanceProfile(PerformanceProfiles.LowMemory)
///     .Build("51Degrees-LiteV4.1.hash", false);
/// ```
/// 
/// 2. Iterate over all properties in the data file, printing the name, value type,
/// and description for each one.
/// ```
/// foreach (var property in engine.Properties)
/// {
///     Console.WriteLine($"{property.Name}[Category: {property.Category}]" +
///         $"({property.Type.Name}) - {property.Description}");
/// }
/// ```
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Hash.Metadata
{
    public class Program
    {
        public class Example : ExampleBase
        {
            // truncate value if it contains newline (esp for the JavaScript property)
            private string TruncateToNl(string s)
            {
                var lines = s.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var result = lines.FirstOrDefault();
                if (lines.Length > 1)
                {
                    result += " ...";
                }
                return result;
            }

            public void Run(string dataFile)
            {
                FileInfo f = new FileInfo(dataFile);
                Console.WriteLine($"Using data file at '{f.FullName}'");
                Console.WriteLine($"This example will use the meta-data exposed " +
                    $"by the device detection engine to display the names and " +
                    $"details of all the properties it can populate.");
                Console.WriteLine($"Press any key to continue.");
                Console.ReadKey();

                using (var ddEngine = new DeviceDetectionHashEngineBuilder(
                    new LoggerFactory())
                    .SetAutoUpdate(false)
                    .SetDataFileSystemWatcher(false)
                    .SetDataUpdateOnStartup(false)
                    // Prefer low memory profile where all data streamed 
                    // from disk on-demand. Experiment with other profiles.
                    //.SetPerformanceProfile(PerformanceProfiles.HighPerformance)
                    .SetPerformanceProfile(PerformanceProfiles.LowMemory)
                    //.SetPerformanceProfile(PerformanceProfiles.Balanced)

                    // Finally build the engine.
                    .Build(dataFile, false))
                {
                    foreach (var property in ddEngine.Properties)
                    {
                        // Output some details about the property.
                        // Add some formatting to highlight property names.
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.Write(property.Name);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine($"[Category: {property.Category}]" +
                            $"({property.Type.Name}) - {property.Description}");

                        // Next, output a list of the possible values this 
                        // property can have.
                        // Most properties in the Device Metrics category do
                        // not have defined values so exclude them.
                        if (property.Category != "Device Metrics")
                        {
                            StringBuilder values = new StringBuilder("Possible values: ");
                            foreach (var value in property.Values.Take(20))
                            {
                                // add value
                                values.Append(TruncateToNl(value.Name));
                                // add description if exists
                                if (string.IsNullOrEmpty(value.Description) == false)
                                {
                                    values.Append($"({value.Description})");
                                }
                                values.Append(",");
                            }
                            if (property.Values.Count() > 20)
                            {
                                values.Append($" + {property.Values.Count() - 20} more ...");
                            }
                            Console.WriteLine(values);
                        }
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