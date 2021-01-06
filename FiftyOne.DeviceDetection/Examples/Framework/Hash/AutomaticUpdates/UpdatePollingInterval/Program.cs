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
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;

/// <summary>
/// @example Hash/AutomaticUpdates/UpdatePollingInterval/Program.cs
/// 
/// @include{doc} example-automatic-updates-polling-interval-hash.txt
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
/// The pipeline has now been set up to poll for updates every ...
/// Update started for ...
/// Data file published date: 13/04/2020 00:00:00
/// Update completed. Status AUTO_UPDATE_SUCCESS
/// Data file published date: 02/07/2020 00:00:00
/// ```
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.Hash.AutomaticUpdates.UpdatePollingInterval
{ 
    public class Program
    {
        public class Example : ExampleBase
        {
            private IPipeline pipeline;
            private int updatePollingInterval = 30;
            private int pollingIntervalRandomisation = 10;
            private EventWaitHandle ewh;
            private string dataFile = "51Degrees.hash";

            public void Run(string originalDataFile, string licenseKey)
            {
                // Copy the original data file to another location as we do not 
                // want to preserve the original for other examples.
                File.Copy(originalDataFile, dataFile, true);

                // Create an event wait handler to wait for the update complete
                //event so that the example runs during the update process.
                ewh = new EventWaitHandle(false, EventResetMode.AutoReset);
                
                FileInfo f = new FileInfo(dataFile);
                Console.WriteLine($"Using data file at '{f.FullName}'");

                // Construct our own data update service so we can monitor update events.
                var loggerFactory = new LoggerFactory();
                var httpClient = new HttpClient();
                var dataUpdateService = new DataUpdateService(loggerFactory.CreateLogger<DataUpdateService>(), httpClient);

                // Bind a method to the CheckForUpdateComplete event and
                // the CheckForUpdateStarted event which
                // prints the published date of the data file.
                dataUpdateService.CheckForUpdateComplete += LogPublishedDate;
                dataUpdateService.CheckForUpdateStarted += LogPublishedDate;

                // Build a new Pipeline to use an on-premise Hash engine and 
                // configure automatic updates.
                pipeline = new DeviceDetectionPipelineBuilder(loggerFactory, httpClient, dataUpdateService)
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
                    // Disable update on startup
                    .SetDataUpdateOnStartUp(false)
                    // Set the frequency in seconds that the pipeline should
                    // check for updates to data files. A recommended 
                    // polling interval in a production environment is
                    // around 30 minutes or 1800 seconds.
                    .SetUpdatePollingInterval(30)
                    // Set the max ammount of time in seconds that should be 
                    // added to the polling interval. This is useful in datacenter
                    // applications where mulitple instances may be polling for 
                    // updates at the same time. A recommended ammount in production 
                    // environments is 600 seconds.
                    .SetUpdateRandomisationMax(10)
                    .Build();

                // Get the published date of the data file from the Hash engine 
                // after building the pipeline.
                var publishedDate = pipeline
                    .GetElement<DeviceDetectionHashEngine>()
                    .DataFiles
                    .Single()
                    .DataPublishedDateTime;
                Console.WriteLine($"Initial data file published date: {publishedDate}");

                Console.WriteLine($"The pipeline has now been set up to poll for updates every {updatePollingInterval} seconds, " +
                    $"a random ammount of time up to {pollingIntervalRandomisation} seconds will be added.");

                // Wait for the update complete event.
                ewh.WaitOne();
            }

            private void LogPublishedDate<T>(object sender, T e) where T : DataUpdateEventArgs
            {
                var publishedDate = pipeline
                    .GetElement<DeviceDetectionHashEngine>()
                    .DataFiles
                    .Single()
                    .DataPublishedDateTime;

                if (e is DataUpdateCompleteArgs completeArgs)
                {
                    Console.WriteLine($"Update completed. Status {completeArgs.Status}");
                    // Set the event wait handler once the update is completed.
                    ewh.Set();
                }
                else
                {
                    Console.WriteLine($"Update started for {e.DataFile.DataFilePath}");
                }

                Console.WriteLine($"Data file published date: {publishedDate}");
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