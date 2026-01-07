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
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FiftyOne.DeviceDetection.Cloud.FlowElements
{
    /// <summary>
    /// A cloud-based engine that can return multiple hardware profiles 
    /// from a single request.
    /// For example, A single TAC code can match multiple hardware devices.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/pipeline-elements/hardware-profile-lookup-cloud.md">Specification</see>
    /// </summary>
    public class HardwareProfileCloudEngine :
        PropertyKeyedCloudEngineBase<MultiDeviceDataCloud, IDeviceData>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">
        /// The logger for this instance to use
        /// </param>
        /// <param name="deviceDataFactory">
        /// Factory function to use when creating aspect data instances.
        /// </param>
        public HardwareProfileCloudEngine(
            ILogger<PropertyKeyedCloudEngineBase<MultiDeviceDataCloud, IDeviceData>> logger, 
            Func<IPipeline, FlowElementBase<MultiDeviceDataCloud, IAspectPropertyMetaData>, MultiDeviceDataCloud> deviceDataFactory)
            : base(logger, deviceDataFactory)
        {
        }

        /// <summary>
        /// The key to use for storing this engine's data in a 
        /// <see cref="IFlowData"/> instance.
        /// </summary>
        public override string ElementDataKey => "hardware";

        /// <summary>
        /// Called by the base class to create a new data instance.
        /// </summary>
        /// <returns></returns>
        protected override IDeviceData CreateProfileData()
        {
            if(Pipelines.Count == 0)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    Messages.ExceptionNotAddedToPipeline,
                    GetType().Name);
                throw new PipelineException(message);
            }

            return new DeviceDataCloud(null, 
                Pipelines[0], this, 
                MissingPropertyService.Instance);
        }

        protected override Type GetPropertyType(PropertyMetaData propertyMetaData, Type parentObjectType)
        {
            if (propertyMetaData == null)
            {
                throw new ArgumentNullException(nameof(propertyMetaData));
            }
            if (parentObjectType == null)
            {
                throw new ArgumentNullException(nameof(parentObjectType));
            }
            if (propertyMetaData.Name == "Profiles")
            {
                return typeof(IReadOnlyList<IDeviceData>);
            }
            if (parentObjectType.Equals(typeof(IDeviceData)) &&
                DeviceDataCloud.TryGetPropertyType(
                    propertyMetaData.Name, out var type))
            {
                return type;
            }
            return base.GetPropertyType(propertyMetaData, parentObjectType);
        }
    }
}
