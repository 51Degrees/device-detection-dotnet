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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace FiftyOne.DeviceDetection.Tests.Framework
{
    [TestClass]
    public class DeviceDetectionPipelineBuilderTests
    {

        /// <summary>
        /// Test that certain configuration options passed to the builder
        /// are actually set in the final engine.
        /// </summary>
        /// <param name="dataFilename">
        /// The complete path to the data file to use.
        /// </param>
        /// <param name="autoUpdate">
        /// True to enable auto update, false to disable.
        /// </param>
        /// <param name="shareUsage">
        /// True to enable share usage, false to disable.
        /// </param>
        /// <param name="licenseKey">
        /// The license key to use when performing automatic update.
        /// </param>
        [DataTestMethod]
        [DataRow(Constants.LITE_HASH_DATA_FILE_NAME, true, false, null)]
        [DataRow(Constants.LITE_HASH_DATA_FILE_NAME, false, false, null)]
        [DataRow(Constants.LITE_HASH_DATA_FILE_NAME, true, true, null)]
        [DataRow(Constants.LITE_HASH_DATA_FILE_NAME, false, true, null)]
        [DataRow(Constants.LITE_HASH_DATA_FILE_NAME, true, false, "key1")]
        [DataRow(Constants.LITE_HASH_DATA_FILE_NAME, true, true, "key1")]
        public void DeviceDetectionPipelineBuilder_CheckConfiguration(
            string dataFilename, bool autoUpdate, bool shareUsage, string licenseKey)
        {
            var datafile = Utils.GetFilePath(dataFilename);
            var updateService = new Mock<IDataUpdateService>();

            // Configure the pipeline.
            var pipeline = new DeviceDetectionPipelineBuilder(
                new NullLoggerFactory(), null, updateService.Object)
                .UseOnPremise(datafile.FullName, licenseKey, false)
                .SetAutoUpdate(autoUpdate)
                .SetShareUsage(shareUsage)
                .Build();

            // Check that the flow elements in the pipeline are as expected.
            if (shareUsage)
            {
                Assert.AreEqual(2, pipeline.FlowElements.Count);
                Assert.IsTrue(pipeline.FlowElements.Any(
                    e => e.GetType() == typeof(ShareUsageElement)));
            }
            else
            {
                Assert.AreEqual(1, pipeline.FlowElements.Count);
            }
            Assert.IsTrue(pipeline.FlowElements.Any(
                e => e.GetType() == typeof(DeviceDetectionHashEngine)));

            // Get the device detection engine element and check that it has
            // been configured as specified.
            var engine = pipeline.FlowElements.Single(
                e => e.GetType() == typeof(DeviceDetectionHashEngine)) as DeviceDetectionHashEngine;

            if (licenseKey != null)
            {
                Assert.AreEqual(autoUpdate, engine.DataFiles[0].AutomaticUpdatesEnabled);
                Assert.AreEqual(licenseKey, engine.DataFiles[0].Configuration.DataUpdateLicenseKeys[0]);
            }
            else
            {
                // If there is no license key configured then automatic updates will be
                // disabled.
                Assert.AreEqual(false, engine.DataFiles[0].AutomaticUpdatesEnabled);
            }

        }
    }
}
