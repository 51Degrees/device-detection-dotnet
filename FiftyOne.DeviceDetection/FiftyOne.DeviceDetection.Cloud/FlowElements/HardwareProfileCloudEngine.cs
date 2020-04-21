using FiftyOne.DeviceDetection.Cloud.Data;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.Cloud.FlowElements
{
    public class HardwareProfileCloudEngine :
        PropertyKeyedCloudEngineBase<MultiDeviceDataCloud, IDeviceData>
    {
        public HardwareProfileCloudEngine(
            ILogger<PropertyKeyedCloudEngineBase<MultiDeviceDataCloud, IDeviceData>> logger, 
            Func<IPipeline, FlowElementBase<MultiDeviceDataCloud, IAspectPropertyMetaData>, MultiDeviceDataCloud> deviceDataFactory)
            : base(logger, deviceDataFactory)
        {
        }

        public override string ElementDataKey => "hardware";

        protected override IDeviceData CreateProfileData()
        {
            return new DeviceDataCloud(null, Pipelines.First(), this, _missingPropertyService);
        }
    }
}
