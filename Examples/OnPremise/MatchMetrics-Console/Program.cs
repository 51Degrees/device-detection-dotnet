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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// @example OnPremise/MatchMetrics-Console/Program.cs
///
/// The example illustrates the various metrics that can be obtained about the device detection
/// process, for example, the degree of certainty about the result. Running the example outputs
/// those properties and values..
/// 
/// The example also illustrates controlling properties that are returned from the detection
/// process - reducing the number of components required to return the properties requested reduces
/// the overall time taken.
/// 
/// There is a [discussion](https://51degrees.com/documentation/_device_detection__hash.html#DeviceDetection_Hash_DataSetProduction_Performance)
/// of metrics and controlling performance on our web site. See also the 
/// [performance options](https://51degrees.com/documentation/_device_detection__features__performance_options.html)
/// page.
/// 
/// This example is available in full on 
/// [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/Examples/OnPremise/MatchMetrics-Console/Program.cs).
/// </summary>

namespace FiftyOne.DeviceDetection.Examples.OnPremise.MatchMetrics
{
    public class Program
    {
        public class Example : ExampleBase
        {
            /// <summary>
            /// Builder used to create a Pipeline instance
            /// </summary>
            public DeviceDetectionPipelineBuilder PipelineBuilder { get; private set; }

            public ILogger<Example> Logger { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pipelineBuilder">
            /// A builder used to create a pipeline that includes device detection.
            /// </param>
            /// <param name="logger"></param>
            public Example(DeviceDetectionPipelineBuilder pipelineBuilder,
                ILogger<Example> logger)
            {
                PipelineBuilder = pipelineBuilder;
                Logger = logger;
            }

            /// <summary>
            /// Run the example
            /// </summary>
            /// <param name="dataFile">
            /// Path to a data file to use in this example. May be absolute or relative.
            /// If relative, the example will search for a matching file in the project 
            /// directories. 
            /// </param>
            /// <param name="evidenceList">
            /// The values to perform device detection against
            /// </param>
            /// <param name="output">
            /// The writer to use when outputting messages
            /// </param>
            public void Run(string dataFile, 
                IEnumerable<Dictionary<string, object>> evidenceList, TextWriter output)
            {
                dataFile = CheckDataFile(dataFile);
                Logger.LogInformation($"Constructing pipeline from file {dataFile}");

                // Build the device detection pipeline  and pass in the desired settings to configure
                // automatic updates.
                using (var pipeline = PipelineBuilder
                    // We pass a null license key as we don't need to do any data file updates.
                    .UseOnPremise(dataFile, null, false)
                    // Disable share usage for this example.
                    .SetShareUsage(false)
                    // Prefer low memory profile where all data streamed from disk on-demand.
                    .SetPerformanceProfile(Pipeline.Engines.PerformanceProfiles.LowMemory)
                    // Disable all data file update mechanisms
                    .SetDataUpdateOnStartUp(false)
                    .SetAutoUpdate(false)
                    .SetDataFileSystemWatcher(false)
                    // You can improve matching performance by specifying only those
                    // properties you wish to use. If you don't specify any properties
                    // you will get all those available in the data file tier that
                    // you have used. The free "Lite" tier contains fewer than 20.
                    // Since we are specifying properties here, we will only see
                    // those properties, along with the match metric properties
                    // in the output.
                    .SetProperty("IsMobile")
                    // Uncomment BrowserName to include Browser component profile ID
                    // in the device ID value.
                    //.SetProperty("BrowserName")
                    // If using the full on-premise data file this property will be
                    // present in the data file. See https://51degrees.com/pricing
                    .SetProperty("HardwareName")
                    // Only use the predictive graph to better handle variances
                    // between the training data and the target User-Agent string.
                    // For a more detailed description of the differences between
                    // performance and predictive, see
                    // https://51degrees.com/documentation/_device_detection__hash.html#DeviceDetection_Hash_DataSetProduction_Performance
                    .SetUsePredictiveGraph(true)
                    .SetUsePerformanceGraph(false)
                    // build the pipeline
                    .Build())
                {
                    ExampleUtils.CheckDataFile(pipeline, Logger);

                    foreach (var evidenceValues in evidenceList)
                    {
                        // A using block MUST be used for the FlowData instance.
                        // This ensures that native resources created by the device detection engine
                        // are freed.
                        using (var data = pipeline.CreateFlowData())
                        {
                            // Process a single evidence to retrieve the values
                            // associated with the user-agent and other evidence such as sec-ch-* for the
                            // selected properties.
                            data.AddEvidence(evidenceValues)
                                .Process();

                            var device = data.Get<IDeviceData>();

                            output.WriteLine("--- Compare evidence with what was matched ---\n");
                            output.WriteLine("Evidence");
                            // output the evidence in reverse value length order
                            evidenceValues.OrderBy(e => e.Value.ToString().Length)
                                .ToList()
                                .ForEach(e => output.WriteLine($"\t{e.Key.PadRight(34)}: {e.Value}"));
                            // Obtain the matched User-Agents: the matched substrings in the
                            // User-Agents are separated with underscores - output in forward length order.
                            output.WriteLine("Matches");
                            device.UserAgents.Value.OrderByDescending(u => u.Length)
                                .ToList()
                                .ForEach(u => output.WriteLine($"\t{"Matched Chars".PadRight(34)}: {u}"));

                            output.WriteLine();


                            output.WriteLine("--- Listing all available properties, by component, by property " +
                                    "name ---");
                            output.WriteLine("For a discussion of what the match properties mean, see: " +
                                    "https://51degrees.com/documentation/_device_detection__hash" +
                                    ".html#DeviceDetection_Hash_DataSetProduction_Performance\n");
                            
                            // get the properties available from the DeviceDetection engine
                            // which has the key "device". For the sake of illustration we will
                            // retrieve it indirectly.
                            var hashEngineElementKey = pipeline
                                .GetElement<DeviceDetectionHashEngine>().ElementDataKey;

                            // retrieve the available properties from the hash engine. The properties
                            // available depends on
                            // a) the use of setProperty() in the builder (see above)
                            // which controls which properties will be extracted, and also affects
                            // the performance of extraction
                            // b) the tier of data file being used. The Lite data file contains fewer
                            // than 20 of the >200 available properties
                            pipeline.ElementAvailableProperties.TryGetValue(
                                hashEngineElementKey, out var availableProperties);

                            // create a Map keyed on the component name of the properties available
                            // components being hardware, browser, OS and Crawler.
                            // Match metric properties are not allocated to a component, so we will
                            // add a key "MatchMetric"
                            var componentMap = availableProperties
                                .Select(p => p.Value as IFiftyOneAspectPropertyMetaData)
                                // We've already handled the matched user agents property above.
                                .Where(p => p != null && p.Name != "UserAgents")
                                .GroupBy(p => p.Component?.Name ?? "MatchMetric");

                            // iterate the map created above
                            componentMap.ToList().ForEach(group =>
                            {
                                output.WriteLine(group.Key);
                                foreach (var entry in group)
                                {
                                    // while we get the available properties and their metadata
                                    // from the pipeline ...
                                    string propertyName = entry.Name;
                                    string propertyDescription = entry.Description;
                                    // ... we get the values for the last detection from flowData
                                    object value = device[entry.Name];

                                    // output property names, values and descriptions. Some property
                                    // values are lists so we need to handle those differently
                                    if (value is IAspectPropertyValue<IReadOnlyList<string>> listValue)
                                    {
                                        if (listValue.HasValue)
                                        {
                                            output.WriteLine($"\t{propertyName.PadRight(24)}: " +
                                                $"{listValue.Value.Count} Values");
                                            foreach (var listItem in listValue.Value)
                                            {
                                                output.WriteLine($"\t\t{"".PadRight(20)}: {listItem}");
                                            }
                                        }
                                        else
                                        {
                                            output.WriteLine($"\t{propertyName.PadRight(24)}: " +
                                                $"No value ({listValue.NoValueMessage})");
                                        }
                                    }
                                    else if (value is IAspectPropertyValue apv)
                                    {
                                        output.WriteLine($"\t{propertyName.PadRight(24)}: " +
                                            $"{(apv.HasValue ? apv.Value.ToString() : apv.NoValueMessage)}");
                                    }
                                    else
                                    {
                                        output.WriteLine($"\t{propertyName.PadRight(24)}: " +
                                            $"UNHANDLED TYPE '{value.GetType().Name}'");
                                    }
                                    output.WriteLine($"\t\t{propertyDescription}");
                                }
                            });

                            output.WriteLine();

                        }
                    }

                    Logger.LogInformation("Finished Example");
                }

            }

            private string CheckDataFile(string dataFile)
            {
                // No filename specified use the default
                if (dataFile == null)
                {
                    dataFile = Constants.LITE_HASH_DATA_FILE_NAME;
                    Logger.LogWarning($"No filename specified. Using default '{dataFile}'");
                }
                // Work out where the data file is if we don't have an absolute path.
                if (dataFile != null && Path.IsPathRooted(dataFile) == false)
                {
                    dataFile = ExampleUtils.FindFile(dataFile);
                }

                if(dataFile == null || File.Exists(dataFile) == false)
                {
                    Logger.LogError("Failed to find a device detection data file. " +
                        "If using the default 'lite' data file, make sure that git lfs is " +
                        "installed and that the device-detection-data submodule has been " +
                        "updated by running `git submodule update --recursive`.");
                    throw new PipelineConfigurationException($"Data file '{dataFile}' not found");
                }

                return dataFile;
            }
        }

        public static void Main(string[] args)
        {
            // Use the supplied path for the data file or find the lite file that is included
            // in the repository.
            var dataFile = args.Length > 0 ? args[0] : null;

            Initialize(dataFile, Console.Out);
        }

        public static void Initialize(
            string dataFile, TextWriter output)
        {
            // Initialize a service collection, which will be used to create the services
            // required by the Pipeline and manage their lifetimes.
            using (var serviceProvider = new ServiceCollection()
                // Make sure we're logging to the console.
                .AddLogging(l => l.AddConsole())
                // Add the builders we're going to need.
                .AddTransient<DeviceDetectionPipelineBuilder>()
                // Add example classes.
                .AddSingleton<Example>()
                .BuildServiceProvider())
            {
                var example = serviceProvider.GetRequiredService<Example>();
                example.Run(dataFile, ExampleUtils.EvidenceValues.Skip(2).Take(1), output);
            }
        }
    }

}
