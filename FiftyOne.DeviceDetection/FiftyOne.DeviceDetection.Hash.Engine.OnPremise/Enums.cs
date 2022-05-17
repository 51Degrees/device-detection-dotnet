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

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise
{
    /// <summary>
    /// Enumeration of possible match methods used by the Hash engine.
    /// </summary>
#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names
    // Would be a breaking change that we don't want to make right now.
    public enum MatchMethods
#pragma warning restore CA1717 // Only FlagsAttribute enums should have plural names
    {
        /// <summary>
        /// No match was found
        /// </summary>
        NONE,
        /// <summary>
        /// The performance graph was used to find a match.
        /// </summary>
        PERFORMANCE,
        /// <summary>
        /// Both the performance and predictive graphs were used to find 
        /// a match
        /// </summary>
        COMBINED,
        /// <summary>
        /// The predictive graph was used to find a match.
        /// </summary>
        PREDICTIVE
    }
}