using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Uach
{
    /// <summary>
    /// A definition of the JSON data object that is returned from a call to 'getHighEntropyValues'
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This is a JSON object class")]
    public class UachJson
    {
        public IList<UachJsonBrand> Brands { get; set; }
        public IList<UachJsonBrand> FullVersionList { get; set; }
        public bool? Mobile { get; set; }
        public string Model { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
    }

    public class UachJsonBrand
    {
        public string Brand { get; set; }
        public string Version { get; set; }
    }
}
