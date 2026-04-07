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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.PropertyKeyed.Data;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Services
{
    /// <summary>
    /// Concrete implementation of the data set service used to create property
    /// value indexed data sets for flow element processing.
    /// </summary>
    public class DataSetService : IDataSetService
    {
        private readonly IPropertyValueQueryService _queryService;
        private readonly ILogger<DataSetService> _logger;

        /// <summary>
        /// Constructs a new instance of <see cref="DataSetService"/>.
        /// </summary>
        /// <param name="queryService">
        /// Service used to get profile and device data from the 
        /// <see cref="DeviceDetectionHashEngine"/>.
        /// </param>
        /// <param name="logger"></param>
        public DataSetService(
            IPropertyValueQueryService queryService,
            ILogger<DataSetService> logger)
        {
            _queryService = queryService;
            _logger = logger;
        }

        /// <inheritdoc/>
        public PropertyKeyedDataSet BuildDataSet(
            IFlowElement element,
            IPipeline pipeline)
        {
            var engine = pipeline.GetDeviceDetectionHashEngine();

            // Create lookup dictionary for O(1) property access
            var propertyLookup = engine.Properties
                .ToDictionary(
                    p => p.Name,
                    p => p,
                    StringComparer.OrdinalIgnoreCase);

            // Create keys for the indexed properties.
            var indexes = new List<PropertyKeyedIndex>();
            foreach (var property in _queryService.IndexedProperties)
            {
                if (propertyLookup.TryGetValue(property, out var metaData))
                {
                    indexes.Add(new PropertyKeyedIndex(
                        new DevicePropertyMetaData(element, metaData)));
                }
                else
                {
                    _logger.LogWarning(
                        "Property '{0}' not found in '{1}. It will not be " +
                        "indexed.",
                        property,
                        engine.ElementDataKey);
                }
            }

            // Create the context pipeline.
            var contextPipeline = _queryService.CreateContext(engine);

            // Populate the indexes.
            foreach (var item in _queryService.Query(contextPipeline))
            {
                ProcessProfile(item, indexes);
            }

            // Collect all engine properties to expose as sub-properties
            // of "Profiles". Each is wrapped with DevicePropertyMetaData
            // that re-associates the property with the outer
            // property-keyed engine instead of the inner
            // DeviceDetectionHashEngine.
            var profileProperties = engine.Properties
                .Select(p => (IFiftyOneAspectPropertyMetaData)
                    new DevicePropertyMetaData(element, p))
                .ToList()
                .AsReadOnly();

            // Build and return the data set.
            return new PropertyKeyedDataSet(
                contextPipeline,
                element.ElementDataKey,
                engine.DataSourceTier,
                element,
                indexes,
                (_, elem) => BuildProperties(profileProperties, elem));
        }

        /// <summary>
        /// Extracts any relevant values from the profile and adds when as
        /// keys for the profile.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="indexes"></param>
        private void ProcessProfile(
            (IProfileMetaData Profile, IDeviceData Data) item,
            List<PropertyKeyedIndex> indexes)
        {
            foreach (var index in indexes)
            {
                var value = item.Data[index.MetaData.Name]
                    as IAspectPropertyValue;
                if (value != null && value.HasValue)
                {
                    foreach (var val in ConvertValues(value))
                    {
                        index.Add(val, item.Profile.ProfileId);
                    }
                }
                else
                {
                    _logger.LogWarning(
                        "Profile '{0}' missing property '{1}'",
                        item.Profile,
                        index.MetaData.Name);
                }
            }
        }

        /// <summary>
        /// Converts the given IAspectPropertyValue to an enumeration of string
        /// values to be indexed.
        /// </summary>
        /// <param name="value">The aspect property value.</param>
        /// <returns>An enumeration of string values.</returns>
        private static IEnumerable<string> ConvertValues(
            IAspectPropertyValue value)
        {
            if (value.Value is IEnumerable<string> list)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
            else
            {
                yield return value.Value.ToString();
            }
        }

        private static List<IFiftyOneAspectPropertyMetaData> BuildProperties(
            IReadOnlyList<IFiftyOneAspectPropertyMetaData> profileProperties,
            IFlowElement element)
        {
            return new List<IFiftyOneAspectPropertyMetaData>
            {
                new DevicePropertyMetaData(
                    element,
                    PropertyKeyedDataSet.PROPERTY_PREFIX_NAME,
                    profileProperties,
                    typeof(IReadOnlyList<IDeviceData>))
            };
        }
    }
}