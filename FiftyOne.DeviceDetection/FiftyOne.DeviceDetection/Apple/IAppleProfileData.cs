using FiftyOne.Pipeline.Engines.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// The data interface populated by <see cref="AppleProfileEngine"/>
    /// </summary>
    public interface IAppleProfileData : IAspectData
    {
        uint? ProfileId { get; set; }
    }
}
