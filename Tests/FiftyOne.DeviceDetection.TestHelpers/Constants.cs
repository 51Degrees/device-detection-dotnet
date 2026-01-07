/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2026 51 Degrees Mobile Experts Limited, Davidson House,
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

namespace FiftyOne.DeviceDetection.TestHelpers
{
    public static class Constants
    {
        public const int UAS_TO_TEST = 10;
        public const string TAC_HASH_DATA_FILE_NAME = "TAC-HashV41.hash";
        public const string ENTERPRISE_HASH_DATA_FILE_NAME = "Enterprise-HashV41.hash";
        public const string LITE_HASH_DATA_FILE_NAME = "51Degrees-LiteV4.1.hash";
        public const string UA_FILE_NAME = "20000 User Agents.csv";

        public static string[] ExcludedProperties = { "JavascriptImageOptimiser" };

        public static string MobileUserAgent =
            "Mozilla/5.0 (iPhone; CPU iPhone OS 7_1 like Mac OS X) " +
            "AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile" +
            "/11D167 Safari/9537.53";

        public static string MobileUserAgentiOS17 =
            "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1";

        public static string[] RequiredProperties = {
                    "HardwareVendor",
                    "HardwareName",
                    "HardwareModel",
                    "PlatformName",
                    "PlatformVersion",
                    "BrowserName",
                    "BrowserVersion",
                    "IsMobile"
        };

        public const string JsonOutputTAC = "{\"BrowserName\":\"Mobile Safari\",\"BrowserVersion\":\"17.0\",\"HardwareModel\":\"iPhone\",\"HardwareName\":[\"iPhone\",\"iPhone 11\",\"iPhone 11 Pro\",\"iPhone 11 Pro Max\",\"iPhone 12\",\"iPhone 12 Pro\",\"iPhone 12 Pro Max\",\"iPhone 12 mini\",\"iPhone 13\",\"iPhone 13 Pro\",\"iPhone 13 Pro Max\",\"iPhone 13 mini\",\"iPhone 14\",\"iPhone 14 Plus\",\"iPhone 14 Pro\",\"iPhone 14 Pro Max\",\"iPhone 15\",\"iPhone 15 Plus\",\"iPhone 15 Pro\",\"iPhone 15 Pro Max\",\"iPhone 16\",\"iPhone 16 Plus\",\"iPhone 16 Pro\",\"iPhone 16 Pro Max\",\"iPhone 3G\",\"iPhone 3GS\",\"iPhone 4\",\"iPhone 4S\",\"iPhone 5\",\"iPhone 5S\",\"iPhone 5c\",\"iPhone 6\",\"iPhone 6 Plus\",\"iPhone 6s\",\"iPhone 6s Plus\",\"iPhone 7\",\"iPhone 7 Plus\",\"iPhone 8\",\"iPhone 8 Plus\",\"iPhone SE\",\"iPhone SE (2nd Gen.)\",\"iPhone SE (3rd Gen.)\",\"iPhone X\",\"iPhone XR\",\"iPhone XS\",\"iPhone XS Max\"],\"HardwareVendor\":\"Apple\",\"IsMobile\":\"True\",\"PlatformName\":\"iOS\",\"PlatformVersion\":\"17.0\"}";
        public const string JsonOutputLite = "{\"BrowserName\":\"Mobile Safari\",\"BrowserVersion\":\"17.0\",\"IsMobile\":\"True\",\"PlatformName\":\"iOS\",\"PlatformVersion\":\"17.0\"}";
    }
}
