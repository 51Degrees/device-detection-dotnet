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
