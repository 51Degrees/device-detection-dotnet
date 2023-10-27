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
using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Configuration;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using static FiftyOne.DeviceDetection.Tests.Core.DeviceDetectionPipelineBuilderTests;

namespace FiftyOne.DeviceDetection.Tests.Core
{
    [TestClass]
    public class DeviceDetectionPipelineBuilderTests
    {
        #region Records 

        public record SetupAction(
            string Description,
            Func<DeviceDetectionOnPremisePipelineBuilder, DeviceDetectionOnPremisePipelineBuilder> Setup);

        public record PipelineDecomp(
            IPipeline Pipeline,
            DeviceDetectionHashEngine Engine,
            IAspectEngineDataFile DataFile,
            IDataFileConfiguration DataFileConfig
            );

        public record ValidationAction(
            string Description,
            Action<PipelineDecomp> Validate);

        public record TestFragment(
            IList<SetupAction> SetupActions,
            ValidationAction ValidationAction
            )
        {
            public TestFragment(
                string desc,
                Func<DeviceDetectionOnPremisePipelineBuilder, DeviceDetectionOnPremisePipelineBuilder> setupCall,
                Action<PipelineDecomp> validationCall) : this(
                    (setupCall is not null) 
                    ? new SetupAction[] { new SetupAction(desc, setupCall) }
                    : new SetupAction[0], 
                    new ValidationAction(desc, validationCall))
            { }
        }

        public record TestParamsComplete(
            string DataFileName,
            string LicenseKey,
            IList<SetupAction> SetupActions,
            IList<ValidationAction> ValidationActions);

        #endregion


        #region AutoUpdate

        private static TestFragment AutoUpdateTestFragment(bool autoUpdate, string licenseKey) =>
            new TestFragment(
                $"{nameof(DeviceDetectionOnPremisePipelineBuilder.SetAutoUpdate)}({autoUpdate})",
                b => b.SetAutoUpdate(autoUpdate),
                (pd) => Assert.AreEqual(autoUpdate && (licenseKey is not null), pd.DataFile.AutomaticUpdatesEnabled));

        #endregion


        #region LicenseKey

        private static IEnumerable<string> PossibleLicenseKeys(bool autoUpdate)
        {
            yield return null;
            if (autoUpdate) { yield return "key1"; }
        }
        private static TestFragment TestFragmentForLicenseKey(string licenseKey) => new TestFragment(
            $"{nameof(DeviceDetectionPipelineBuilder.UseOnPremise)}(licenseKey='{licenseKey}')",
            null,
            (licenseKey is not null) 
            ? pd => Assert.AreEqual(licenseKey, pd.DataFileConfig.DataUpdateLicenseKeys[0])
            : pd => Assert.AreEqual(0, pd.DataFileConfig.DataUpdateLicenseKeys.Count));

        #endregion


        #region ShareUsage

        private static IEnumerable<bool> PossibleShareUsageFlags => new bool[] { true, false };
        private static TestFragment ShareUsageTestFragment(bool shareUsage) => new TestFragment(
            $"{nameof(DeviceDetectionOnPremisePipelineBuilder.SetShareUsage)}({shareUsage})",
            b => b.SetShareUsage(shareUsage),
            shareUsage
            ? (pd => {
                Assert.AreEqual(2, pd.Pipeline.FlowElements.Count);
                Assert.IsTrue(pd.Pipeline.FlowElements.Any(
                    e => e.GetType() == typeof(ShareUsageElement)));
            }) 
            : (pd => Assert.AreEqual(1, pd.Pipeline.FlowElements.Count)));

        #endregion


        #region XXXXX

        #endregion


        #region FragmentsMerge

        private static IEnumerable<IList<TestFragment>> TestFragmentVariantsForAutoUpdate(bool autoUpdate, string licenseKey) {
            yield return new TestFragment[] { AutoUpdateTestFragment(autoUpdate, licenseKey) };
            yield return new TestFragment[] { TestFragmentForLicenseKey(licenseKey) };
            yield return PossibleShareUsageFlags.Select(ShareUsageTestFragment).ToList();
        }

        private static IEnumerable<TestFragment> PickComboFromVariants(IList<IList<TestFragment>> variants, long comboIndex)
        {
            long remainingIndex = comboIndex;
            foreach (var variantList in variants)
            {
                var currentIndex = remainingIndex % variantList.Count;
                remainingIndex /= variantList.Count;
                yield return variantList[(int)currentIndex];
            }
        }

        private static IEnumerable<IEnumerable<TestFragment>> TestFragmentCombosForAutoUpdate(bool autoUpdate, string licenseKey)
        {
            var variants = TestFragmentVariantsForAutoUpdate(autoUpdate, licenseKey).ToList();
            long combosCount = variants.Aggregate(1, (c, l) => c * l.Count);
            for (long i = 0; i < combosCount; ++i)
            {
                yield return PickComboFromVariants(variants, i);
            }
        }

        private static IEnumerable<TestParamsComplete> AllTestParamsRaw(string dataFileName)
        {
            foreach(var autoUpdate in new bool[] { false, true })
            {
                foreach(var licenseKey in PossibleLicenseKeys(autoUpdate))
                {
                    foreach(var fragmentsCombo in TestFragmentCombosForAutoUpdate(autoUpdate, licenseKey))
                    {
                        yield return new TestParamsComplete(
                            dataFileName,
                            licenseKey,
                            fragmentsCombo.SelectMany(f => f.SetupActions).ToList(),
                            fragmentsCombo.Select(f => f.ValidationAction).ToList());
                    }
                }
            }
        }

        private static IEnumerable<object[]> AllTestParams
            => AllTestParamsRaw(Constants.LITE_HASH_DATA_FILE_NAME).Select(t => new object[] { t });

        #endregion


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
        [DynamicData(nameof(AllTestParams))]
        public void DeviceDetectionPipelineBuilder_CheckConfiguration(TestParamsComplete testParams)
        {
            var datafile = Utils.GetFilePath(testParams.DataFileName);
            var updateService = new Mock<IDataUpdateService>();

            Console.WriteLine(
                $"Preparing base builder with ("
                + $"{nameof(testParams.DataFileName)} = {testParams.DataFileName}, "
                + $"{nameof(testParams.LicenseKey)} = {testParams.LicenseKey})");

            // Configure the pipeline.
            var pipelineBuilder = new DeviceDetectionPipelineBuilder(
                new NullLoggerFactory(), null, updateService.Object)
                .UseOnPremise(datafile.FullName, testParams.LicenseKey, false);

            foreach(var setupAction in testParams.SetupActions)
            {
                Console.WriteLine($"Setting up: {setupAction.Description}");
                pipelineBuilder = setupAction.Setup(pipelineBuilder);
            }

            Console.WriteLine("Building...");

            // Build the pipeline
            using var pipeline = pipelineBuilder.Build();


            Console.WriteLine("Building finished!");

            Assert.IsTrue(pipeline.FlowElements.Any(
                e => e.GetType() == typeof(DeviceDetectionHashEngine)));

            // Get the device detection engine element and check that it has
            // been configured as specified.
            var engine = pipeline.FlowElements.Single(
                e => e.GetType() == typeof(DeviceDetectionHashEngine)) as DeviceDetectionHashEngine;

            var dataFile = engine.DataFiles[0];
            var dataFileConfig = dataFile.Configuration;

            var pipelineDecomposition = new PipelineDecomp(
                pipeline,
                engine,
                dataFile,
                dataFileConfig);

            // Validate expectations
            foreach(var validationAction in testParams.ValidationActions)
            {
                Console.WriteLine($"Validating: {validationAction.Description}");
                validationAction.Validate(pipelineDecomposition);
            }
        }
    }
}
