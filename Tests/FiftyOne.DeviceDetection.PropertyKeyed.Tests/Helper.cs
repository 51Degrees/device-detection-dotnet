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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    /// <summary>
    /// Helper methods for finding test data files.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Returns all the .hash device detection data files found by
        /// walking up the directory tree looking for a 
        /// "device-detection-data" folder.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetDeviceDetectionFiles()
        {
            const string ddFolder = "device-detection-data";
            var directory = GetDirectory(ddFolder);
            if (directory != null)
            {
                foreach (var file in directory.GetFiles().Where(i =>
                    i.Name.EndsWith(".hash")))
                {
                    yield return file.FullName;
                }
            }
        }

        private static DirectoryInfo GetDirectory(string configFolder)
        {
            var current = new DirectoryInfo(
                AppDomain.CurrentDomain.BaseDirectory);
            while (current != null)
            {
                var directory = new DirectoryInfo(Path.Combine(
                    current.FullName,
                    configFolder));
                if (directory.Exists)
                {
                    return directory;
                }
                current = current.Parent;
            }
            return null;
        }
    }
}
