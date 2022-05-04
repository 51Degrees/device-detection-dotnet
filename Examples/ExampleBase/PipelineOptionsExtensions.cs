using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using System;
using System.Linq;

namespace FiftyOne.DeviceDetection.Examples
{
    public static class PipelineOptionsExtensions
    {
        /// <summary>
        /// The name of the setting that is used to specify the resource key in configuration
        /// files.
        /// </summary>
        private const string RESOURCE_KEY_SETTING_NAME = "ResourceKey";
        /// <summary>
        /// The name of the setting that is used to specify the cloud end point in configuration
        /// files.
        /// </summary>
        private const string CLOUD_END_POINT_SETTING_NAME = "EndPoint";

        /// <summary>
        /// The name of the setting that is used to specify the data file in configuration files.
        /// </summary>
        private const string DATA_FILE_SETTING_NAME = "DataFile";

        /// <summary>
        /// Get the resource key setting from the supplied <see cref="PipelineOptions"/> 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetResourceKey(this PipelineOptions options)
        {
            var cloudConfig = options.GetElementConfig(nameof(CloudRequestEngine));
            cloudConfig.BuildParameters.TryGetValue(RESOURCE_KEY_SETTING_NAME,
                out var resourceKeyObj);
            return resourceKeyObj?.ToString();
        }

        /// <summary>
        /// Set the resource key setting in the supplied <see cref="PipelineOptions"/> instance
        /// to the given value.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="resourceKey"></param>
        public static void SetResourceKey(this PipelineOptions options, string resourceKey)
        {
            var cloudConfig = options.GetElementConfig(nameof(CloudRequestEngine));
            cloudConfig.BuildParameters[RESOURCE_KEY_SETTING_NAME] = resourceKey;
        }

        /// <summary>
        /// Set the cloud end point setting in the supplied <see cref="PipelineOptions"/> instance
        /// to the given value.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cloudEndPoint"></param>
        public static void SetCloudEndPoint(this PipelineOptions options, string cloudEndPoint)
        {
            var cloudConfig = options.GetElementConfig(nameof(CloudRequestEngine));
            cloudConfig.BuildParameters[CLOUD_END_POINT_SETTING_NAME] = cloudEndPoint;
        }

        /// <summary>
        /// Get the resource key setting from the supplied <see cref="PipelineOptions"/> 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetDataFile(this PipelineOptions options)
        {
            var hashConfig = options.GetElementConfig(nameof(DeviceDetectionHashEngine));
            hashConfig.BuildParameters.TryGetValue(DATA_FILE_SETTING_NAME,
                out var dataFileObj);
            return dataFileObj?.ToString();
        }

        /// <summary>
        /// Set the data file setting from the supplied <see cref="PipelineOptions"/> 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public static void SetDataFile(this PipelineOptions options, string dataFile)
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
