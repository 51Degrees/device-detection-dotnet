/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2023 51 Degrees Mobile Experts Limited, Davidson House,
 * Forbury Square, Reading, Berkshire, United Kingdom RG1 3EU.
 *
 * This Original Work is licensed under the European Union Public Licence
 * (EUPL) v.1.2 and is subject to its terms as set out below.
 *
 * If a copy of the EUPL was not distributed with this file, You can obtain
 * one at https://opensource.org/licenses/EUPL-1.2.
 *
 * The 'Compatible Licences' set out in the Appendix to the EUPL (as may be
 * amended by the European Commission) shall be deemed incompatible for
 * the purposes of the Work and the provisions of the compatibility
 * clause in Article 5 of the EUPL shall not apply.
 *
 * If using the Work as, or as part of, a network application, by
 * including the attribution notice(s) required under Article 5 of the EUPL
 * in the end user terms of the application under an appropriate heading,
 * such notice(s) shall fulfill the requirements of that article.
 * ********************************************************************* */

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
