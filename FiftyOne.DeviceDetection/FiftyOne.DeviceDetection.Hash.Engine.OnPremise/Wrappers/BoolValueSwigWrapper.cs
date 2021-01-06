using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class BoolValueSwigWrapper : IValueSwigWrapper<bool>
    {
        public BoolValueSwig Object { get; }

        public BoolValueSwigWrapper(BoolValueSwig instance)
        {
            Object = instance;
        }

        public string getNoValueMessage()
        {
            return Object.getNoValueMessage();
        }

        public bool getValue()
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
