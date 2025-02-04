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

using FiftyOne.DeviceDetection.Cloud.Data;
using FiftyOne.Pipeline.CloudRequestEngine.Data;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.Cloud.FlowElements
{
    /// <summary>
    /// Engine that takes the JSON response from the 
    /// <see cref="CloudRequestEngine"/> and uses it populate a 
    /// DeviceDataCloud instance for easier consumption.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/pipeline-elements/device-detection-cloud.md">Specification</see>
    /// </summary>
    public class DeviceDetectionCloudEngine : CloudAspectEngineBase<DeviceDataCloud>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">
        /// The logger for this instance to use
        /// </param>
        /// <param name="deviceDataFactory">
        /// Factory function to use when creating aspect data instances.
        /// </param>
        public DeviceDetectionCloudEngine(
            ILogger<DeviceDetectionCloudEngine> logger,
            Func<IPipeline, FlowElementBase<DeviceDataCloud, IAspectPropertyMetaData>, DeviceDataCloud> deviceDataFactory)
            : base(logger,
                  deviceDataFactory)
        {
        }

        /// <summary>
        /// The key to use for storing this engine's data in a 
        /// <see cref="IFlowData"/> instance.
        /// </summary>
        public override string ElementDataKey => "device";

        /// <summary>
        /// The filter that defines the evidence that is used by 
        /// this engine.
        /// This engine needs no evidence as it works from the response
        /// from the <see cref="ICloudRequestEngine"/>.
        /// </summary>
        public override IEvidenceKeyFilter EvidenceKeyFilter =>
            new EvidenceKeyFilterWhitelist(new List<string>());

        private static JsonConverter[] JSON_CONVERTERS = new JsonConverter[]
        {
            new CloudJsonConverter()
        };

        /// <summary>
        /// Perform the processing for this engine:
        /// 1. Extract properties relevant to this engine from the JSON.
        /// 2. Deserialize JSON data to populate a 
        /// <see cref="DeviceDataCloud"/> instance.
        /// </summary>
        /// <param name="data">
        /// The <see cref="IFlowData"/> instance containing data for the 
        /// current request.
        /// </param>
        /// <param name="aspectData">
        /// The <see cref="DeviceDataCloud"/> instance to populate with
        /// values.
        /// </param>
        /// <param name="json">
        /// The JSON data from the <see cref="CloudRequestEngine"/> 
        /// response.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if a required parameter is null
        /// </exception>
        protected override void ProcessCloudEngine(IFlowData data, DeviceDataCloud aspectData, string json)
        {
            if (aspectData == null) throw new ArgumentNullException(nameof(aspectData));

            // Extract data from JSON to the aspectData instance.
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            var propertyValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(dictionary["device"].ToString(),
                new JsonSerializerSettings()
                {
                    Converters = JSON_CONVERTERS,
                });

            var device = CreateAPVDictionary(propertyValues, Properties.ToList());
            aspectData.PopulateFrom(device);
        }

        protected override Type GetPropertyType(
            PropertyMetaData propertyMetaData,
            Type parentObjectType)
        {
            if (propertyMetaData == null)
            {
                throw new ArgumentNullException(nameof(propertyMetaData));
            }
            if (DeviceDataCloud.TryGetPropertyType(
                    propertyMetaData.Name,
                    out var type))
            {
                return type;
            }
            else
            {
                return base.GetPropertyType(propertyMetaData, parentObjectType);
            }
        }
    }
}
