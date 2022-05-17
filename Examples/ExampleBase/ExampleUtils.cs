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
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Examples
{
    public static class ExampleUtils
    {
        /// <summary>
        /// The default environment variable used to get the resource key to use when running 
        /// examples.
        /// </summary>
        public const string RESOURCE_KEY_ENV_VAR = "51D_RESOURCE_KEY";

        /// <summary>
        /// Timeout used when searching for files.
        /// </summary>
        private const int FindFileTimeoutMs = 10000;

        /// <summary>
        /// If data file is older than this number of days then a warning will be displayed.
        /// </summary>
        public const int DataFileAgeWarning = 30;

        /// <summary>
        /// Find the specified filename within the specified directory.
        /// If no directory is specified, the working directory is used.
        /// If the file cannot be found, the algorithm will move to the 
        /// parent directory and repeat the process.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string FindFile(
            string filename)
        {
            var cancel = new CancellationTokenSource();
            // Start the file system search as a separate task.
            var searchTask = Task.Run(() => FindFile(filename, cancel.Token));
            // Wait for either the search or a timeout task to complete.
            Task.WaitAny(searchTask, Task.Delay(FindFileTimeoutMs));
            cancel.Cancel();
            // If search has not got a result then return null.
            return searchTask.IsCompleted ? searchTask.Result : null;
        }

        private static string FindFile(
            string filename,
            CancellationToken cancel,
            DirectoryInfo dir = null)
        {
            if (dir == null)
            {
                dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            }
            string result = null;

            var files = dir.GetFiles(filename, SearchOption.AllDirectories);

            if (files.Length == 0 &&
                dir.Parent != null &&
                cancel.IsCancellationRequested == false)
            {
                result = FindFile(filename, cancel, dir.Parent);
            }
            else if (files.Length > 0)
            {
                result = files[0].FullName;
            }
            return result;
        }

        /// <summary>
        /// Display information about the data file and log warnings if specific requirements
        /// are not met.
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="logger"></param>
        public static void CheckDataFile(IPipeline pipeline, ILogger logger)
        {
            // Get the 'engine' element within the pipeline that performs device detection.
            // We can use this to get details about the data file as well as meta-data describing
            // things such as the available properties.
            var engine = pipeline.GetElement<DeviceDetectionHashEngine>();
            CheckDataFile(engine, logger);
        }

        /// <summary>
        /// Display information about the data file and log warnings if specific requirements
        /// are not met.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="logger"></param>
        public static void CheckDataFile(DeviceDetectionHashEngine engine, ILogger logger)
        {
            if (engine != null)
            {
                var dataFileDate = engine.DataFiles[0]
                    .DataPublishedDateTime;
                logger.LogInformation($"Using a '{engine.DataSourceTier}' data file created " +
                    $"'{dataFileDate}' from location '{engine.DataFiles[0].DataFilePath}'");

                if (DateTime.UtcNow > dataFileDate.AddDays(
                    DataFileAgeWarning))
                {
                    logger.LogWarning($"This example is using a data file that is more than " +
                        $"{DataFileAgeWarning} days old. A more recent data file may be needed " +
                        $"to correctly detect the latest devices, browsers, etc. The latest lite " +
                        $"data file is available from the device-detection-data repository on " +
                        $"GitHub https://github.com/51Degrees/device-detection-data. Find out " +
                        $"about the Enterprise data file, which includes automatic daily " +
                        $"updates, on our pricing page: https://51degrees.com/pricing");
                }
                if (engine.DataSourceTier == "Lite")
                {
                    logger.LogWarning($"This example is using the 'Lite' data file. This is " +
                        $"used for illustration, and has limited accuracy and capabilities. " +
                        $"Find out about the Enterprise data file on our pricing page: " +
                        $"https://51degrees.com/pricing");
                }
            }
        }
    }
}
