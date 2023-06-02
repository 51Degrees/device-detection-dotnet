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

using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Configuration;
using FiftyOne.Pipeline.Engines.FlowElements;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace FiftyOne.DeviceDetection
{
    /// <summary>
    /// Builder used to create pipelines with an cloud-based 
    /// device detection engine.
    /// </summary>
    public class DeviceDetectionCloudPipelineBuilder :
        CloudPipelineBuilderBase<DeviceDetectionCloudPipelineBuilder>
    {
        /// <summary>
        /// Internal Constructor.
        /// This builder should only be created through the 
        /// <see cref="DeviceDetectionPipelineBuilder"/> 
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="httpClient"></param>
        internal DeviceDetectionCloudPipelineBuilder(
            ILoggerFactory loggerFactory,
            HttpClient httpClient) : base(loggerFactory, httpClient)
        {
        }

        /// <summary>
        /// Build the pipeline using the configured values.
        /// </summary>
        /// <returns>
        /// A new <see cref="IPipeline"/> instance that contains a 
        /// <see cref="CloudRequestEngine"/> for making requests
        /// to the 51Degrees cloud service and a 
        /// <see cref="DeviceDetectionCloudEngine"/> to interpret the
        /// device detection results.
        /// </returns>
        public override IPipeline Build()
        {
            // Configure and build the device detection engine
            var deviceDetectionEngineBuilder = new DeviceDetectionCloudEngineBuilder(LoggerFactory);
            if (LazyLoading)
            {
                deviceDetectionEngineBuilder.SetLazyLoading(new LazyLoadingConfiguration(
                    (int)LazyLoadingTimeout.TotalMilliseconds,
                    LazyLoadingCancellationToken));
            }

            // Add the engine to the list (The CloudRequestEngine 
            // will be added by the base class)
            FlowElements.Add(deviceDetectionEngineBuilder.Build());

            // Build and return the pipeline
            return base.Build();
        }
    }
}
