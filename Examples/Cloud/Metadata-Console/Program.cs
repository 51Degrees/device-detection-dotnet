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

using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

/// <summary>
/// @example Cloud/Metadata-Console/Program.cs
///
/// The cloud service exposes meta data that can provide additional information about the various 
/// properties that might be returned.
/// This example shows how to access this data and display the values available.
/// 
/// A list of the properties will be displayed, along with some additional information about each
/// property. Note that this is the list of properties used by the supplied resource key, rather
/// than all properties that can be returned by the cloud service.
/// 
/// In addition, the evidence keys that are accepted by the service are listed. These are the 
/// keys that, when added to the evidence collection in flow data, could have some impact on the
/// result that is returned.
/// 
/// Bear in mind that this is a list of ALL evidence keys accepted by all products offered by the 
/// cloud. If you are only using a single product (for example - device detection) then not all
/// of these keys will be relevant.
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/Examples/Cloud/Metadata/Program.cs). 
/// 
/// @include{doc} example-require-resourcekey.txt
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// - Microsoft.Extensions.Logging.Console
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Cloud.Metadata
{
    public class Program
    {
        public class Example : ExampleBase
        {
            public void Run(string resourceKey, ILoggerFactory loggerFactory, TextWriter output)
            {
                using (var pipeline = new DeviceDetectionPipelineBuilder(loggerFactory)
                    .UseCloud(resourceKey)
                    .Build())
                {
                    OutputProperties(pipeline.GetElement<DeviceDetectionCloudEngine>(), output);
                    // We use the CloudRequestEngine to get evidence key details, rather than the
                    // DeviceDetectionCloudEngine.
                    // This is because the DeviceDetectionCloudEngine doesn't actually make use
                    // of any evidence values. It simply processes the JSON that is returned
                    // by the call to the cloud service that is made by the CloudRequestEngine.
                    // The CloudRequestEngine is actually taking the evidence values and passing
                    // them to the cloud, so that's the engine we want the keys from.
                    OutputEvidenceKeyDetails(pipeline.GetElement<CloudRequestEngine>(), output);
                }
            }

            private void OutputEvidenceKeyDetails(CloudRequestEngine engine, TextWriter output)
            {
                output.WriteLine();
                if (typeof(EvidenceKeyFilterWhitelist).IsAssignableFrom(engine.EvidenceKeyFilter.GetType()))
                {
                    // If the evidence key filter extends EvidenceKeyFilterWhitelist then we can
                    // display a list of accepted keys.
                    var filter = engine.EvidenceKeyFilter as EvidenceKeyFilterWhitelist;
                    output.WriteLine($"Accepted evidence keys:");
                    foreach (var entry in filter.Whitelist)
                    {
                        output.WriteLine($"\t{entry.Key}");
                    }
                }
                else
                {
                    output.WriteLine($"The evidence key filter has type " +
                        $"{engine.EvidenceKeyFilter.GetType().Name}. As this does not extend " +
                        $"EvidenceKeyFilterWhitelist, a list of accepted values cannot be " +
                        $"displayed. As an alternative, you can pass evidence keys to " +
                        $"filter.Include(string) to see if a particular key will be included " +
                        $"or not.");
                    output.WriteLine($"For example, header.user-agent is " +
                        (engine.EvidenceKeyFilter.Include("header.user-agent") ? "" : "not ") +
                        "accepted.");
                }

            }

            private void OutputProperties(DeviceDetectionCloudEngine engine, TextWriter output)
            {
                foreach (var property in engine.Properties)
                {
                    // Output some details about the property.
                    // If we're outputting to console then we also add some formatting to make it 
                    // more readable.
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    output.Write($"Property - {property.Name}");
                    Console.ResetColor();
                    var typeName = property.Type.IsGenericType ? 
                        property.Type.GenericTypeArguments[0].Name : property.Type.Name;
                    output.WriteLine($"[Category: {property.Category}] ({typeName})");
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
                logger.LogError($"No resource key specified in the configuration file " +
                    $"'appsettings.json' or the environment variable " +
                    $"'{ExampleUtils.RESOURCE_KEY_ENV_VAR}'. The 51Degrees cloud service is " +
                    $"accessed using a 'ResourceKey'. For more information see " +
                    $"http://51degrees.com/documentation/_info__resource_keys.html. " +
                    $"A resource key with the properties required by this example can be " +
                    $"created for free at https://configure.51degrees.com/1QWJwHxl. " +
                    $"Once complete, supply the resource key as a command line argument or via " +
                    $"the environment variable mentioned at the start of this message.");
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