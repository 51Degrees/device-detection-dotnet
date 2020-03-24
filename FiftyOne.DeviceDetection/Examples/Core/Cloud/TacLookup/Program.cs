using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.DeviceDetection.Shared;
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;

namespace TacLookup
{
    class Program
    {
        private static string TAC = "86386802";

        static void Main(string[] args)
        {
            // Obtain a resource key for free at https://configure.51degrees.com
            // Make sure to include the 'HardwareVendor' and 'HardwareModel' 
            // properties as they are used by this example.
            string resourceKey = "!!YOUR_RESOURCE_KEY!!";

            if (resourceKey.StartsWith("!!"))
            {
                Console.WriteLine("You need to create a resource key at " +
                    "https://configure.51degrees.com and paste it into this example.");
                Console.WriteLine("Make sure to include the 'HardwareVendor' " +
                    "and 'HardwareModel' properties as they are used by this " +
                    "example.");
            }
            else
            {
                ILoggerFactory loggerFactory = new LoggerFactory();
                HttpClient httpClient = new HttpClient();

                // Create the cloud request engine
                using (var cloudEngine = new CloudRequestEngineBuilder(loggerFactory, httpClient)
                    .SetResourceKey(resourceKey)
                    .Build())
                // Create the property-keyed engine to process the 
                // response from the request engine.
                using (var propertyKeyedEngine = new PropertyKeyedCloudEngineBuilder(loggerFactory)
                    .Build())
                // Create the pipeline using the engines.
                using (var pipeline = new PipelineBuilder(loggerFactory)
                    .AddFlowElement(cloudEngine)
                    .AddFlowElement(propertyKeyedEngine)
                    .Build())
                {
                    // Pass a TAC into the pipeline and list the matching devices.
                    AnalyseTac(TAC, pipeline);
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
            // Get device data from the flow data.
            var devices = data.Get<IMultiDeviceData>();
            Console.WriteLine($"Which devices are associated with the TAC '{tac}'?");
            foreach (var device in devices.Devices)
            {
                StringBuilder text = new StringBuilder("\t");
                if (device.HardwareVendor.HasValue)
                {
                    text.Append(device.HardwareVendor.ToString());
                }
                else
                {
                    text.Append(device.HardwareVendor.NoValueMessage);
                }
                text.Append(" ");
                if (device.HardwareModel.HasValue)
                {
                    text.Append(device.HardwareModel.ToString());
                }
                else
                {
                    text.Append(device.HardwareModel.NoValueMessage);
                }
                Console.WriteLine(text.ToString());
            }
        }
    }
}
