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

using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Apple
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
        public static readonly List<string> EVIDENCE_APPLE_KEYS = new List<string>()
        {
            EVIDENCE_FAMILY,
            EVIDENCE_HASH,
            EVIDENCE_HASH3D,
            EVIDENCE_RENDERER,
            EVIDENCE_SCREEN_GAMUT,
            EVIDENCE_SCREEN_HEIGHT,
            EVIDENCE_SCREEN_WIDTH,
            EVIDENCE_SCREEN_RATIO,
            EVIDENCE_BENCHMARK_AVG,
            EVIDENCE_BENCHMARK_STD
        };

        public const string EVIDENCE_SCREEN_WIDTH =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "width";
        public const string EVIDENCE_SCREEN_WIDTH_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SCREEN_WIDTH;

        public const string EVIDENCE_SCREEN_HEIGHT =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "height";
        public const string EVIDENCE_SCREEN_HEIGHT_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SCREEN_HEIGHT;

        public const string EVIDENCE_SCREEN_RATIO =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "ratio";
        public const string EVIDENCE_SCREEN_RATIO_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SCREEN_RATIO;

        public const string EVIDENCE_SCREEN_GAMUT =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "mediaColorGamut";
        public const string EVIDENCE_SCREEN_GAMUT_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_SCREEN_GAMUT;

        public const string EVIDENCE_HASH3D =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "hash3d";
        public const string EVIDENCE_HASH3D_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_HASH3D;

        public const string EVIDENCE_HASH =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "hash";
        public const string EVIDENCE_HASH_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_HASH;

        public const string EVIDENCE_BENCHMARK_AVG =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "benchmarkcpuavg";
        public const string EVIDENCE_BENCHMARK_AVG_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_BENCHMARK_AVG;

        public const string EVIDENCE_BENCHMARK_STD =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "benchmarkcpustd";
        public const string EVIDENCE_BENCHMARK_STD_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_BENCHMARK_STD;

        public const string EVIDENCE_FAMILY =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "family";
        public const string EVIDENCE_FAMILY_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_FAMILY;

        public const string EVIDENCE_RENDERER =
            Pipeline.Engines.Constants.FIFTYONE_COOKIE_PREFIX + "reportedrenderer";
        public const string EVIDENCE_RENDERER_KEY =
            Pipeline.Core.Constants.EVIDENCE_QUERY_PREFIX +
            Pipeline.Core.Constants.EVIDENCE_SEPERATOR +
            EVIDENCE_RENDERER;
    }
}
