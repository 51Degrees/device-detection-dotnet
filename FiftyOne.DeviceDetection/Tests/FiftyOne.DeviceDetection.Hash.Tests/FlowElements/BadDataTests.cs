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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers;
using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FiftyOne.DeviceDetection.Hash.Tests.FlowElements
{
    [TestClass]
    public class BadDataTests
    {
        ILoggerFactory LoggerFactory = new LoggerFactory();

        private int[] badVersion = new int[] { 1, 2, 3, 4 };
        private int[] goodVersion = new int[] { 4, 1, 0, 0 };

        private string BadVersionDataFile = Environment.CurrentDirectory + "/BadVersionDataFile.hash";
        private string BadHeaderDataFile = Environment.CurrentDirectory + "/BadHeaderDataFile.hash";
        private string SmallDataFile = Environment.CurrentDirectory + "/SmallDataFile.hash";

        private const int sizeOfHeader = 200;

        [TestInitialize]
        public void Init()
        {
            var nullHeader = new byte[sizeOfHeader];
            for (int i = 0; i < sizeOfHeader; i++)
            {
                nullHeader[i] = 12;
            }

            using (var file = File.Create(BadVersionDataFile))
            using (var writer = new BinaryWriter(file))
            {
                foreach (var i in badVersion)
                {
                    writer.Write(i);
                }
                writer.Write(nullHeader);
            }

            using (var file = File.Create(BadHeaderDataFile))
            using (var writer = new BinaryWriter(file))
            {
                foreach (var i in goodVersion)
                {
                    writer.Write(i);
                }
                writer.Write(nullHeader);
            }

            using (var file = File.Create(SmallDataFile))
            using (var writer = new BinaryWriter(file))
            {
                writer.Write((byte)0);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            File.Delete(BadVersionDataFile);
            File.Delete(BadHeaderDataFile);
            File.Delete(SmallDataFile);
        }

        [TestMethod]
        public void BadData_VersionNumber_File()
        {
            try
            {
                _ = new DeviceDetectionHashEngineBuilder(LoggerFactory)
                    .SetAutoUpdate(false)
                    .SetDataFileSystemWatcher(false)
                    .SetDataUpdateOnStartup(false)
                    .SetDataUpdateLicenseKey(null)
                    .Build(BadVersionDataFile, false);
                Assert.Fail("No exception was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(
                    "The data is an unsupported version. Check you have the latest data and API.",
                    e.Message);
            }
        }

        [TestMethod]
        public void BadData_Header_File()
        {
            try
            {
                _ = new DeviceDetectionHashEngineBuilder(LoggerFactory)
                    .SetAutoUpdate(false)
                    .SetDataFileSystemWatcher(false)
                    .SetDataUpdateOnStartup(false)
                    .SetDataUpdateLicenseKey(null)
                    .Build(BadHeaderDataFile, false);
                Assert.Fail("No exception was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(
                    "The data was not in the correct format. Check the data file is uncompressed.",
                    e.Message);
            }
        }

        [TestMethod]
        public void BadData_SmallData_File()
        {
            try
            {
                _ = new DeviceDetectionHashEngineBuilder(LoggerFactory)
                    .SetAutoUpdate(false)
                    .SetDataFileSystemWatcher(false)
                    .SetDataUpdateOnStartup(false)
                    .SetDataUpdateLicenseKey(null)
                    .Build(SmallDataFile, false);
                Assert.Fail("No exception was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(
                    "The data was not in the correct format. Check the data file is uncompressed.",
                    e.Message);
            }
        }

        [TestMethod]
        public void BadData_VersionNumber_Memory()
        {
            try
            {
                var data = File.ReadAllBytes(BadVersionDataFile);
                using (var stream = new MemoryStream(data)) {
                    _ = new DeviceDetectionHashEngineBuilder(LoggerFactory)
                        .SetAutoUpdate(false)
                        .SetDataFileSystemWatcher(false)
                        .SetDataUpdateOnStartup(false)
                        .SetDataUpdateLicenseKey(null)
                        .Build(stream);
                }
                Assert.Fail("No exception was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(
                    "The data is an unsupported version. Check you have the latest data and API.",
                    e.Message);
            }
        }

        [TestMethod]
        public void BadData_Header_Memory()
        {
            try
            {
                var data = File.ReadAllBytes(BadHeaderDataFile);
                using (var stream = new MemoryStream(data))
                {
                    _ = new DeviceDetectionHashEngineBuilder(LoggerFactory)
                        .SetAutoUpdate(false)
                        .SetDataFileSystemWatcher(false)
                        .SetDataUpdateOnStartup(false)
                        .SetDataUpdateLicenseKey(null)
                        .Build(stream);
                }
                Assert.Fail("No exception was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(
                    "The data was not in the correct format. Check the data file is uncompressed.",
                    e.Message);
            }
        }

        [TestMethod]
        public void BadData_SmallData_Memory()
        {
            try
            {
                var data = File.ReadAllBytes(SmallDataFile);
                using (var stream = new MemoryStream(data))
                {
                    _ = new DeviceDetectionHashEngineBuilder(LoggerFactory)
                        .SetAutoUpdate(false)
                        .SetDataFileSystemWatcher(false)
                        .SetDataUpdateOnStartup(false)
                        .SetDataUpdateLicenseKey(null)
                        .Build(stream);
                }
                Assert.Fail("No exception was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual(
                    "The data was not in the correct format. Check the data file is uncompressed.",
                    e.Message);
            }
        }
    }
}
