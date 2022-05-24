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
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;

/// <summary>
/// @example Cloud/NativeModel-Console/Program.cs
///
/// This example shows how to use the 51Degrees Cloud service to lookup the details of a device 
/// based on a given 'native model name'. Native model name is a string of characters that are 
/// returned from a query to the device's OS. 
/// There are different mechanisms to get native model names for 
/// [Android devices](https://developer.android.com/reference/android/os/Build#MODEL) and 
/// [iOS devices](https://gist.github.com/soapyigu/c99e1f45553070726f14c1bb0a54053b#file-machinename-swift)
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/Examples/Cloud/NativeModel-Console/Program.cs). 
/// 
/// @include{doc} example-require-resourcekey.txt
///
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// - Microsoft.Extensions.Logging.Console
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Cloud.NativeModelLookup
{
    public class Program
    {
        public class Example
        {
            // Example values to use when looking up device details from native model names.
            private const string _nativeModel1 = "SC-03L";
            private const string _nativeModel2 = "iPhone11,8";

            public void Run(string resourceKey, ILoggerFactory loggerFactory, 
                TextWriter output, string cloudEndPoint = "")
            {
                output.WriteLine("This example shows the details of devices " +
                    "associated with a given 'native model name'.");
                output.WriteLine($"The native model name can be retrieved by " +
                    $"code running on the device (For example, a mobile app).");
                output.WriteLine($"For Android devices, see " +
                    $"https://developer.android.com/reference/android/os/Build#MODEL");
                output.WriteLine($"For iOS devices, see " +
                    $"https://gist.github.com/soapyigu/c99e1f45553070726f14c1bb0a54053b#file-machinename-swift");
                output.WriteLine("----------------------------------------");

                HttpClient httpClient = new HttpClient();

                // This example creates the pipeline and engines in code. For a demonstration
                // of how to do this using a configuration file instead, see the TacLookup example.
                // For more information about builders in general see the documentation at
                // http://51degrees.com/documentation/_concepts__configuration__builders__index.html
                var cloudRequestEngineBuilder = new CloudRequestEngineBuilder(loggerFactory, httpClient)
                    .SetResourceKey(resourceKey);

                // If a cloud endpoint has been provided then set the
                // cloud pipeline endpoint. 
                if (string.IsNullOrWhiteSpace(cloudEndPoint) == false)
                {
                    cloudRequestEngineBuilder.SetEndPoint(cloudEndPoint);
                }

                // Create the cloud request engine.
                using (var cloudEngine = cloudRequestEngineBuilder.Build())
                // Create the hardware profile engine to process the response from the
                // request engine.
                using (var propertyKeyedEngine = new HardwareProfileCloudEngineBuilder(loggerFactory)
                    .Build())
                // Create the pipeline using the engines.
                using (var pipeline = new PipelineBuilder(loggerFactory)
                    .AddFlowElement(cloudEngine)
                    .AddFlowElement(propertyKeyedEngine)
                    .Build())
                {
                    // Pass a native model into the pipeline and list the matching devices.
                    AnalyseNativeModel(_nativeModel1, pipeline, output);
                    // Repeat for an alternative native model name.
                    AnalyseNativeModel(_nativeModel2, pipeline, output);
                }
            }

            static void AnalyseNativeModel(string nativemodel, IPipeline pipeline, TextWriter output)
            {
                // Create the FlowData instance. This is wrapped in a using block to ensure 
                // resources are disposed correctly.
                using (var data = pipeline.CreateFlowData())
                {
                    // Add the native model key as evidence.
                    data.AddEvidence(Shared.Constants.EVIDENCE_QUERY_NATIVE_MODEL_KEY, nativemodel);
                    // Process the supplied evidence.
                    data.Process();
                    // Get result data from the flow data.
                    var result = data.Get<MultiDeviceDataCloud>();
                    output.WriteLine($"Which devices are associated with the " +
                        $"native model name '{nativemodel}'?");
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
        }

        static void Main(string[] args)
        {
            // Use the command line args to get the resource key if present.
            // Otherwise, get it from the environment variable.
            string resourceKey = args.Length > 0 ? args[0] :
                Environment.GetEnvironmentVariable(
                    ExampleUtils.RESOURCE_KEY_ENV_VAR);

            // Configure a logger to output to the console.
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            var logger = loggerFactory.CreateLogger<Program>();

            if (string.IsNullOrEmpty(resourceKey))
            {
                logger.LogError($"No resource key specified on the command line or in the " +
                    $"environment variable '{ExampleUtils.RESOURCE_KEY_ENV_VAR}'. " +
                    $"The 51Degrees cloud service is accessed using a 'ResourceKey'. " +
                    $"For more information " +
                    $"see http://51degrees.com/documentation/_info__resource_keys.html. " +
                    $"Native model lookup is not available as a free service. This means that " +
                    $"you will first need a license key, which can be purchased from our " +
                    $"pricing page: http://51degrees.com/pricing. Once this is done, a resource " +
                    $"key with the properties required by this example can be created at " +
                    $"https://configure.51degrees.com/QKyYH5XT. You can now populate the " +
                    $"environment variable mentioned at the start of this message with the " +
                    $"resource key or pass it as the first argument on the command line.");
            }
            else
            {
                new Example().Run(resourceKey, loggerFactory, Console.Out);
            }

            // Dispose the logger to ensure any messages get flushed
            loggerFactory.Dispose();
        }
    }
}
