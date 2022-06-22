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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;

/// <summary>
/// @example OnPremise/UpdateDataFile-Console/Program.cs
/// 
/// This example illustrates various parameters that can be adjusted when using the on-premise
/// device detection engine, and controls when a new data file is sought and when it is loaded by
/// the device detection software.
/// 
/// Three main aspects are demonstrated:
/// - Update on Start-Up
/// - Filesystem Watcher
/// - Daily auto-update
/// 
/// # License Key
/// In order to test this example you will need a 51Degrees Enterprise license which can be
/// purchased from our
/// <a href="https://51degrees.com/pricing">pricing page</a>. Look for our "Bigger" or
/// "Biggest" options.
/// 
/// # Data Files
/// You can find out more about data files, licenses etc. at our (FAQ page)[https://51degrees.com/resources/faqs]
/// 
/// ## Enterprise Data File
/// Enterprise (fully-featured) data files are typically released by 51Degrees four days a week
/// (Mon-Thu) and on-premise deployments can fetch and download those files automatically. Equally,
/// customers may choose to download the files themselves and move them into place to be detected
/// by the 51Degrees filesystem watcher.
/// 
/// ### Manual Download
/// If you prefer to download files yourself, you may do so here:
/// ```
/// https://distributor.51degrees.com/api/v2/download?LicenseKeys=<your_license_key>&Type=27&Download=True&Product=22
/// ```
/// 
/// ## Lite Data File
/// Lite data files (free-to-use, limited capabilities, no license key required) are created roughly
/// once a month and cannot be updated using auto-update, they may be downloaded from
/// (Github)[href=https://github.com/51Degrees/device-detection-data] and are included with
/// source distributions of this software.
/// 
/// # Update on Start-Up
/// You can configure the pipeline builder to download an Enterprise data file on start-up.
/// 
/// ## Pre-Requisites
/// - a license key
/// - a file location for the download
///      - this may be an existing file - which will be overwritten
///      - or if it does not exist must end in ".hash" and must be in an existing directory
///      
/// ## Configuration
/// - the pipeline must be configured to use a temp file
/// ``` {c#}
///    .UseOnPremise(dataFilename, true)
/// ```
/// - update on start-up must be specified, which will cause pipeline creation to block until a
/// file is downloaded
/// ``` {c#}
///      .setDataUpdateOnStartup(true)
/// ```
/// 
/// # File System Watcher
/// You can configure the pipeline builder to watch for changes to the currently loaded device
/// detection data file, and to replace the file currently in use with the new one. This is
/// useful, for example, if you wish to download and update the device detection file "manually" -
/// i.e. you would download it then drop it into place with the same path as the currently loaded
/// file.
///
/// ## Pre-Requisites
/// - a license key
/// - the file location of the existing file
/// 
/// ## Configuration
/// - the pipeline must be configured to use a temp file
/// ``` {c#}
///    .UseOnPremise(dataFilename, true)
/// ```
/// 
/// ## Daily auto-update
/// Enterprise data files are usually created four times a week. Each data file contains a date
/// for when the next data file is expected. You can configure the pipeline so that it starts
/// looking for a newer data file after that time, by connecting to the 51Degrees distributor to
/// see if an update is available. If one is, then it is downloaded and will replace the existing
/// device detection file, which is currently in use.
///
/// ## Pre-Requisites
/// - a license key
/// - the file location of the existing file
/// 
/// ## Configuration
/// - the pipeline must be configured with a license key and the 'use temp file' flag must be set.
/// ``` {c#}
///    .UseOnPremise(dataFile, licenseKey, true)
/// ```
/// - Set the frequency in seconds that the pipeline should check for updates to data files.
/// A recommended polling interval in a production environment is around 30 minutes.
/// ``` {c#}
///    .SetUpdatePollingInterval(30*60)
/// ```
/// - Set the max amount of time in seconds that should be added to the polling interval. This is
/// useful in data-center applications where multiple instances may be polling for updates at the
/// same time. A recommended amount in production environments is 600 seconds.
/// ``` {c#}
///    .SetUpdateRandomisationMax(10*60)
/// ```
/// 
/// # Location
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/blob/master/Examples/OnPremise/UpdateDataFile-Console/Program.cs).
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.OnPremise.UpdateDataFile
{
    public class Program
    {
        public class Example : ExampleBase
        {
            /// <summary>
            /// Builder used to create a Pipeline instance
            /// </summary>
            public DeviceDetectionPipelineBuilder PipelineBuilder { get; private set; }
            /// <summary>
            /// Builder used to create an on-premise device detection engine.
            /// </summary>
            public DeviceDetectionHashEngineBuilder EngineBuilder { get; private set; }
            public ILogger<Example> Logger { get; private set; }

            /// <summary>
            /// Timeout to use when waiting for updates to complete.
            /// </summary>
            private TimeSpan _updateTimeout = new TimeSpan(0, 0, 20);

            /// <summary>
            /// Helper that is used to wait for updates to complete.
            /// </summary>
            private CompletionListener CompletionListener;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pipelineBuilder">
            /// A builder used to create a pipeline that includes device detection.
            /// </param>
            /// <param name="engineBuilder">
            /// A builder used to create an on-premise device detection engine.
            /// Generally, we wouldn't need this in addition to the pipeline builder above.
            /// In this example, we use it to check information about the data file before 
            /// creating the full pipeline.
            /// </param>
            /// <param name="completionListener">
            /// A helper class that is used to watch for events that signal data updates have
            /// completed.
            /// </param>
            /// <param name="logger"></param>
            public Example(DeviceDetectionPipelineBuilder pipelineBuilder, 
                DeviceDetectionHashEngineBuilder engineBuilder,
                CompletionListener completionListener,
                ILogger<Example> logger)
            {
                PipelineBuilder = pipelineBuilder;
                EngineBuilder = engineBuilder;
                CompletionListener = completionListener;
                Logger = logger;
            }

            /// <summary>
            /// Run the example.
            /// </summary>
            /// <param name="dataFile">
            /// Path to a data file to use in this example. May be absolute or relative.
            /// If relative, the example will search for a matching file in the project 
            /// directories. 
            /// </param>
            /// <param name="licenseKey">
            /// The license key to use when requesting a data file update.
            /// </param>
            /// <param name="interactive">
            /// If this example is being run in an interactive session, set this to true.
            /// </param>
            public void Run(string dataFile, string licenseKey, bool interactive)
            {
                Logger.LogInformation("Starting example");

                licenseKey = CheckLicenseKey(licenseKey, Logger);
                dataFile = CheckDataFile(dataFile, Logger);

                string copyDataFile = dataFile + ".bak";
                if (File.Exists(dataFile))
                {
                    // Let's check this file out
                    var metadata = ExampleUtils.GetDataFileInfo(dataFile, EngineBuilder);
                    // and output the results
                    ExampleUtils.LogDataFileInfo(metadata, Logger);
                    if (metadata.Tier.Equals("Lite"))
                    {
                        Logger.LogError("Will not download an 'Enterprise' data file over the top of " +
                                "a 'Lite' data file, please supply another location.");
                        throw new ArgumentException("File supplied has wrong data tier");
                    }
                    Logger.LogInformation("Existing data file will be replaced with downloaded data file");
                    Logger.LogInformation("Existing data file will be copied to {}", copyDataFile);
                }

                // do we really want to do this
                if (interactive)
                {
                    Logger.LogWarning("Please note - this example will use available downloads " +
                            "in your licensed allocation.");
                    Logger.LogWarning("Do you wish to continue with this example (y)? ");
                    var key = Console.ReadKey();
                    if (key.Key != ConsoleKey.Y)
                    {
                        Logger.LogInformation("Stopping example without download");
                        return;
                    }
                }

                if (File.Exists(dataFile))
                {
                    File.Copy(dataFile, copyDataFile, true);
                    Logger.LogInformation("Existing data file copied to {}", copyDataFile);
                }

                Logger.LogInformation("Creating pipeline and initiating update on start-up - " +
                    "please wait for that to complete");

                // Build the device detection pipeline  and pass in the desired settings to configure
                // automatic updates.
                using (var pipeline = PipelineBuilder
                    // specify the filename for the data file. When using update on start-up
                    // the file need not exist, but the directory it is in must exist.
                    // Any file that is present is overwritten. Because the file will be
                    // overwritten, the pipeline must be configured to copy the supplied
                    // file to a temporary file (createTempDataCopy parameter == true).
                    //
                    // For automatic updates to work you will also need to provide a license key.
                    // A license key can be obtained with a subscription from https://51degrees.com/pricing
                    .UseOnPremise(dataFile, licenseKey, true)
                    // Enable update on startup, the auto update system
                    // will be used to check for an update before the
                    // device detection engine is created. This will block
                    // creation of the pipeline until the data file is downloaded.
                    .SetDataUpdateOnStartUp(true)
                    // Enable automatic updates once the pipeline has started.
                    .SetAutoUpdate(true)
                    // Watch the data file on disk and refresh the engine
                    // as soon as that file is updated.
                    .SetDataFileSystemWatcher(true)
                    // For the purposes of this example we are setting the time
                    // between checks to see if the file has changed to 1 second.
                    // By default this is 30 mins.
                    .SetUpdatePollingInterval(1)
                    // Build the pipeline.
                    .Build())
                {
                    // thread blocks till update checking is complete - or if there is an
                    // exception we don't get this far
                    Logger.LogInformation("Update on start-up complete - status - " +
                        CompletionListener.Result.Status);

                    if(CompletionListener.Result.Status == AutoUpdateStatus.AUTO_UPDATE_SUCCESS ||
                        CompletionListener.Result.Status == AutoUpdateStatus.AUTO_UPDATE_NOT_NEEDED)
                    {
                        Logger.LogInformation("Modifying downloaded file to trigger reload ... " +
                            "please wait for that to complete");
                        // wait for the dataUpdateService to notify us that it has updated
                        CompletionListener.Reset();

                        // it's the same file but changing the file metadata will trigger reload,
                        // demonstrating that if you download a new file and replace the
                        // existing one, then it will be loaded
                        try
                        {
                            new FileInfo(dataFile).LastWriteTimeUtc = DateTime.UtcNow;
                        }
                        catch (IOException) 
                        {
                            throw new InvalidOperationException("Could not modify file time, " +
                                "abandoning example");
                        }

                        try
                        {
                            CompletionListener.WaitForComplete(_updateTimeout);
                            Logger.LogInformation($"Update on file modification complete, " +
                                $"status: {CompletionListener.Result.Status}");
                        }
                        catch (TimeoutException)
                        {
                            Logger.LogError("Timeout waiting for engine to refresh data.");
                        }
                    }
                    else
                    {
                        Logger.LogError("Auto update was not successful, abandoning example");
                        throw new InvalidOperationException("Auto update failed: " +
                            CompletionListener.Result.Status);
                    }

                    Logger.LogInformation("Finished Example");
                }

            }

            private string CheckLicenseKey(string licenseKey, ILogger logger)
            {
                if (licenseKey == null)
                {
                    licenseKey = Environment.GetEnvironmentVariable(Constants.LICENSE_KEY_ENV_VAR);
                }
                if (licenseKey == null || ExampleUtils.IsInvalidKey(licenseKey))
                {
                    logger.LogError("In order to test this example you will need a 51Degrees " +
                        "Enterprise license which can be obtained on a trial basis or purchased " +
                        "from our pricing page https://51degrees.com/pricing. You must supply the " +
                        "license key as the second command line argument to this program, or as " +
                        $"an environment variable named '{Constants.LICENSE_KEY_ENV_VAR}'");
                    throw new ArgumentException("No license key available", nameof(licenseKey));
                }

                return licenseKey;
            }

            private string CheckDataFile(string dataFile, ILogger logger)
            {
                // No filename specified use the default
                if (dataFile == null)
                {
                    dataFile = Constants.ENTERPRISE_HASH_DATA_FILE_NAME;
                    logger.LogWarning($"No filename specified. Using default '{dataFile}'");
                }
                // Work out where the data file is if we don't have an absolute path.
                if (dataFile != null && Path.IsPathRooted(dataFile) == false)
                {
                    var fullPath = ExampleUtils.FindFile(dataFile);
                    if (fullPath == null)
                    {
                        dataFile = Path.Combine(Directory.GetCurrentDirectory(), dataFile);
                        logger.LogWarning($"File '{dataFile}' not found, a file will be " +
                            $"downloaded to that location on start-up");
                    }
                    else
                    {
                        dataFile = fullPath;
                    }
                }
                // If we do have an absolute path but the file does not exist, then log a warning.
                else if (dataFile != null && File.Exists(dataFile) == false)
                {
                    if (new FileInfo(dataFile).Directory.Exists == false)
                    {
                        logger.LogError("The directory must exist when specifying a " +
                            "location for a new file to be downloaded. Path specified was " +
                            $"'{dataFile}'");
                        throw new ArgumentException("Directory for new file must exist",
                            nameof(dataFile));
                    }
                    else
                    {
                        logger.LogWarning($"File '{dataFile}' not found, a file will be " +
                            $"downloaded to that location on start-up");
                    }
                }
                return dataFile;
            }
        }

        public static void Main(string[] args)
        {
            // Use the supplied path for the data file or find the lite file that is included
            // in the repository.
            var dataFile = args.Length > 0 ? args[0] : null;
            var licenseKey = args.Length > 1 ? args[1] : null;

            Initialize(dataFile, licenseKey, true);
        }

        public static void Initialize(
            string dataFile, string licenseKey, bool interactive)
        {
            // Initialize a service collection, which will be used to create the services
            // required by the Pipeline and manage their lifetimes.
            using (var serviceProvider = new ServiceCollection()
                // Make sure we're logging to the console.
                .AddLogging(l => l
                    .AddConsole()
                    // Only display messages @ warning or above.
                    // Or info and above if the come from this example.
                    // This filters out some messages from the Pipeline that would otherwise 
                    // make this example harder to follow.
                    .AddFilter((c, l) => 
                    {
                        return l >= LogLevel.Warning ||
                            (c.StartsWith(typeof(Program).FullName) && l >= LogLevel.Information);
                    }))
                // Add an HttpClient instance. This is used for making requests to the
                // data update end point.
                .AddSingleton<HttpClient>()
                // We want to use the standard data update service
                .AddSingleton<IDataUpdateService, DataUpdateService>()
                // Add the builders we're going to need.
                .AddTransient<DeviceDetectionPipelineBuilder>()
                .AddTransient<DeviceDetectionHashEngineBuilder>()
                // Add example classes.
                .AddSingleton<Example>()
                .AddSingleton<CompletionListener>()
                .BuildServiceProvider())
            {
                var example = serviceProvider.GetRequiredService<Example>();
                example.Run(dataFile, licenseKey, interactive);
            }
        }
    }
}
