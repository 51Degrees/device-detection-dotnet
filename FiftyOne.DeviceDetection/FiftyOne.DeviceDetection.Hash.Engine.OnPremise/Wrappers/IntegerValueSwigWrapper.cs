using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class IntegerValueSwigWrapper : IValueSwigWrapper<int>
    {
        public IntegerValueSwig Object { get; }

        public IntegerValueSwigWrapper(IntegerValueSwig instance)
        {
            Object = instance;
        }

        public string getNoValueMessage()
        {
            return Object.getNoValueMessage();
        }

        public int getValue()
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
