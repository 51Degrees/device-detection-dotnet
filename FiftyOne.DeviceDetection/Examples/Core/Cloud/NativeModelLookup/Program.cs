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
        private static string nativemodel1 = "iPhone11,8";
        private static string nativemodel2 = "Moto g(4)";

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
                Console.WriteLine($"The native model name can be retrieved by " +
                    $"code running on the device (For example, a mobile app).");
                Console.WriteLine($"For Android devices, see " +
                    $"https://developer.android.com/reference/android/os/Build#MODEL");
                Console.WriteLine($"For iOS devices, see " +
                    $"https://gist.github.com/soapyigu/c99e1f45553070726f14c1bb0a54053b#file-machinename-swift");
                Console.WriteLine("----------------------------------------");

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
                    // Pass an iOS native model into the pipeline and 
                    // list the matching devices.
                    AnalyseTac(nativemodel1, pipeline);
                    // Repeat for an Android native model name.
                    AnalyseTac(nativemodel2, pipeline);
                }
            }
#if (DEBUG)
            Console.WriteLine("Done. Press any key to exit.");
            Console.ReadKey();
#endif
        }

        static void AnalyseTac(string nativemodel, IPipeline pipeline)
        {
            // Create the FlowData instance.
            var data = pipeline.CreateFlowData();
            // Add the native model key as evidence.
            data.AddEvidence(Constants.EVIDENCE_QUERY_NATIVE_MODEL_KEY, nativemodel);
            // Process the supplied evidence.
            data.Process();
            // Get device data from the flow data.
            var devices = data.Get<IMultiDeviceData>();
            Console.WriteLine($"Which devices are associated with the " +
                $"native model name '{nativemodel}'?");
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
