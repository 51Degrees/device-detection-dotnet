using FiftyOne.Pipeline.Core.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    public class PropertyKeyedDeviceEngine : PropertyKeyedDeviceBaseEngine
    {
        public PropertyKeyedDeviceEngine(
            ILoggerFactory loggerFactory, 
            IReadOnlyList<string> indexedProperties, 
            string keyProperty, 
            string elementDataKey) : 
            base(loggerFactory, indexedProperties, keyProperty, elementDataKey)
        {
        }

        protected override bool Validate(string keyPropertyValue, IFlowData data)
        {
            return true;
        }
    }
}
