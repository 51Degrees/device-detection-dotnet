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

using FiftyOne.DeviceDetection.Cloud.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Cloud.FlowElements
{
    /// <summary>
    /// Fluent builder used to create a cloud-based engine that can
    /// return multiple hardware profiles from a single request.
    /// For example, A single TAC code can match multiple hardware devices.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/pipeline-elements/hardware-profile-lookup-cloud.md">Specification</see>
    /// </summary>
    public class HardwareProfileCloudEngineBuilder: AspectEngineBuilderBase<HardwareProfileCloudEngineBuilder, HardwareProfileCloudEngine>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<DeviceDataCloud> _dataLogger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory">
        /// The factory to use when creating a logger.
        /// </param>
        public HardwareProfileCloudEngineBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _dataLogger = _loggerFactory.CreateLogger<DeviceDataCloud>();
        }

        /// <summary>
        /// Build a new engine using the configured values.
        /// </summary>
        /// <returns>
        /// A new <see cref="HardwareProfileCloudEngine"/>
        /// </returns>
        public HardwareProfileCloudEngine Build()
        {
            return BuildEngine();
        }

        /// <summary>
        /// This method is called by the base class to create a new
        /// <see cref="HardwareProfileCloudEngine"/> instance before 
        /// additional configuration is applied.
        /// </summary>
        /// <param name="properties">
        /// A string list of the properties that the engine should populate.
        /// In this case, this list is ignored as the resource key 
        /// defines the properties that are returned by the cloud service.
        /// </param>
        /// <returns>
        /// A new <see cref="HardwareProfileCloudEngine"/> instance.
        /// </returns>
        protected override HardwareProfileCloudEngine NewEngine(List<string> properties)
        {
            return new HardwareProfileCloudEngine(
                _loggerFactory.CreateLogger<HardwareProfileCloudEngine>(),
                CreateData);
        }

        private MultiDeviceDataCloud CreateData(IPipeline pipeline, FlowElementBase<MultiDeviceDataCloud, IAspectPropertyMetaData> engine)
        {
            return new MultiDeviceDataCloud(
                _dataLogger,
                pipeline,
                (IAspectEngine)engine,
                MissingPropertyService.Instance);
        }

    }
}
