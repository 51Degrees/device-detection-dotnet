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

using FiftyOne.DeviceDetection.Cloud.Data;
using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;

/// <summary>
/// @example Cloud/TAC-Console/Program.cs
///
/// This example shows how to use the 51Degrees Cloud service to lookup the details of a device 
/// based on a given 'TAC'. More background information on TACs can be found through various online 
/// sources such as <a href="https://en.wikipedia.org/wiki/Type_Allocation_Code">Wikipedia</a>.
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/Examples/Cloud/TAC-Console/Program.cs). 
/// 
/// @include{doc} example-require-resourcekey.txt
///
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// - Microsoft.Extensions.Configuration.Json
/// - Microsoft.Extensions.DependencyInjection
/// - Microsoft.Extensions.Logging.Console
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Cloud.TacLookup
{
    public class Program
    {
        public class Example
        {
            // Example values to use when looking up device details from TACs.
            private const string _tac1 = "35925406";
            private const string _tac2 = "86386802";

            public void Run(IServiceProvider serviceProvider, TextWriter output)
            {
                output.WriteLine("This example shows the details of devices " +
                    "associated with a given 'Type Allocation Code' or 'TAC'.");
                output.WriteLine("More background information on TACs can be " +
                    "found through various online sources such as Wikipedia: " +
                    "https://en.wikipedia.org/wiki/Type_Allocation_Code");
                output.WriteLine("----------------------------------------");

                var pipelineOptions = serviceProvider.GetRequiredService<PipelineOptions>();
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                // In this example, we use the FiftyOnePipelineBuilder and configure it from a file.
                // For a demonstration of how to do this in code instead, see the
                // NativeModelLookup example.
                // For more information about builders in general see the documentation at
                // http://51degrees.com/documentation/_concepts__configuration__builders__index.html

                // Create the pipeline using the service provider and the configured options.
                using (var pipeline = new FiftyOnePipelineBuilder(loggerFactory, serviceProvider)
                    .BuildFromConfiguration(pipelineOptions))
                {
                    // Pass a TAC into the pipeline and list the matching devices.
                    AnalyseTac(_tac1, pipeline, output);
                    AnalyseTac(_tac2, pipeline, output);
                }
            }

            static void AnalyseTac(string tac, IPipeline pipeline, TextWriter output)
            {
                // Create the FlowData instance.
                using (var data = pipeline.CreateFlowData())
                {
                    // Add the TAC as evidence.
                    data.AddEvidence(Shared.Constants.EVIDENCE_QUERY_TAC_KEY, tac);
                    // Process the supplied evidence.
                    data.Process();
                    // Get result data from the flow data.
                    var result = data.Get<MultiDeviceDataCloud>();
                    output.WriteLine($"Which devices are associated with the TAC '{tac}'?");
                    // The 'MultiDeviceDataCloud' object contains one or more instances
                    // implementing 'IDeviceData'.
                    // This is the same interface used for standard device detection, so we have
                    // access to all the same properties.
                    foreach (var device in result.Profiles)
                    {
                        var vendor = device.HardwareVendor.GetHumanReadable();
                        var name = device.HardwareName.GetHumanReadable();
                        var model = device.HardwareModel.GetHumanReadable();
                        output.WriteLine($"\t{vendor} {name} ({model})");
                    }
                }
            }

            /// <summary>
            /// This Run method is called by the example test to avoid the need to duplicate the 
            /// service provider setup logic.
            /// </summary>
            /// <param name="options"></param>
            public void Run(PipelineOptions options, TextWriter output)
            {
                // Initialize a service collection which will be used to create the services
                // required by the Pipeline and manage their lifetimes.
                using (var serviceProvider = new ServiceCollection()
                    // Add the configuration to the services collection.
                    .AddSingleton(options)
                    // Make sure we're logging to the console.
                    .AddLogging(l => l.AddConsole())
                    // Add an HttpClient instance. This is used for making requests to the
                    // cloud service.
                    .AddSingleton<HttpClient>()
                    // Add the builders that will be needed to create the engines specified in the 
                    // configuration file.
                    .AddSingleton<CloudRequestEngineBuilder>()
                    .AddSingleton<HardwareProfileCloudEngineBuilder>()
                    .BuildServiceProvider())
                {
                    // Get the resource key setting from the config file. 
                    var resourceKey = options.GetResourceKey();

                    // If we don't have a resource key then log an error.
                    if (string.IsNullOrWhiteSpace(resourceKey))
                    {
                        serviceProvider.GetRequiredService<ILogger<Program>>().LogError(
                            $"No resource key specified on the command line or in " +
                            $"the environment variable '{ExampleUtils.RESOURCE_KEY_ENV_VAR}'. " +
                            $"The 51Degrees cloud service is accessed using a 'ResourceKey'. " +
                            $"For more information see " +
                            $"http://51degrees.com/documentation/_info__resource_keys.html. " +
                            $"TAC lookup is not available as a free service. This means " +
                            $"that you will first need a license key, which can be purchased " +
                            $"from our pricing page: http://51degrees.com/pricing. Once this is " +
                            $"done, a resource key with the properties required by this example " +
                            $"can be created at https://configure.51degrees.com/QKyYH5XT. You " +
                            $"can now populate the environment variable mentioned at the start " +
                            $"of this message with the resource key or pass it as the first " +
                            $"argument on the command line.");
                    }
                    else
                    {
                        new Example().Run(serviceProvider, output);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            // Use the command line args to get the resource key if present.
            // Otherwise, get it from the environment variable.
            string resourceKey = args.Length > 0 ? args[0] :
                Environment.GetEnvironmentVariable(
                    ExampleUtils.RESOURCE_KEY_ENV_VAR);

            // Load the configuration file
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Bind the configuration to a pipeline options instance
            PipelineOptions options = new PipelineOptions();
            config.Bind("PipelineOptions", options);

            // Get the resource key setting from the config file. 
            var resourceKeyFromConfig = options.GetResourceKey();
            var configHasKey = string.IsNullOrWhiteSpace(resourceKeyFromConfig) == false &&
                    resourceKeyFromConfig.StartsWith("!!") == false;

            // If no resource key is specified in the config file then override it with the key
            // from the environment variable / command line. 
            if (configHasKey == false)
            {
                options.SetResourceKey(resourceKey);
            }

            new Example().Run(options, Console.Out);
        }
    }
}
