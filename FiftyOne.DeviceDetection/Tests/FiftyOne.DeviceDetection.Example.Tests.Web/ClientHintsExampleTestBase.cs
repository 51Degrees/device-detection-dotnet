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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    public class ClientHintsExampleTestBase : WebExampleTestBase
    {
        public static string SUPER_KEY { get; private set;}
        public static string BROWSER_KEY { get; private set; }
        public static string HARDWARE_KEY { get; private set; }
        public static string PLATFORM_KEY { get; private set; }
        public static string NO_ACCEPTCH_KEY { get; private set; }

        private static bool _gotEnvVars = false;

        protected static void GetEnvVars()
        {
            if (_gotEnvVars == false)
            {
                // Get resource keys from environment variables.
                // These must be configured with at least the following properties:
                // HardwareVendor,HardwareName,DeviceType,PlatformVendor,
                // PlatformName,PlatformVersion,BrowserVendor,BrowserName,
                // BrowserVersion
                // In addition, each key will need to have specific setup
                // for the '*Accept-CH' properties:
                // SUPER_RESOURCE_KEY - SetHeaderBrowserAccept-CH, 
                //    SetHeaderHardwareAccept-CH, SetHeaderPlatformAccept-CH
                // ACCEPTCH_BROWSER_KEY - SetHeaderBrowserAccept-CH only
                // ACCEPTCH_HARDWARE_KEY - SetHeaderHardwareAccept-CH only
                // ACCEPTCH_PLATFORM_KEY - SetHeaderPlatformAccept-CH only
                // ACCEPTCH_NONE_KEY - No *Accept-CH properties.
                GetKeyFromEnv("SUPER_RESOURCE_KEY", v => SUPER_KEY = v);
                GetKeyFromEnv("ACCEPTCH_BROWSER_KEY", v => BROWSER_KEY = v);
                GetKeyFromEnv("ACCEPTCH_HARDWARE_KEY", v => HARDWARE_KEY = v);
                GetKeyFromEnv("ACCEPTCH_PLATFORM_KEY", v => PLATFORM_KEY = v);
                GetKeyFromEnv("ACCEPTCH_NONE_KEY", v => NO_ACCEPTCH_KEY = v);
                _gotEnvVars = true;
            }
        }

        public const string BASE_PROPERTIES = "HardwareVendor,HardwareName,DeviceType,PlatformVendor,PlatformName,PlatformVersion,BrowserVendor,BrowserName,BrowserVersion";
        public const string ALL_PROPERTIES = null;
        public const string BROWSER_PROPERTIES = BASE_PROPERTIES + ",SetHeaderBrowserAccept-CH";
        public const string HARDWARE_PROPERTIES = BASE_PROPERTIES + ",SetHeaderHardwareAccept-CH";
        public const string PLATFORM_PROPERTIES = BASE_PROPERTIES + ",SetHeaderPlatformAccept-CH";

        public const string CHROME_UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36";
        public const string EDGE_UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36 Edg/95.0.1020.44";
        public const string FIREFOX_UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:94.0) Gecko/20100101 Firefox/94.0";
        public const string SAFARI_UA = "Mozilla/5.0 (iPhone; CPU iPhone OS 15_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/15.0 Mobile/15E148 Safari/604.1";
        public const string CURL_UA = "curl/7.80.0";

        /// <summary>
        /// Lists of the most critical headers for each component. Tests will fail if these are
        /// not included in Accept-CH when expected.
        /// </summary>
        public static readonly List<string> BROWSER_ACCEPT_CH = new List<string>()
        {
            "Sec-CH-UA"
        };
        public static readonly List<string> HARDWARE_ACCEPT_CH = new List<string>()
        {
            "Sec-CH-UA-Model"
        };
        public static readonly List<string> PLATFORM_ACCEPT_CH = new List<string>()
        {
            "Sec-CH-UA-Platform"
        };

        protected void VerifyAcceptCH(
            HttpResponseHeaders headers,
            List<string> expectedAcceptCH)
        {
            if (expectedAcceptCH == null || expectedAcceptCH.Count == 0)
            {
                // If no Accept-CH values are expected, then make sure the
                // header is not present in the response.
                if (headers.Contains("Accept-CH"))
                {
                    Assert.Fail(
                        $"Expected the Accept-CH header not to be " +
                        $"populated, but it was: " +
                        string.Join(",", headers.GetValues("Accept-CH")));
                }
            }
            else
            {
                // Verify the expected values are present in the 
                // Accept-CH header.
                Assert.IsTrue(headers.Contains("Accept-CH"),
                    "Expected the Accept-CH header to be populated, " +
                    "but it was not.");
                var actualValues = headers.GetValues("Accept-CH")
                    .SelectMany(h => h.Split(new char[] { ',', ' ' },
                        StringSplitOptions.RemoveEmptyEntries));
                // We don't require the expected list of values to match exactly, as the headers 
                // used by detection change over time. However, we do make sure that the most 
                // critical ones are included.
                foreach (var expectedValue in expectedAcceptCH)
                {
                    Assert.IsTrue(actualValues.Contains(expectedValue, 
                            StringComparer.OrdinalIgnoreCase),
                        $"The list of values in Accept-CH does not include " +
                        $"'{expectedValue}'. ({string.Join(",", actualValues)})");
                }
            }
        }

    }
}
