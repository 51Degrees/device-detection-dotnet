/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL) 
 * v.1.2 and is subject to its terms as set out below.
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

using FiftyOne.DeviceDetection.Cloud.Data;
using FiftyOne.Pipeline.CloudRequestEngine.Data;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Cloud.FlowElements
{
    /// <summary>
    /// Engine that takes the JSON response from the 
    /// <see cref="CloudRequestEngine"/> and uses it populate a 
    /// DeviceDataCloud instance for easier consumption.
    /// </summary>
    public class DeviceDetectionCloudEngine : CloudAspectEngineBase<DeviceDataCloud, IAspectPropertyMetaData>
    {
        private IList<IAspectPropertyMetaData> _aspectProperties;
        private object _aspectPropertiesLock = new object();
        private string _dataSourceTier;

        public DeviceDetectionCloudEngine(
            ILogger<DeviceDetectionCloudEngine> logger,
            Func<IPipeline, FlowElementBase<DeviceDataCloud, IAspectPropertyMetaData>, DeviceDataCloud> deviceDataFactory)
            : base(logger,
                  deviceDataFactory)
        {
        }

        public override IList<IAspectPropertyMetaData> Properties
        {
            get
            {
                if (_aspectProperties == null)
                {
                    lock (_aspectPropertiesLock)
                    {
                        if (_aspectProperties == null)
                        {
                            if (LoadAspectProperties(RequestEngine.Instance) == false)
                            {
                                throw new Exception("Failed to load aspect properties");
                            }
                        }
                    }
                }
                return _aspectProperties;
            }
        }

        public override string DataSourceTier => _dataSourceTier;

        public override string ElementDataKey => "device";

        public override IEvidenceKeyFilter EvidenceKeyFilter =>
            // This engine needs no evidence. 
            // It works from the cloud request data.
            new EvidenceKeyFilterWhitelist(new List<string>());

        private static JsonConverter[] JSON_CONVERTERS = new JsonConverter[]
        {
            new CloudJsonConverter()
        };

        protected override void ProcessEngine(IFlowData data, DeviceDataCloud aspectData)
        {
            var requestData = data.GetFromElement(RequestEngine.Instance);
            var json = requestData?.JsonResponse;

            if (string.IsNullOrEmpty(json))
            {
                throw new PipelineConfigurationException(
                    $"Json response from cloud request engine is null. " +
                    $"This is probably because there is not a " +
                    $"'CloudRequestEngine' before the '{GetType().Name}' " +
                    $"in the Pipeline. This engine will be unable " +
                    $"to produce results until this is corrected.");
            }
            else
            {
                // Extract data from json to the aspectData instance.
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                var device = JsonConvert.DeserializeObject<Dictionary<string, object>>(dictionary["device"].ToString(),
                    new JsonSerializerSettings()
                    {
                        Converters = JSON_CONVERTERS,
                    });
                var noValueReasons = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                JsonConvert.PopulateObject(dictionary["nullValueReasons"].ToString(), noValueReasons);

                aspectData.SetNoValueReasons(noValueReasons);
                aspectData.PopulateFromDictionary(device);
            }
        }

        protected override void UnmanagedResourcesCleanup()
        {
        }

        private bool LoadAspectProperties(CloudRequestEngine engine)
        {
            var dictionary = engine.PublicProperties;

            if (dictionary != null &&
                dictionary.Count > 0 &&
                dictionary.ContainsKey(ElementDataKey))
            {
                _aspectProperties = new List<IAspectPropertyMetaData>();
                _dataSourceTier = dictionary[ElementDataKey].DataTier;

                foreach (var item in dictionary[ElementDataKey].Properties)
                {
                    var property = new AspectPropertyMetaData(this,
                        item.Name,
                        item.GetPropertyType(),
                        item.Category,
                        new List<string>(),
                        true);
                    _aspectProperties.Add(property);
                }
                return true;
            }
            else
            {
                _logger.LogError($"Aspect properties could not be loaded for" +
                    $" the Device Detection cloud engine", this);
                return false;
            }
        }
    }
}
