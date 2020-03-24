using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Cloud.Data
{
    public class MultiDeviceDataCloud : AspectDataBase, IMultiDeviceData
    {
        private List<IDeviceData> _devices = new List<IDeviceData>();

        public MultiDeviceDataCloud(ILogger<AspectDataBase> logger, IPipeline pipeline, IAspectEngine engine) : base(logger, pipeline, engine)
        {
        }

        public MultiDeviceDataCloud(ILogger<AspectDataBase> logger, IPipeline pipeline, IAspectEngine engine, IMissingPropertyService missingPropertyService) : base(logger, pipeline, engine, missingPropertyService)
        {
        }

        public IReadOnlyList<IDeviceData> Devices => _devices;

        
        public void AddDevice(IDeviceData device)
        {
            _devices.Add(device);
        }
    }
}
