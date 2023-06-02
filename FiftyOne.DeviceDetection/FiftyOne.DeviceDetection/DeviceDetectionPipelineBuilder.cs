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

using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;

namespace FiftyOne.DeviceDetection
{
    /// <summary>
    /// Builder used to create a Pipeline with a device detection engine.
    /// </summary>
    public class DeviceDetectionPipelineBuilder
    {
        private ILoggerFactory _loggerFactory;
        private IDataUpdateService _dataUpdateService;
        private HttpClient _httpClient;

        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceDetectionPipelineBuilder() :
            this(new LoggerFactory())
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory">
        /// The factory to use for creating loggers within the pipeline.
        /// </param>
        public DeviceDetectionPipelineBuilder(
            ILoggerFactory loggerFactory) :
            this(loggerFactory, new HttpClient())
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory">
        /// The factory to use for creating loggers within the pipeline.
        /// </param>
        /// <param name="httpClient">
        /// The HTTP client to use within the pipeline.
        /// </param>
        public DeviceDetectionPipelineBuilder(
            ILoggerFactory loggerFactory,
            HttpClient httpClient)
        {
            _loggerFactory = loggerFactory;
            _httpClient = httpClient;
            _dataUpdateService = new DataUpdateService(
                _loggerFactory.CreateLogger<DataUpdateService>(),
                _httpClient);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory">
        /// The factory to use for creating loggers within the pipeline.
        /// </param>
        /// <param name="httpClient">
        /// The HTTP client to use within the pipeline.
        /// </param>
        /// <param name="dataUpdateService">
        /// The DataUpdateService to use when checking for data updates.
        /// </param>
        public DeviceDetectionPipelineBuilder(
            ILoggerFactory loggerFactory,
            HttpClient httpClient,
            IDataUpdateService dataUpdateService)
        {
            _loggerFactory = loggerFactory;
            _httpClient = httpClient;
            _dataUpdateService = dataUpdateService;
        }

        /// <summary>
        /// Use the 51Degrees Cloud service to perform device detection.
        /// </summary>
        /// <param name="resourceKey">
        /// The resource key to use when querying the cloud service. 
        /// Obtain one from https://configure.51degrees.com
        /// </param>
        /// <returns>
        /// A builder that can be used to configure and build a pipeline
        /// that will use the cloud device detection engine.
        /// </returns>
        public DeviceDetectionCloudPipelineBuilder UseCloud(string resourceKey)
        {
            return new DeviceDetectionCloudPipelineBuilder(
                _loggerFactory, _httpClient)
                .SetResourceKey(resourceKey);
        }

        /// <summary>
        /// Use the 51Degrees Cloud service to perform device detection.
        /// </summary>
        /// <param name="resourceKey">
        /// The resource key to use when querying the cloud service. 
        /// Obtain one from https://configure.51degrees.com
        /// </param>
        /// <param name="endpoint">
        /// The 51Degrees Cloud URL.
        /// </param>
        /// <returns>
        /// A builder that can be used to configure and build a pipeline
        /// that will use the cloud device detection engine.
        /// </returns>
        public DeviceDetectionCloudPipelineBuilder UseCloud(string resourceKey, string endpoint)
        {
#pragma warning disable CA2234 // Pass system uri objects instead of strings
            // Changing this has some unintended consequences so 
            // leaving for now.
            return UseCloud(resourceKey).SetEndPoint(endpoint);
#pragma warning restore CA2234 // Pass system uri objects instead of strings
        }

        /// <summary>
        /// Use a 51Degrees on-premise device detection engine to 
        /// perform device detection.
        /// </summary>
        /// <param name="datafile">
        /// The full path to the device detection data file.
        /// </param>
        /// <param name="createTempDataCopy">
        /// If true, the engine will create a temporary copy of the data 
        /// file rather than using the data file directly.
        /// </param>
        /// <returns>
        /// A builder that can be used to configure and build a pipeline
        /// that will use the on-premise detection engine.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if a required parameter is null
        /// </exception>
        [Obsolete("Call the overload that takes a license key instead. " +
            "This method will be removed in a future version")]
        public DeviceDetectionOnPremisePipelineBuilder UseOnPremise(
            string datafile, bool createTempDataCopy)
        {
            if (datafile == null) { throw new ArgumentNullException(nameof(datafile)); }

            var builder = new DeviceDetectionOnPremisePipelineBuilder(
                _loggerFactory, _dataUpdateService, _httpClient);
            builder.SetFilename(datafile, "", createTempDataCopy);
            return builder;
        }

        /// <summary>
        /// Use a 51Degrees on-premise device detection engine to 
        /// perform device detection.
        /// </summary>
        /// <param name="datafile">
        /// The full path to the device detection data file.
        /// </param>
        /// <param name="key">
        /// The license key to use when checking for updates to the
        /// data file.
        /// A license key can be obtained from the 
        /// [51Degrees website](https://51degrees.com/pricing).
        /// If you have no license key then this parameter can be 
        /// set to null, but doing so will disable automatic updates. 
        /// </param>
        /// <param name="createTempDataCopy">
        /// If true, the engine will create a temporary copy of the data 
        /// file rather than using the data file directly.
        /// This is required in order for automatic updates to work 
        /// correctly.
        /// </param>
        /// <returns>
        /// A builder that can be used to configure and build a pipeline
        /// that will use the on-premise detection engine.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if a required parameter is null
        /// </exception>
        /// <seealso cref="UseOnPremise(Stream, DeviceDetectionAlgorithm, string)"/>
        public DeviceDetectionOnPremisePipelineBuilder UseOnPremise(
            string datafile, string key, bool createTempDataCopy = true)
        {
            if (datafile == null) { throw new ArgumentNullException(nameof(datafile)); }

            var builder = new DeviceDetectionOnPremisePipelineBuilder(
                _loggerFactory, _dataUpdateService, _httpClient);
            builder.SetFilename(datafile, key, createTempDataCopy);
            return builder;
        }

        /// <summary>
        /// Use a 51Degrees on-premise device detection engine to 
        /// perform device detection.
        /// </summary>
        /// <param name="dataStream">
        /// The device detection data file as a <see cref="Stream"/>.
        /// </param>
        /// <param name="algorithm">
        /// The detection algorithm that the supplied data supports.
        /// </param>
        /// <returns>
        /// A builder that can be used to configure and build a pipeline
        /// that will use the on-premise detection engine.
        /// </returns>
        [Obsolete("Call the overload that takes a license key instead. " +
            "This method will be removed in a future version")]
        public DeviceDetectionOnPremisePipelineBuilder UseOnPremise(
            Stream dataStream, DeviceDetectionAlgorithm algorithm)
        {
            var builder = new DeviceDetectionOnPremisePipelineBuilder(
                _loggerFactory, _dataUpdateService, _httpClient);
            builder.SetEngineData(dataStream, algorithm);
            return builder;
        }

        /// <summary>
        /// Use a 51Degrees on-premise device detection engine to 
        /// perform device detection.
        /// </summary>
        /// <param name="dataStream">
        /// The device detection data file as a <see cref="Stream"/>.
        /// </param>
        /// <param name="algorithm">
        /// The detection algorithm that the supplied data supports.
        /// </param>
        /// <param name="key">
        /// The license key to use when checking for updates to the
        /// data file.
        /// A license key can be obtained from the 
        /// [51Degrees website](https://51degrees.com/pricing).
        /// If you have no license key then this parameter can be 
        /// set to null, but doing so will disable automatic updates. 
        /// </param>
        /// <returns>
        /// A builder that can be used to configure and build a pipeline
        /// that will use the on-premise detection engine.
        /// </returns>
        /// <seealso cref="UseOnPremise(string, string, bool)"/>
        public DeviceDetectionOnPremisePipelineBuilder UseOnPremise(
            Stream dataStream, DeviceDetectionAlgorithm algorithm, string key)
        {
            var builder = new DeviceDetectionOnPremisePipelineBuilder(
                _loggerFactory, _dataUpdateService, _httpClient);
            builder.SetEngineData(dataStream, algorithm,key);
            return builder;
        }
    }


}
