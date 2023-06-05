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

namespace FiftyOne.DeviceDetection.Shared.Services
{
    /// <summary>
    /// Helper service for working with device detection meta-data.
    /// </summary>
    public interface IMetaDataService
    {

        /// <summary>
        /// Get the default profile Id for the specified component
        /// </summary>
        /// <param name="componentId">
        /// The component Id to get the default for
        /// </param>
        /// <returns>
        /// The default profile Id for the specified component
        /// </returns>
        uint? DefaultProfileIdForComponent(byte componentId);

        /// <summary>
        /// Get the component Id for the specified profile
        /// </summary>
        /// <param name="profileId">
        /// The profile Id to get the component Id for
        /// </param>
        /// <returns>
        /// The component Id for the specified profile
        /// </returns>
        byte? ComponentIdForProfile(uint profileId);

        /// <summary>
        /// Get all default profile Ids
        /// </summary>
        /// <returns>
        /// A dictionary with key of component Id and value of profile Id.
        /// </returns>
        IReadOnlyDictionary<byte, uint?> DefaultProfilesIds();

    }
}
