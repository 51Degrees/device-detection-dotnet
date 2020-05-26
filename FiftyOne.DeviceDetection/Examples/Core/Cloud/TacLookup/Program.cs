using FiftyOne.DeviceDetection.Cloud.Data;
using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.DeviceDetection.Shared;
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

/// <summary>
/// @example Cloud/TacLookup/Program.cs
///
/// TAC lookup example of using 51Degrees device detection.
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/FiftyOne.DeviceDetection/Examples/Core/Cloud/TacLookup/Program.cs). 
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
/// 1. Build a new Pipeline with a cloud-based hardware profiles engine.
/// ```
/// var cloudEngine = new CloudRequestEngineBuilder(loggerFactory, httpClient)
///     .SetResourceKey(resourceKey)
///     .Build();
/// var hardwareProfileEngine = new HardwareProfileCloudEngineBuilder(loggerFactory)
///     .Build();
/// var pipeline = new PipelineBuilder(loggerFactory)
///     .AddFlowElement(cloudEngine)
///    .AddFlowElement(hardweareProfileEngine)
///    .Build();
/// ```
///
/// 2. Create a new FlowData instance ready to be populated with evidence for the
/// Pipeline.
/// ```
/// var data = pipeline.CreateFlowData();
/// ```
///
/// 3. Add a TAC string to the evidence collection and process it.
/// ```
/// data.AddEvidence(Constants.EVIDENCE_QUERY_TAC_KEY, tac)
///     .Process();
/// ```
///
/// 4. Extract the value of a property as a string from the results.
/// ```
/// var profiles = data.Get<MultiDeviceDataCloud>().Profiles;
/// Console.WriteLine("IsMobile: " + profiles[0].IsMobile.Value);
/// ```
/// </summary>
namespace TacLookup
{
    class Program
    {
        private static string TAC = "35925406";
        private static string TAC2 = "86386802";

        static void Main(string[] args)
        {
            // Obtain a resource key for free at https://configure.51degrees.com
            // Make sure to include the 'HardwareVendor' and 'HardwareModel' 
            // properties as they are used by this example.
            string resourceKey = "!!YOUR_RESOURCE_KEY!!";

            if (resourceKey.StartsWith("!!"))
            {
                Console.WriteLine("You need to create a resource key at " +
                    "https://configure.51degrees.com and paste it into the code, " +
                    "replacing !!YOUR_RESOURCE_KEY!!.");
                Console.WriteLine("Make sure to include the 'HardwareVendor', " +
                    "'HardwareName' and 'HardwareModel' properties as they " +
                    "are used by this example.");
            }
            else
            {
                Console.WriteLine("This example shows the details of devices " +
                    "associated with a given 'Type Allocation Code' or 'TAC'.");
                Console.WriteLine("More background information on TACs can be " +
                    "found through various online sources such as Wikipedia: " +
                    "https://en.wikipedia.org/wiki/Type_Allocation_Code");
                Console.WriteLine("----------------------------------------");

                ILoggerFactory loggerFactory = new LoggerFactory();
                HttpClient httpClient = new HttpClient();

                // Create the cloud request engine
                using (var cloudEngine = new CloudRequestEngineBuilder(loggerFactory, httpClient)
                    .SetResourceKey(resourceKey)
                    .Build())
                // Create the property-keyed engine to process the 
                // response from the request engine.
                using (var propertyKeyedEngine = new HardwareProfileCloudEngineBuilder(loggerFactory)
                    .Build())
                // Create the pipeline using the engines.
                using (var pipeline = new PipelineBuilder(loggerFactory)
                    .AddFlowElement(cloudEngine)
                    .AddFlowElement(propertyKeyedEngine)
                    .Build())
                {
                    // Pass a TAC into the pipeline and list the matching devices.
                    AnalyseTac(TAC, pipeline);
                    AnalyseTac(TAC2, pipeline);
                }
            }
#if (DEBUG)
            Console.WriteLine("Done. Press any key to exit.");
            Console.ReadKey();
#endif
        }

        static void AnalyseTac(string tac, IPipeline pipeline)
        {
            // Create the FlowData instance.
            var data = pipeline.CreateFlowData();
            // Add the TAC as evidence.
            data.AddEvidence(Constants.EVIDENCE_QUERY_TAC_KEY, tac);
            // Process the supplied evidence.
            data.Process();
            // Get result data from the flow data.
            var result = data.Get<MultiDeviceDataCloud>();
            Console.WriteLine($"Which devices are associated with the TAC '{tac}'?");
            foreach (var device in result.Profiles)
            {
                var vendor = device.HardwareVendor;
                var name = device.HardwareName;
                var model = device.HardwareModel;

                if (vendor.HasValue &&
                    model.HasValue &&
                    name.HasValue)
                {
                    Console.WriteLine($"\t{vendor.Value} {string.Join(",", name.Value)} ({model.Value})");
                }
                else
                {
                    Console.WriteLine(vendor.NoValueMessage);
                }
            }
        }
    }
}
