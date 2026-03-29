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

using System;
using System.IO;
using System.Linq;

namespace FiftyOne.DeviceDetection.RobotsTxt;

public class Constants
{
    /// <summary>
    /// The plain text of the license. Used for the web page and embedding the
    /// license into the robots.txt file annotations.
    /// </summary>
    public static string License { get; } = new StreamReader(
        typeof(Constants).Assembly.GetManifestResourceStream(
        typeof(Constants).Assembly.GetManifestResourceNames().Single(i => 
        i.Contains("license")))).ReadToEnd();

    /// <summary>
    /// The Url of the license for the robots.txt file.
    /// </summary>
    public static Uri LicenseUrl { get; } = new Uri(
        "https://51degrees.com/terms/robots-txt");
}
