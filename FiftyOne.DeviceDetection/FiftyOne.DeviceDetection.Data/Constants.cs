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

namespace FiftyOne.DeviceDetection.Shared
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
        /// <summary>
        /// The suffix that is used to identify a TAC in evidence.
        /// https://en.wikipedia.org/wiki/Type_Allocation_Code
        /// </summary>
        public const string EVIDENCE_TAC_SUFFIX = "tac";
        /// <summary>
        /// The complete key for supplying a TAC as evidence.
        /// </summary>
        public const string EVIDENCE_QUERY_TAC_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_TAC_SUFFIX;

        /// <summary>
        /// The suffix that is used to identify a native model name in evidence.
        /// This is the text returned by 
        /// https://developer.android.com/reference/android/os/Build#MODEL 
        /// for Android devices and by
        /// https://gist.github.com/soapyigu/c99e1f45553070726f14c1bb0a54053b#file-machinename-swift
        /// for iOS devices.
        /// </summary>
        public const string EVIDENCE_NATIVE_MODEL_SUFFIX = "nativemodel";
        /// <summary>
        /// The complete key for supplying a native model name as evidence.
        /// </summary>
        public const string EVIDENCE_QUERY_NATIVE_MODEL_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_NATIVE_MODEL_SUFFIX;


        /// <summary>
        /// The partial suffix used to identify profile ids in evidence.
        /// </summary>
        public const string EVIDENCE_PROFILE_IDS_SUFFIX = "ProfileIds";
        /// <summary>
        /// The complete key for supplying profile ids as evidence.
        /// </summary>
        public const string EVIDENCE_PROFILE_IDS_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX +
            EVIDENCE_PROFILE_IDS_SUFFIX;


        /// <summary>
        /// The partial suffix used to identify the User-Agent Client Hint sec-ch-ua in evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_SUFFIX = "sec-ch-ua";
        /// <summary>
        /// The complete key for supplying sec-ch-ua as evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_QUERY_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_SUFFIX;
        public const string EVIDENCE_SECCHUA_HEADER_KEY =
            Pipeline.Core.Constants.EVIDENCE_HTTPHEADER_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_SUFFIX;

        /// <summary>
        /// The partial suffix used to identify the User-Agent Client Hint sec-ch-ua-full-version-list in evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_FULLVERSIONLIST_SUFFIX = "sec-ch-ua-full-version-list";
        /// <summary>
        /// The complete key for supplying sec-ch-ua-full-version-list as evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_FULLVERSIONLIST_QUERY_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_FULLVERSIONLIST_SUFFIX;
        public const string EVIDENCE_SECCHUA_FULLVERSIONLIST_HEADER_KEY =
            Pipeline.Core.Constants.EVIDENCE_HTTPHEADER_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_FULLVERSIONLIST_SUFFIX;

        /// <summary>
        /// The partial suffix used to identify the User-Agent Client Hint sec-ch-ua-model in evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_MODEL_SUFFIX = "sec-ch-ua-model";
        /// <summary>
        /// The complete key for supplying sec-ch-ua-model as evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_MODEL_QUERY_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_MODEL_SUFFIX;
        public const string EVIDENCE_SECCHUA_MODEL_HEADER_KEY =
            Pipeline.Core.Constants.EVIDENCE_HTTPHEADER_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_MODEL_SUFFIX;

        /// <summary>
        /// The partial suffix used to identify the User-Agent Client Hint sec-ch-ua-mobile in evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_MOBILE_SUFFIX = "sec-ch-ua-mobile";
        /// <summary>
        /// The complete key for supplying sec-ch-ua-mobile as evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_MOBILE_QUERY_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_MOBILE_SUFFIX;
        public const string EVIDENCE_SECCHUA_MOBILE_HEADER_KEY =
            Pipeline.Core.Constants.EVIDENCE_HTTPHEADER_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_MOBILE_SUFFIX;

        /// <summary>
        /// The partial suffix used to identify the User-Agent Client Hint sec-ch-ua-platform in evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_PLATFORM_SUFFIX = "sec-ch-ua-platform";
        /// <summary>
        /// The complete key for supplying sec-ch-ua-platform as evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_PLATFORM_QUERY_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_PLATFORM_SUFFIX;
        public const string EVIDENCE_SECCHUA_PLATFORM_HEADER_KEY =
            Pipeline.Core.Constants.EVIDENCE_HTTPHEADER_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_PLATFORM_SUFFIX;

        /// <summary>
        /// The partial suffix used to identify the User-Agent Client Hint 
        /// sec-ch-ua-platform-version in evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_PLATFORM_VERSION_SUFFIX = "sec-ch-ua-platform-version";
        /// <summary>
        /// The complete key for supplying sec-ch-ua-platform-version as evidence.
        /// </summary>
        public const string EVIDENCE_SECCHUA_PLATFORM_VERSION_QUERY_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_PLATFORM_VERSION_SUFFIX;
        public const string EVIDENCE_SECCHUA_PLATFORM_VERSION_HEADER_KEY =
            Pipeline.Core.Constants.EVIDENCE_HTTPHEADER_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SECCHUA_PLATFORM_VERSION_SUFFIX;
    }
}
