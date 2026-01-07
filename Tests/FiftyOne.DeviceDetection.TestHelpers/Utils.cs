/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2026 51 Degrees Mobile Experts Limited, Davidson House,
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.TestHelpers
{
    public static class Utils
    {
        /// <summary>
        /// Timeout used when searching for files.
        /// </summary>
        private const int FindFileTimeoutMs = 10000;

        /// <summary>
        /// The folder that contains the C++ and therefore device data folder.
        /// </summary>
        private const string OnPremiseDirectory = 
            "FiftyOne.DeviceDetection.Hash.Engine.OnPremise";

        /// <summary>
        /// Cache of file names to file infos to speed up tests.
        /// </summary>
        private static readonly ConcurrentDictionary<string, FileInfo> Cache = 
            new ConcurrentDictionary<string, FileInfo>();

        public static FileInfo GetFilePath(string filename)
        {
            var fullPath = Cache.GetOrAdd(
                filename,
                (f) =>
                {
                    var p = FindFile(filename, GetOnPremiseDirectory());
                    return p == null ? null : new FileInfo(p);
                });
            if (fullPath == null || fullPath.Exists == false)
            {
                Assert.Inconclusive($"Expected data file " +
                    $"'{filename}' was missing. Test not run.");
            }
            return fullPath;
        }

        /// <summary>
        /// Finds the on premise directory where the test data files are
        /// expected to be located.
        /// </summary>
        /// <returns></returns>
        private static DirectoryInfo GetOnPremiseDirectory()
        {
            var current = new DirectoryInfo(Environment.CurrentDirectory);
            while (current != null)
            {
                var onPremise = current.GetDirectories(
                    OnPremiseDirectory,
                    SearchOption.TopDirectoryOnly);
                if (onPremise.Length == 1)
                {
                    return onPremise[0];
                }
                current = current.Parent;
            }
            throw new DirectoryNotFoundException(OnPremiseDirectory);
        }

        /// <summary>
        /// Uses a background task to search for the specified filename within the working 
        /// directory.
        /// If the file cannot be found, the algorithm will move to the parent directory and 
        /// repeat the process.
        /// This continues until the file is found or a timeout is triggered.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dir">
        /// The directory to start looking from.
        /// </param>
        /// <returns></returns>
        private static string FindFile(string filename, DirectoryInfo dir)
        {
            var cancel = new CancellationTokenSource();
            // Start the file system search as a separate task.
            var searchTask = Task.Run(() => FindFile(filename, dir, cancel.Token));
            // Wait for either the search or a timeout task to complete.
            Task.WaitAny(searchTask, Task.Delay(FindFileTimeoutMs));
            cancel.Cancel();
            // If search has not got a result then return null.
            return searchTask.IsCompleted ? searchTask.Result : null;
        }

        private static string FindFile(
            string filename,
            DirectoryInfo dir,
            CancellationToken cancel)
        {
            string result = null;
            try
            {
                var files = dir.GetFiles(filename, SearchOption.AllDirectories);
                if (files.Length == 0 &&
                    dir.Parent != null &&
                    cancel.IsCancellationRequested == false)
                {
                    result = FindFile(filename, dir.Parent, cancel);
                }
                else if (files.Length > 0)
                {
                    result = files[0].FullName;
                }
            }
            // No matter what goes wrong here, we just want to indicate that we
            // couldn't find the file by returning null.
            catch { result = null; }

            return result;
        }
    }
}
