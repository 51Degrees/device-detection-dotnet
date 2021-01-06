using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers
{
    internal interface IValueSwigWrapper<T> : IDisposable
    {
        bool hasValue();
        string getNoValueMessage();
        T getValue();
    }
}
