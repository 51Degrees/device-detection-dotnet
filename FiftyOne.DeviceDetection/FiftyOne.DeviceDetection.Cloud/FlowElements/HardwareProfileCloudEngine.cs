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
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.Cloud.FlowElements
{
    /// <summary>
    /// A cloud-based engine that can return multiple hardware profiles 
    /// from a single request.
    /// For example, A single TAC code can match multiple hardware devices.
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
