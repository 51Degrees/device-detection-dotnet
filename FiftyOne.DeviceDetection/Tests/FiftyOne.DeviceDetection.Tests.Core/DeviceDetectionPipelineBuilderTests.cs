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

using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.Tests.Core
{
    [TestClass]
    public class DeviceDetectionPipelineBuilderTests
    {

        /// <summary>
        /// Test that certain configuration options passed to the builder
        /// are actually set in the final engine.
        /// </summary>
        /// <param name="datafilename">
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
        [DataRow(TestHelpers.Constants.PATTERN_DATA_FILE_NAME, true, false, null)]
        [DataRow(TestHelpers.Constants.PATTERN_DATA_FILE_NAME, false, false, null)]
        [DataRow(TestHelpers.Constants.PATTERN_DATA_FILE_NAME, true, true, null)]
        [DataRow(TestHelpers.Constants.PATTERN_DATA_FILE_NAME, false, true, null)]
        [DataRow(TestHelpers.Constants.PATTERN_DATA_FILE_NAME, true, false, "key1")]
        [DataRow(TestHelpers.Constants.PATTERN_DATA_FILE_NAME, true, true, "key1")]
        public void DeviceDetectionPipelineBuilder_CheckConfiguration(
            string datafilename, bool autoUpdate, bool shareUsage, string licenseKey)
        {
            var datafile = Utils.GetFilePath(datafilename);
            var updateService = new Mock<IDataUpdateService>();

            // Configure the pipeline.
            var pipeline = new DeviceDetectionPipelineBuilder(
                new NullLoggerFactory(), null, updateService.Object)
                .UseOnPremise(datafile.FullName, false)
                .SetAutoUpdate(autoUpdate)
                .SetShareUsage(shareUsage)
                .SetDataUpdateLicenseKey(licenseKey)
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
                e => e.GetType() == typeof(DeviceDetectionPatternEngine)));

            // Get the device detection engine element and check that it has
            // been configured as specified.
            var engine = pipeline.FlowElements.Single(
                e => e.GetType() == typeof(DeviceDetectionPatternEngine)) as DeviceDetectionPatternEngine;

            Assert.AreEqual(autoUpdate, engine.DataFiles[0].AutomaticUpdatesEnabled);
            if(licenseKey != null)
            {
                Assert.AreEqual(licenseKey, engine.DataFiles[0].Configuration.DataUpdateLicenseKeys[0]);
            }

        }
    }
}
