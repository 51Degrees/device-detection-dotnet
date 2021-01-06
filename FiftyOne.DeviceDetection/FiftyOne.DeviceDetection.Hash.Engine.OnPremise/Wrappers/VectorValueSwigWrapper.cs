using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class VectorValueSwigWrapper : IValueSwigWrapper<VectorStringSwig>
    {
        public VectorStringValuesSwig Object { get; }

        public VectorValueSwigWrapper(VectorStringValuesSwig instance)
        {
            Object = instance;
        }

        public string getNoValueMessage()
        {
            return Object.getNoValueMessage();
        }

        public VectorStringSwig getValue()
        {
            return Object.getValue();
        }

        public bool hasValue()
        {
            return Object.hasValue();
        }

        public void Dispose()
        {
            Object.Dispose();
        }
    }
}
