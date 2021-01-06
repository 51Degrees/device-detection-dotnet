using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal class DoubleValueSwigWrapper : IValueSwigWrapper<double>
    {
        public DoubleValueSwig Object { get; }

        public DoubleValueSwigWrapper(DoubleValueSwig instance)
        {
            Object = instance;
        }

        public string getNoValueMessage()
        {
            return Object.getNoValueMessage();
        }

        public double getValue()
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
