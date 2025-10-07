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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using System;
using System.Linq;

namespace performance_tests
{
    public static class PipelineOptionsExtensions
    {
        /// <summary>
        /// The name of the setting that is used to specify the data file in configuration files.
        /// </summary>
        private const string DATA_FILE_SETTING_NAME = "DataFile";

        /// <summary>
        /// Get the hash data file setting from the supplied <see cref="PipelineOptions"/> 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetHashDataFile(this PipelineOptions options)
        {
            var hashConfig = options.GetElementConfig(nameof(DeviceDetectionHashEngine));
            hashConfig.BuildParameters.TryGetValue(DATA_FILE_SETTING_NAME,
                out var dataFileObj);
            return dataFileObj?.ToString();
        }

        /// <summary>
        /// Set the hash data file setting from the supplied <see cref="PipelineOptions"/> 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public static void SetHashDataFile(this PipelineOptions options, string dataFile)
        {
            var hashConfig = options.GetElementConfig(nameof(DeviceDetectionHashEngine));
            hashConfig.BuildParameters[DATA_FILE_SETTING_NAME] = dataFile;
        }

        public static ElementOptions GetElementConfig(
            this PipelineOptions options, 
            string elementName)
        {
            var query = options.Elements
                .Where(e => e.BuilderName.IndexOf(elementName,
                    StringComparison.OrdinalIgnoreCase) >= 0);
            if (query.Count() == 1)
            {
                return query.Single();
            }
            else
            {
                throw new Exception($"Failed to find the expected '{elementName}' section " +
                    "in the supplied configuration.");
            }
        }
    }
}
