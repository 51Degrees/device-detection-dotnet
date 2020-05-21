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
    }
}
