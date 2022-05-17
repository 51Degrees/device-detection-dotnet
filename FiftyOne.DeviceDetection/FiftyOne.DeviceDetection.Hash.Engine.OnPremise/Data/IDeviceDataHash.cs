/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.Pipeline.Engines.Data;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data
{
    /// <summary>
    /// Represents a data class that contains device data from an 
    /// on-premise device detection engine. 
    /// </summary>
    public interface IDeviceDataHash : IDeviceData
    {
        /// <summary>
        /// Gets all values that this instance has for a specific property.
        /// This is used for 'list' properties that can have multiple 
        /// values for a single profile.
        /// </summary>
        /// <param name="propertyName">
        /// The property to get the values for.
        /// </param>
        /// <returns>
        /// An list of strings wrapped in an <see cref="IAspectPropertyValue"/>
        /// instance.
        /// </returns>
        IAspectPropertyValue<IReadOnlyList<string>> GetValues(string propertyName);
    }
}
