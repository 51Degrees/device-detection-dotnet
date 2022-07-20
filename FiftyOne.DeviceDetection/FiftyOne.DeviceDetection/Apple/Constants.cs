using System;
using System.Collections.Generic;
using System.Text;

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
