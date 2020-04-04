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
        private const string DEVICE_LIST_KEY = "devices";

        public MultiDeviceDataCloud(ILogger<AspectDataBase> logger, IPipeline pipeline, IAspectEngine engine) : base(logger, pipeline, engine)
        {
            this[DEVICE_LIST_KEY] = new List<IDeviceData>();
        }


        public MultiDeviceDataCloud(ILogger<AspectDataBase> logger, IPipeline pipeline, IAspectEngine engine, IMissingPropertyService missingPropertyService) : base(logger, pipeline, engine, missingPropertyService)
        {
            this[DEVICE_LIST_KEY] = new List<IDeviceData>();
        }

        public IReadOnlyList<IDeviceData> Devices => GetDeviceList();
        
        public void AddDevice(IDeviceData device)
        {
            GetDeviceList().Add(device);
        }

        private List<IDeviceData> GetDeviceList()
        {
            return this[DEVICE_LIST_KEY] as List<IDeviceData>;
        }
    }
}
