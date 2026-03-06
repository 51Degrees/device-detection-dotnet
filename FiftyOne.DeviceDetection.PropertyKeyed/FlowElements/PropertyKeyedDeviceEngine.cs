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
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// A generic device detection engine that uses property-keyed 
    /// lookups against a <see cref="DeviceDetectionHashEngine"/>.
    /// Can be configured to validate input and use any property as the key.
    /// </summary>
    public class PropertyKeyedDeviceEngine :
        PropertyKeyedEngine<IMultiDeviceData, IDeviceData>
    {
        private readonly ILogger<PropertyKeyedDeviceEngine> _logger;
        private readonly ILogger<MultiDeviceData> _loggerMultiDd;
        private readonly string _keyProperty;
        private readonly string _elementDataKeyValue;
        private readonly Func<string, IFlowData, bool> _validator;
        private readonly object _dataSetLock = new object();
        private bool _isDataSetInitialized;

        /// <inheritdoc/>
        public override string ElementDataKey => _elementDataKeyValue;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="loggerFactory">The factory used to create loggers.</param>
        /// <param name="indexedProperties">The properties to be indexed.</param>
        /// <param name="keyProperty">The property name to use for lookups (e.g. "TAC", "NativeModel").</param>
        /// <param name="elementDataKey">The unique key for this engine's data in flow data.</param>
        /// <param name="validator">Optional validation function for the key property value. Returns true if valid.</param>
        public PropertyKeyedDeviceEngine(
            ILoggerFactory loggerFactory,
            IReadOnlyList<string> indexedProperties,
            string keyProperty,
            string elementDataKey,
            Func<string, IFlowData, bool> validator = null) : base(
                loggerFactory,
                indexedProperties)
        {
            if (string.IsNullOrEmpty(keyProperty))
            {
                throw new ArgumentException(
                    "keyProperty is required and cannot be null or empty.",
                    nameof(keyProperty));
            }
            if (string.IsNullOrEmpty(elementDataKey))
            {
                throw new ArgumentException(
                    "elementDataKey is required and cannot be null or empty.",
                    nameof(elementDataKey));
            }

            _logger = loggerFactory.CreateLogger<PropertyKeyedDeviceEngine>();
            _loggerMultiDd = loggerFactory.CreateLogger<MultiDeviceData>();
            _keyProperty = keyProperty;
            _elementDataKeyValue = elementDataKey;
            _validator = validator ?? ((value, flowData) => true);
        }

        /// <inheritdoc/>
        protected override string GetKeyPropertyName() => _keyProperty;

        /// <inheritdoc/>
        protected override bool Validate(string keyPropertyValue, IFlowData data)
        {
            return _validator(keyPropertyValue, data);
        }

        /// <summary>
        /// Creates the element data for each request.
        /// </summary>
        /// <param name="pipeline">The pipeline instance.</param>
        /// <returns>A new MultiDeviceData instance.</returns>
        protected override IMultiDeviceData CreateElementData(
            IPipeline pipeline)
        {
            EnsureDataSetInitialized(pipeline);
            
            return new MultiDeviceData(
                _loggerMultiDd,
                pipeline,
                this,
                Pipeline.Engines.Services.MissingPropertyService.Instance,
                DataSet);
        }

        /// <summary>
        /// Called when pipeline is added. Attempts early initialization.
        /// </summary>
        /// <param name="pipeline">The pipeline being added.</param>
        public override void AddPipeline(IPipeline pipeline)
        {
            base.AddPipeline(pipeline);

            // Attempt early initialization with fail-fast
            var engine = pipeline.GetElement<DeviceDetectionHashEngine>();
            if (engine == null)
            {
                throw new PipelineConfigurationException(
                    $"{GetType().Name} requires DeviceDetectionHashEngine to be present " +
                    "in the pipeline. Ensure DeviceDetectionHashEngine is added before " +
                    "this engine.");
            }

            lock (_dataSetLock)
            {
                if (_isDataSetInitialized == false)
                {
                    DataSet = BuildContext(engine, pipeline);
                    _isDataSetInitialized = true;
                }
            }
        }

        /// <summary>
        /// Ensures that the DataSet is initialized.
        /// This is a fallback in case AddPipeline wasn't called.
        /// </summary>
        /// <param name="pipeline">The pipeline to query for the hash engine.</param>
        private void EnsureDataSetInitialized(IPipeline pipeline)
        {
            if (_isDataSetInitialized == false)
            {
                lock (_dataSetLock)
                {
                    if (_isDataSetInitialized == false)
                    {
                        var engine = pipeline.GetElement<DeviceDetectionHashEngine>();
                        if (engine != null)
                        {
                            DataSet = BuildContext(engine, pipeline);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "DeviceDetectionHashEngine was not found in the pipeline. " +
                                "Profile lookups will not work.");
                        }
                        _isDataSetInitialized = true;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void ProcessProfileMatch(
            IFlowData data,
            IMultiDeviceData aspectData,
            uint profileId)
        {
            aspectData.AddProfileId(profileId);
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDataSet BuildDataSet(
            string dataFilePath)
        {
            throw new NotSupportedException(
                $"{GetType().Name} does not support direct data file loading. " +
                "It resolves data from DeviceDetectionHashEngine in the pipeline. " +
                "Ensure DeviceDetectionHashEngine is added to the pipeline before this engine.");
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDataSet BuildDataSet(
            Stream data)
        {
            throw new NotSupportedException(
                $"{GetType().Name} does not support direct data stream loading. " +
                "It resolves data from DeviceDetectionHashEngine in the pipeline. " +
                "Ensure DeviceDetectionHashEngine is added to the pipeline before this engine.");
        }

        /// <summary>
        /// Creates a PropertyKeyedDataSet from a DeviceDetectionHashEngine.
        /// </summary>
        /// <param name="engine">The hash engine to build from.</param>
        /// <param name="pipeline">The pipeline for context.</param>
        /// <returns>A new PropertyKeyedDataSet.</returns>
        private PropertyKeyedDataSet BuildContext(
            DeviceDetectionHashEngine engine,
            IPipeline pipeline)
        {
            // Create lookup dictionary for O(1) property access
            var propertyLookup = engine.Properties
                .ToDictionary(
                    p => p.Name,
                    p => p,
                    StringComparer.OrdinalIgnoreCase);

            var indexes = new List<PropertyKeyedIndex>();
            foreach (var property in IndexedProperties)
            {
                if (propertyLookup.TryGetValue(property, out var metaData))
                {
                    indexes.Add(new PropertyKeyedIndex(
                        new DevicePropertyMetaData(this, metaData)));
                }
                else
                {
                    _logger.LogWarning(
                        "Property '{Property}' not found in DeviceDetectionHashEngine. " +
                        "It will not be indexed.",
                        property);
                }
            }

            // Build context pipeline for profile resolution
            var contextPipeline = new PipelineBuilder(LoggerFactory)
                .SetAutoDisposeElements(false) // Don't dispose the shared engine
                .AddFlowElement(engine)
                .Build();

            // Populate the indexes using the engine's list of profiles
            foreach (var profile in engine.Profiles)
            {
                using (var profileData = contextPipeline.CreateFlowData())
                {
                    profileData.AddEvidence(
                        Shared.Constants.EVIDENCE_PROFILE_IDS_KEY,
                        profile.ProfileId.ToString());
                    profileData.Process();

                    var dd = profileData.GetFromElement(engine) as IDeviceData;
                    if (dd != null)
                    {
                        foreach (var index in indexes)
                        {
                            var value = dd[index.MetaData.Name] as IAspectPropertyValue;
                            if (value != null)
                            {
                                var values = ConvertValues(value);
                                foreach (var val in values)
                                {
                                    index.Add(val, profile.ProfileId);
                                }
                            }
                        }
                    }
                }
            }

            return new PropertyKeyedDataSet(
                contextPipeline,
                engine.ElementDataKey,
                engine.DataSourceTier,
                this,
                indexes,
                (keyProperties, e) => new List<IFiftyOneAspectPropertyMetaData>
                {
                    new DevicePropertyMetaData(
                        e,
                        PropertyKeyedDataSet.PROPERTY_PREFIX_NAME,
                        keyProperties
                            .SelectMany(i =>
                                (i.ItemProperties ?? Enumerable.Empty<IElementPropertyMetaData>())
                                .OfType<IFiftyOneAspectPropertyMetaData>())
                            .ToList()
                            .AsReadOnly(),
                        typeof(IReadOnlyList<IDeviceData>))
                });
        }

        /// <summary>
        /// Converts the given IAspectPropertyValue to a 
        /// collection of string values to be indexed.
        /// </summary>
        /// <param name="value">The aspect property value.</param>
        /// <returns>A collection of string values.</returns>
        private static IEnumerable<string> ConvertValues(IAspectPropertyValue value)
        {
            if (value.HasValue)
            {
                if (value.Value is IEnumerable<string> list)
                {
                    return list;
                }
                return new[] { value.Value.ToString() };
            }
            return Enumerable.Empty<string>();
        }
    }
}
