/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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

namespace FiftyOne.DeviceDetection.Uach
{
    /// <summary>
    /// Class containing values for commonly used evidence keys
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming",
        "CA1707:Identifiers should not contain underscores",
        Justification = "51Degrees coding style is for constant names " +
            "to be all-caps with an underscore to separate words.")]
    public static class Constants
    {
        public const string EVIDENCE_HIGH_ENTROPY_VALUES =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "gethighentropyvalues";
        public const string EVIDENCE_HIGH_ENTROPY_VALUES_KEY_QUERY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_HIGH_ENTROPY_VALUES;
        public const string EVIDENCE_HIGH_ENTROPY_VALUES_KEY_COOKIE =
            Pipeline.Core.Constants.EVIDENCE_COOKIE_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_HIGH_ENTROPY_VALUES;

    }
}
