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

using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Constants = FiftyOne.DeviceDetection.TestHelpers.Constants;

namespace FiftyOne.DeviceDetection.Tests.Core
{
    [TestClass]
    public class DeviceDetectionTests
    {
        private static UserAgentGenerator USER_AGENTS = new UserAgentGenerator(
            Utils.GetFilePath(Constants.UA_FILE_NAME));

        [DataTestMethod]
        // ******** Hash with a single thread *********
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.MaxPerformance, false, false, false, DisplayName = "Hash-MaxPerformance-NoCache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.HighPerformance, false, false, false, DisplayName = "Hash-HighPerformance-NoCache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.LowMemory, false, false, false, DisplayName = "Hash-LowMemory-NoCache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.Balanced, false, false, false, DisplayName = "Hash-Balanced-NoCache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.BalancedTemp, false, false, false, DisplayName = "Hash-BalancedTemp-NoCache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.MaxPerformance, true, false, false, DisplayName = "Hash-MaxPerformance-Cache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.HighPerformance, true, false, false, DisplayName = "Hash-HighPerformance-Cache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.LowMemory, true, false, false, DisplayName = "Hash-LowMemory-Cache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.Balanced, true, false, false, DisplayName = "Hash-Balanced-Cache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.BalancedTemp, true, false, false, DisplayName = "Hash-BalancedTemp-Cache-NoLazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.MaxPerformance, false, true, false, DisplayName = "Hash-MaxPerformance-NoCache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.HighPerformance, false, true, false, DisplayName = "Hash-HighPerformance-NoCache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.LowMemory, false, true, false, DisplayName = "Hash-LowMemory-NoCache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.Balanced, false, true, false, DisplayName = "Hash-Balanced-NoCache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.BalancedTemp, false, true, false, DisplayName = "Hash-BalancedTemp-NoCache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.MaxPerformance, true, true, false, DisplayName = "Hash-MaxPerformance-Cache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.HighPerformance, true, true, false, DisplayName = "Hash-HighPerformance-Cache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.LowMemory, true, true, false, DisplayName = "Hash-LowMemory-Cache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.Balanced, true, true, false, DisplayName = "Hash-Balanced-Cache-LazyLoad-SingleThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.BalancedTemp, true, true, false, DisplayName = "Hash-BalancedTemp-Cache-LazyLoad-SingleThread")]
        // ******** Hash with multiple threads *********
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.MaxPerformance, false, false, true, DisplayName = "Hash-MaxPerformance-NoCache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.HighPerformance, false, false, true, DisplayName = "Hash-HighPerformance-NoCache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.LowMemory, false, false, true, DisplayName = "Hash-LowMemory-NoCache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.Balanced, false, false, true, DisplayName = "Hash-Balanced-NoCache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.BalancedTemp, false, false, true, DisplayName = "Hash-BalancedTemp-NoCache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.MaxPerformance, true, false, true, DisplayName = "Hash-MaxPerformance-Cache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.HighPerformance, true, false, true, DisplayName = "Hash-HighPerformance-Cache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.LowMemory, true, false, true, DisplayName = "Hash-LowMemory-Cache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.Balanced, true, false, true, DisplayName = "Hash-Balanced-Cache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.BalancedTemp, true, false, true, DisplayName = "Hash-BalancedTemp-Cache-NoLazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.MaxPerformance, false, true, true, DisplayName = "Hash-MaxPerformance-NoCache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.HighPerformance, false, true, true, DisplayName = "Hash-HighPerformance-NoCache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.LowMemory, false, true, true, DisplayName = "Hash-LowMemory-NoCache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.Balanced, false, true, true, DisplayName = "Hash-Balanced-NoCache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.BalancedTemp, false, true, true, DisplayName = "Hash-BalancedTemp-NoCache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.MaxPerformance, true, true, true, DisplayName = "Hash-MaxPerformance-Cache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.HighPerformance, true, true, true, DisplayName = "Hash-HighPerformance-Cache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.LowMemory, true, true, true, DisplayName = "Hash-LowMemory-Cache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.Balanced, true, true, true, DisplayName = "Hash-Balanced-Cache-LazyLoad-MultiThread")]
        [DataRow(Constants.HASH_DATA_FILE_NAME, PerformanceProfiles.BalancedTemp, true, true, true, DisplayName = "Hash-BalancedTemp-Cache-LazyLoad-MultiThread")]
        public void Hash_AllConfigurations_100_UserAgents(
            string datafileName,
            PerformanceProfiles performanceProfile,
            bool useCache,
            bool useLazyLoading,
            bool multiThreaded)
        {
            TestOnPremise_AllConfigurations_100_UserAgents(datafileName,
                performanceProfile,
                useCache,
                useLazyLoading,
                multiThreaded);
        }

        /// <summary>
        /// This test will create a device detection pipeline based on the
        /// supplied parameters, process 1000 user agents.
        /// The various parameters allow the test to be run for many
        /// different configurations.
        /// </summary>
        /// <param name="datafileName">
        /// The filename of the data file to use for device detection.
        /// </param>
        /// <param name="performanceProfile">
        /// The performance profile to use.
        /// </param>
        /// <param name="useCache">
        /// Whether or not to use a c# cache results cache in the pipeline.
        /// </param>
        /// <param name="useLazyLoading">
        /// Whether or not to use the lazy loading feature.
        /// </param>
        /// <param name="multiThreaded">
        /// Whether to use a single thread or multiple threads when 
        /// passing user agents to the pipeline for processing.
        /// </param>
        public void TestOnPremise_AllConfigurations_100_UserAgents(
            string datafileName,
            PerformanceProfiles performanceProfile,
            bool useCache,
            bool useLazyLoading,
            bool multiThreaded)
        {
            var datafile = Utils.GetFilePath(datafileName);
            var updateService = new Mock<IDataUpdateService>();

            // Configure the pipeline builder based on the 
            // parameters passed to this method.
            var builder = new DeviceDetectionPipelineBuilder(
                new NullLoggerFactory(), null, updateService.Object)
                .UseOnPremise(datafile.FullName, null, false)
                .SetPerformanceProfile(performanceProfile)
                .SetShareUsage(false)
                .SetDataFileSystemWatcher(false);
            if (useCache)
            {
                builder.UseResultsCache();
            }
            if (useLazyLoading)
            {
                builder.UseLazyLoading();
            }

            CancellationTokenSource cancellationSource = new CancellationTokenSource();

            using (var pipeline = builder.Build())
            {
                var options = new ParallelOptions()
                {
                    CancellationToken = cancellationSource.Token,
                    // The max parallelism is limited to 8 when the
                    // multiThreaded flag is enabled.
                    // This is because, if it is not limited, the lazy 
                    // loading tests will start all requests almost
                    // immediately and then some will take so long to
                    // complete that they exceed the configured timeout.
                    MaxDegreeOfParallelism = (multiThreaded ? 8 : 1)
                };


                // Create a parallel loop to actually process the user agents.
                Parallel.ForEach(USER_AGENTS.GetRandomUserAgents(100), options,
                    (useragent) =>
                    {
                        // Create the flow data instance
                        var flowData = pipeline.CreateFlowData();

                        // Add the user agent to the flow data 
                        // and process it
                        flowData.AddEvidence(
                                Pipeline.Core.Constants.EVIDENCE_HTTPHEADER_PREFIX +
                                Pipeline.Core.Constants.EVIDENCE_SEPERATOR + "User-Agent", useragent)
                            .Process();

                        // Make sure no errors occurred. If any did then
                        // cancel the parallel process.
                        if (flowData.Errors != null)
                        {
                            Assert.AreEqual(0, flowData.Errors.Count,
                                $"Expected no errors but got {flowData.Errors.Count}" +
                                $":{Environment.NewLine}{ReportErrors(flowData.Errors)}");
                            cancellationSource.Cancel();
                        }

                        // Get the device data instance and access the
                        // IsMobile property to ensure we can get 
                        // data out.
                        var deviceData = flowData.Get<IDeviceData>();
                        var result = deviceData.IsMobile;
                    });
            }
        }

        /// <summary>
        /// Private method to present the given list of FlowError 
        /// instances as a single string.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static string ReportErrors(IList<IFlowError> errors)
        {
            StringBuilder result = new StringBuilder();
            foreach(var error in errors)
            {
                result.AppendLine($"Error in element '{error.FlowElement.GetType().Name}'");
                AddExceptionToMessage(result, error.ExceptionData);
            }
            return result.ToString();
        }

        private static void AddExceptionToMessage(StringBuilder message, Exception ex, int depth = 0)
        {
            AddToMessage(message, $"{ex.GetType().Name} - {ex.Message}", depth);
            AddToMessage(message, $"{ex.StackTrace}", depth);
            if (ex.InnerException != null)
            {
                AddExceptionToMessage(message, ex.InnerException, depth++);
            }
        }

        private static void AddToMessage(StringBuilder message, string textToAdd, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                message.Append("   ");
            }
            message.AppendLine(textToAdd);
        }
    }
}
