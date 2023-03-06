using System;
using System.Collections.Generic;
using System.Text;

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
