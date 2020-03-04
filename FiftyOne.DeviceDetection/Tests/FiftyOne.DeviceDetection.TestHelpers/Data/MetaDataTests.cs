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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.TestHelpers.Data
{
    public class MetaDataTests
    {
        private static IList<Task<int>> StartHashingThreads(
            int threadCount,
            IWrapper wrapper,
            IMetaDataHasher hasher)
        {
            var tasks = new List<Task<int>>();
            while (tasks.Count < threadCount)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await Task.Delay(tasks.Count * 80);
                    int hash = 0;
                    hash = hasher.HashProperties(hash, wrapper);
                    hash = hasher.HashValues(hash, wrapper);
                    hash = hasher.HashComponents(hash, wrapper);
                    hash = hasher.HashProfiles(hash, wrapper);

                    Console.WriteLine($"Thread finished with hash value '{hash}'.");

                    return hash;
                }));
            }
            return tasks;
        }

        public static void Reload(IWrapper wrapper, IMetaDataHasher hasher)
        {
            int threadCount = 6;
            int refreshes = 0;
            bool done = false;
            var reloader = Task.Run(async () =>
            {
                while (done == false)
                {
                    Console.WriteLine("Refresh");
                    wrapper.GetEngine().RefreshData(null);
                    refreshes++;
                    await Task.Delay(200);
                }
            });
            IList<Task<int>> tasks = StartHashingThreads(
                threadCount,
                wrapper,
                hasher);
            int[] hashes = Task.WhenAll(tasks).Result;
            done = true;
            reloader.Wait();
            Console.WriteLine($"Refreshed the dataset {refreshes} times.");
            for (int i = 0; i < threadCount - 1; i++)
            {
                Assert.AreEqual(hashes[i], hashes[i + 1], "Hashes were not equal");
            }

        }

        public static void ReloadMemory(IWrapper wrapper, IMetaDataHasher hasher)
        {
            var masterData = File.ReadAllBytes(
                wrapper.GetEngine().GetDataFileMetaData().DataFilePath);

            int threadCount = 6;
            int refreshes = 0;
            bool done = false;
            var reloader = Task.Run(async () =>
            {
                while (done == false)
                {
                    using (var stream = new MemoryStream())
                    {
                        stream.Write(masterData, 0, masterData.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        Console.WriteLine("Refresh");
                        wrapper.GetEngine().RefreshData(
                            wrapper.GetEngine().DataFiles[0].Identifier,
                            stream);
                    }
                    refreshes++;
                    await Task.Delay(200);
                }
            });
            IList<Task<int>> tasks = StartHashingThreads(
                threadCount,
                wrapper,
                hasher);
            int[] hashes = Task.WhenAll(tasks).Result;
            done = true;
            reloader.Wait();
            Console.WriteLine($"Refreshed the dataset {refreshes} times.");
            for (int i = 0; i < threadCount - 1; i++)
            {
                Assert.AreEqual(hashes[i], hashes[i + 1], "Hashes were not equal");
            }

        }
    }
}
