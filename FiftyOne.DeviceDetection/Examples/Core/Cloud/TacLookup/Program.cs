using FiftyOne.DeviceDetection.Cloud.Data;
using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.DeviceDetection.Shared;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

/// <summary>
/// @example Cloud/TacLookup/Program.cs
///
/// @include{doc} example-tac-lookup-cloud.txt
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/FiftyOne.DeviceDetection/Examples/Core/Cloud/TacLookup/Program.cs). 
/// 
/// @include{doc} example-require-resourcekey.txt
///
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Cloud.TacLookup
{
    public class Program
    {
        public class Example
        {
            private static string TAC = "35925406";
            private static string TAC2 = "86386802";

            public void Run(string resourceKey, string cloudEndPoint = "")
            {
                Console.WriteLine("This example shows the details of devices " +
                    "associated with a given 'Type Allocation Code' or 'TAC'.");
                Console.WriteLine("More background information on TACs can be " +
                    "found through various online sources such as Wikipedia: " +
                    "https://en.wikipedia.org/wiki/Type_Allocation_Code");
                Console.WriteLine("----------------------------------------");

                ILoggerFactory loggerFactory = new LoggerFactory();
                HttpClient httpClient = new HttpClient();

                // Create a cloud request engine builder
                var cloudRequestEngineBuilder = new CloudRequestEngineBuilder(loggerFactory, httpClient)
                    .SetResourceKey(resourceKey);

                // If a cloud endpoint has been provided then set the
                // cloud pipeline endpoint. 
                if (string.IsNullOrWhiteSpace(cloudEndPoint) == false)
                {
                    cloudRequestEngineBuilder.SetEndPoint(cloudEndPoint);
                }

                // Create the cloud request engine
                using (var cloudEngine = cloudRequestEngineBuilder.Build())
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

            static void AnalyseTac(string tac, IPipeline pipeline)
            {
                // Create the FlowData instance.
                using (var data = pipeline.CreateFlowData())
                {
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

                        // Check that the properties have values.
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
                new Example().Run(resourceKey);
            }
#if (DEBUG)
            Console.WriteLine("Done. Press any key to exit.");
            Console.ReadKey();
#endif
        }
    }
}
