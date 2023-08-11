/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2023 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.Pipeline.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.TestHelpers.Data
{
    public class MetaDataTests
    {
        // The delay in milliseconds between calls to refresh the data
        // used by the engine.
        // The LowMemory and Balanced profiles are significantly 
        // slower so we use a longer delay.
        private static int ReloadDelay(
            PerformanceProfiles profile, 
            bool inmemory)
        {
            if (inmemory) return 800;
            switch (profile)
            {
                case PerformanceProfiles.MaxPerformance:
                case PerformanceProfiles.HighPerformance:
                    return 800;
                case PerformanceProfiles.LowMemory:
                case PerformanceProfiles.Balanced:
                case PerformanceProfiles.BalancedTemp:
                    return 1800;
                default:
                    return 800;
            }
        }
        // Each hash task will delay it's start by x times this value 
        // in ms.
        // Where x = the task sequence number (i.e. first task x = 0, 
        // second task x = 1, etc).
        // Therefore the last task will start after 
        // (HashTaskDelay - 1) * HashTaskDelay milliseconds
        private static int HashTaskDelay(bool inmemory)
        {
            if (inmemory) return 500;
            else return 500;
        }

        private static int RefreshLimit()
        {
            if(IntPtr.Size == 4)
            {
                return 1;
            }
            return 8;
        }

        // The number of simultaneous hashing tasks to start.
        private const int HashTaskCount = 6;
        // Number of hashing tasks currently running
        private int _hashTasksActive = 0;



        /// <summary>
        /// Used to return results from 'hashing' tasks
        /// </summary>
        private class HashTaskResult
        {
            public int HashValue { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime FinishTime { get; set; }
        }
        /// <summary>
        /// Used to return results from the refresh task
        /// </summary>
        private class RefreshResult
        {
            public DateTime StartTime { get; set; }
            public DateTime FinishTime { get; set; }
        }
        

        public void Reload(IWrapper wrapper, 
            IMetaDataHasher hasher, 
            PerformanceProfiles profile)
        {
            CancellationTokenSource cancelRefresh = new CancellationTokenSource();
            var reloader = Refresh(wrapper, cancelRefresh.Token, profile);

            Reload(wrapper, hasher, reloader, cancelRefresh, false);
        }

        public void ReloadMemory(IWrapper wrapper, 
            IMetaDataHasher hasher,
            PerformanceProfiles profile)
        {
            var masterData = File.ReadAllBytes(
                wrapper.GetEngine().GetDataFileMetaData().DataFilePath);
            CancellationTokenSource cancelRefresh = new CancellationTokenSource();
            var reloader = RefreshInMemory(wrapper, masterData, cancelRefresh.Token, profile);

            Reload(wrapper, hasher, reloader, cancelRefresh, true);
        }

        private void Reload(IWrapper wrapper, 
            IMetaDataHasher hasher, 
            Task<List<RefreshResult>> reloader,
            CancellationTokenSource cancelRefresh,
            bool inMemory)
        {
            IList<Task<HashTaskResult>> tasks = StartHashingTasks(
                HashTaskCount,
                inMemory,
                wrapper,
                hasher);
            HashTaskResult[] hashes = Task.WhenAll(tasks).Result;

            cancelRefresh.Cancel();
            var refreshResults = reloader.Result;

            Console.WriteLine($"Refreshed the dataset {refreshResults.Count} times.");
            ValidateResults(HashTaskCount, hashes, refreshResults);
        }

        private static void ValidateResults(
            int taskCount,
            HashTaskResult[] hashes,
            List<RefreshResult> refreshResults)
        {

            for (int i = 0; i < taskCount - 1; i++)
            {
                Assert.AreEqual(hashes[i].HashValue,
                    hashes[i + 1].HashValue,
                    "Hashes were not equal");
            }
            var avgHashTime = hashes
                .Average(h => (h.FinishTime - h.StartTime).TotalMilliseconds);
            var avgRefreshTime = refreshResults
                .Average(h => (h.FinishTime - h.StartTime).TotalMilliseconds);

            var refreshesDuringActiveHashTasks = refreshResults.Where(r =>
                hashes.Any(h => (r.StartTime > h.StartTime &&
                    r.StartTime < h.FinishTime) ||
                    (r.FinishTime > h.StartTime &&
                    r.FinishTime < h.FinishTime)));
            Console.WriteLine($"{refreshesDuringActiveHashTasks.Count()} " +
                $"refreshes during active hash tasks");
            Assert.IsTrue(refreshesDuringActiveHashTasks.Count() > 0,
                "At least 1 refresh needs to occur while hash tasks are " +
                "active in order for this test to be valid. Check the " +
                "ReloadDelay and HashTaskDelay settings. " +
                $"Average hash task time = {avgHashTime} ms. " +
                $"Average refresh task time = {avgRefreshTime} ms.");
        }

        private IList<Task<HashTaskResult>> StartHashingTasks(
            int taskCount,
            bool inMemory,
            IWrapper wrapper,
            IMetaDataHasher hasher)
        {
            var tasks = new List<Task<HashTaskResult>>();
            while (tasks.Count < taskCount)
            {
                tasks.Add(HashData(tasks.Count * HashTaskDelay(inMemory), inMemory, wrapper, hasher));
            }
            return tasks;
        }

        private async Task<HashTaskResult> HashData(
            int delayMs,
            bool inMemory,
            IWrapper wrapper,
            IMetaDataHasher hasher)
        {
            DateTime start = DateTime.UtcNow;

            await Task.Delay(delayMs);

            int taskId = delayMs / HashTaskDelay(inMemory);
            Console.WriteLine($"Hash task '{taskId}' started.");

            Interlocked.Increment(ref _hashTasksActive);
            int hash = 0;

            try
            {
                hash = hasher.HashProperties(hash, wrapper);
                hash = hasher.HashValues(hash, wrapper);
                hash = hasher.HashComponents(hash, wrapper);
                hash = hasher.HashProfiles(hash, wrapper);
            }
            finally
            {
                Interlocked.Decrement(ref _hashTasksActive);
            }

            Console.WriteLine($"Hash task '{taskId}' finished with hash value '{hash}'.");

            return new HashTaskResult()
            {
                HashValue = hash,
                StartTime = start,
                FinishTime = DateTime.UtcNow
            };
        }

        private async Task<List<RefreshResult>> Refresh(
            IWrapper wrapper,
            CancellationToken cancellationToken,
            PerformanceProfiles profile)
        {
            List<RefreshResult> result = new List<RefreshResult>();
            while (cancellationToken.IsCancellationRequested == false &&
                result.Count < RefreshLimit())
            {
                if (_hashTasksActive == 0)
                {
                    // No hash tasks active so just wait and 
                    // then try again.
                    await Task.Delay(10);
                }
                else
                {
                    DateTime start = DateTime.UtcNow;
                    Console.WriteLine("Refresh started");
                    wrapper.GetEngine().RefreshData(null);
                    Console.WriteLine("Refresh completed");
                    result.Add(new RefreshResult()
                    {
                        StartTime = start,
                        FinishTime = DateTime.UtcNow
                    });
                    await Task.Delay(ReloadDelay(profile, false));
                }
            }

            return result;
        }

        private async Task<List<RefreshResult>> RefreshInMemory(
            IWrapper wrapper,
            byte[] masterData,
            CancellationToken cancellationToken,
            PerformanceProfiles profile)
        {
            List<RefreshResult> result = new List<RefreshResult>();

            while (cancellationToken.IsCancellationRequested == false &&
                result.Count < RefreshLimit())
            {
                if (_hashTasksActive == 0)
                {
                    // No hash tasks active so just wait and 
                    // then try again.
                    await Task.Delay(10);
                }
                else
                {
                    using (var stream = new MemoryStream())
                    {
                        stream.Write(masterData, 0, masterData.Length);
                        stream.Seek(0, SeekOrigin.Begin);

                        DateTime start = DateTime.UtcNow;
                        Console.WriteLine("Refresh started");
                        wrapper.GetEngine().RefreshData(
                            wrapper.GetEngine().DataFiles[0].Identifier,
                            stream);
                        Console.WriteLine("Refresh completed");
                        result.Add(new RefreshResult()
                        {
                            StartTime = start,
                            FinishTime = DateTime.UtcNow
                        });
                    }
                    await Task.Delay(ReloadDelay(profile, true));
                }
            }

            return result;
        }
    }
}
