using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class StringValueSwigWrapper : IValueSwigWrapper<string>
    {
        public StringValueSwig Object { get; }

        public StringValueSwigWrapper(StringValueSwig instance)
        {
            Object = instance;
        }

        public string getNoValueMessage()
        {
            return Object.getNoValueMessage();
        }

        public string getValue()
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
