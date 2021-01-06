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
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// @example Hash/AutomaticUpdates/UpdateOnStartUp/Program.cs
/// 
/// @include{doc} example-automatic-updates-on-startup-hash.txt
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/FiftyOne.DeviceDetection/Examples/Framework/Hash/AutomaticUpdates/UpdateOnStartUp/Program.cs). 
/// 
/// @include{doc} example-require-licensekey.txt
/// @include{doc} example-require-datafile.txt
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// 
/// Expected output:
/// ```
/// Using data file at '51Degrees-LiteV4.1.hash'
/// Data file published date: 13/04/2020 00:00:00
/// Creating pipeline and updating device data.....
/// Data file published date: 02/07/2020 00:00:00
/// ```
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Hash.AutomaticUpdates.UpdateOnStartUp
{
    public class Program
    {
        public class Example : ExampleBase
        {
            private string dataFile = "51Degrees.hash";

            public void Run(string originalDataFile, string licenseKey)
            {
                // Copy the original data file to another location as we do not 
                // want to preserve the original for other examples.
                File.Copy(originalDataFile, dataFile, true);

                FileInfo f = new FileInfo(dataFile);
                Console.WriteLine($"Using data file at '{f.FullName}'");

                DateTime initialPublishedDate = DateTime.MinValue;

                // Create a temporary Device Detection 'Hash' Engine to check the initial published date of the data file.
                // There is no need to do this in production, it is for demonstration purposes only.
                // This also higlights the added simplicity of the Device Detection Pipeline builder.
                using (var loggerFactory = new LoggerFactory()) {
                    var dataUpdateService = new DataUpdateService(loggerFactory.CreateLogger<DataUpdateService>(), new HttpClient());
                    using (var temporaryDeviceDetectionEngine = new DeviceDetectionHashEngineBuilder(loggerFactory, dataUpdateService)
                        .Build(dataFile, false))
                    {
                        // Get the published date of the device data file. Engines can have multiple data files but 
                        // for the Device Detection 'Hash' engine we can guarantee there will only be one.
                        initialPublishedDate = temporaryDeviceDetectionEngine.DataFiles.Single().DataPublishedDateTime;
                    }
                }

                Console.WriteLine($"Data file published date: {initialPublishedDate}");
                Console.Write($"Creating pipeline and updating device data");
                CancellationTokenSource cancellationSource = new CancellationTokenSource();
                Task.Run(() => { OutputUntilCancelled(".", 1000, cancellationSource.Token); });

                // Build a new Pipeline to use an on-premise Hash engine and 
                // configure automatic updates.
                var pipeline = new DeviceDetectionPipelineBuilder()
                    // Use the On-Premise engine (aka Hash engine) and pass in
                    // the path to the data file, your license key and whether 
                    // to use a temporary file.
                    // A license key is required otherwise automatic updates will
                    // remain disabled.
                    .UseOnPremise(dataFile, licenseKey, true)
                    // Enable automatic updates.
                    .SetAutoUpdate(true)
                    // Watch the data file on disk and refresh the engine 
                    // as soon as that file is updated. 
                    .SetDataFileSystemWatcher(true)
                    // Enable update on startup, the auto update system 
                    // will be used to check for an update before the
                    // device detection engine is created. This will block 
                    // creation of the pipeline.
                    .SetDataUpdateOnStartUp(true)
                    .Build();

                cancellationSource.Cancel();
                Console.WriteLine();

                // Get the published date of the data file from the Hash engine 
                // after building the pipeline.
                var updatedPublishedDate = pipeline
                    .GetElement<DeviceDetectionHashEngine>()
                    .DataFiles
                    .Single()
                    .DataPublishedDateTime;
                
                if (DateTime.Equals(initialPublishedDate, updatedPublishedDate))
                {
                    Console.WriteLine("There was no update available at this time.");
                }
                Console.WriteLine($"Data file published date: {updatedPublishedDate}");
            }
        }

        private static void OutputUntilCancelled(string text, int intervalMs, CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                Console.Write(text);
                Task.Delay(intervalMs).Wait();
            }
        }

        static void Main(string[] args)
        {
            var licenseKey = "!!Your license key!!";

            if (licenseKey.StartsWith("!!"))
            {
                Console.WriteLine("You need a license key to run this example, " +
                    "you can obtain one by subscribing to a 51Degrees bundle: https://51degrees.com/pricing");
                Console.ReadKey();
                return;
            }

#if NETCORE
            var defaultDataFile = "..\\..\\..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV4.1.hash";
#else
            var defaultDataFile = "..\\..\\..\\..\\..\\..\\..\\..\\device-detection-cxx\\device-detection-data\\51Degrees-LiteV4.1.hash";
#endif
            var dataFile = args.Length > 0 ? args[0] : defaultDataFile;
            new Example().Run(dataFile, licenseKey);
#if (DEBUG)
            Console.WriteLine("Complete. Press key to exit.");
            Console.ReadKey();
#endif
        }
    }
}