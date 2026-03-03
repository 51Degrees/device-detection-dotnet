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
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// Base class for device detection engines that use property-keyed 
    /// lookups against a <see cref="DeviceDetectionHashEngine"/>.
    /// Implements all abstract methods from 
    /// <see cref="PropertyKeyedEngine{TData, TProfile}"/> specific
    /// to device detection.
    /// </summary>
    public abstract class PropertyKeyedDeviceEngine :
        PropertyKeyedEngine<IMultiDeviceData, IDeviceData>
    {
        private readonly ILogger<MultiDeviceData> _loggerMultiDd;
        private readonly object _dataSetLock = new object();
        private bool _isDataSetInitialized = false;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="loggerFactory">
        /// The factory used to create loggers.
        /// </param>
        /// <param name="indexedProperties">
        /// The properties to be indexed (e.g. "TAC", "NativeModel").
        /// </param>
        protected PropertyKeyedDeviceEngine(
            ILoggerFactory loggerFactory,
            IReadOnlyList<string> indexedProperties) : base(
                loggerFactory,
                indexedProperties)
        {
            _loggerMultiDd = loggerFactory.CreateLogger<MultiDeviceData>();
        }

        /// <summary>
        /// Creates the element data for each request.
        /// </summary>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        protected override IMultiDeviceData CreateElementData(
            IPipeline pipeline)
        {
            EnsureDataSetInitialized(pipeline);
            
            return new MultiDeviceData(
                _loggerMultiDd,
                pipeline,
                this as Pipeline.Engines.FlowElements.IAspectEngine,
                Pipeline.Engines.Services.MissingPropertyService.Instance);
        }

        /// <summary>
        /// Ensures that the <see cref="DataSet"/> is initialized exactly once 
        /// by resolving the <see cref="DeviceDetectionHashEngine"/> from the pipeline.
        /// This is done lazily at runtime because the engine might not be available 
        /// via <see cref="IPipeline.GetElement{TElement}"/> during construction.
        /// </summary>
        /// <param name="pipeline">The pipeline to query for the hash engine.</param>
        private void EnsureDataSetInitialized(IPipeline pipeline)
        {
            if (!_isDataSetInitialized)
            {
                lock (_dataSetLock)
                {
                    if (!_isDataSetInitialized)
                    {
                        var engine = pipeline.GetElement<DeviceDetectionHashEngine>();
                        if (engine != null)
                        {
                            DataSet = BuildContext(engine);
                        }
                        else
                        {
                            _loggerMultiDd.LogWarning("DeviceDetectionHashEngine was not found in the pipeline.");
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
            var flow = DataSet.Pipeline.CreateFlowData();
            flow.AddEvidence(
                Shared.Constants.EVIDENCE_PROFILE_IDS_KEY,
                profileId.ToString());
            flow.Process();
            aspectData.AddFlowData(flow);
        }

        /// <inheritdoc/>
        public override void AddPipeline(IPipeline pipeline)
        {
            base.AddPipeline(pipeline);
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDataSet BuildDataSet(
            string dataFilePath)
        {
            throw new NotSupportedException("BuildDataSet is not supported for this element type. Use AddPipeline instead.");
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDataSet BuildDataSet(
            Stream data)
        {
            throw new NotSupportedException("BuildDataSet is not supported for this element type. Use AddPipeline instead.");
        }

        /// <summary>
        /// Creates an <see cref="PropertyKeyedDataSet"/> from a built 
        /// DeviceDetectionHashEngine.
        /// </summary>
        private PropertyKeyedDataSet BuildContext(
            DeviceDetectionHashEngine engine)
        {
            // Use the pipeline passed to this engine, or fallback to a new one if necessary for context
            var contextPipeline = new PipelineBuilder(LoggerFactory)
                .SetAutoDisposeElements(true)
                .AddFlowElement(engine)
                .Build();

            var indexes = new List<PropertyKeyedIndex>();
            foreach (var property in IndexedProperties)
            {
                var metaData = engine.Properties.FirstOrDefault(p =>
                    p.Name.Equals(property, StringComparison.OrdinalIgnoreCase));
                if (metaData != null)
                {
                    indexes.Add(new PropertyKeyedIndex(
                        new DevicePropertyMetaData(this, metaData)));
                }
            }

            // Populate the indexes using the engine's list of profiles.
            foreach (var profile in engine.Profiles)
            {
                var profileData = contextPipeline.CreateFlowData();
                profileData.AddEvidence(Shared.Constants.EVIDENCE_PROFILE_IDS_KEY, profile.ProfileId.ToString());
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
                        keyProperties.SelectMany(i =>
                            (i.ItemProperties ?? Enumerable.Empty<IElementPropertyMetaData>()).OfType<IFiftyOneAspectPropertyMetaData>())
                            .ToList().AsReadOnly(),
                        typeof(IReadOnlyList<IDeviceData>))
                });
        }

        /// <summary>
        /// Converts the given <see cref="IAspectPropertyValue"/> to a 
        /// collection of string values to be indexed.
        /// </summary>
        /// <param name="value">The aspect property value.</param>
        /// <returns>A collection of string values.</returns>
        private IEnumerable<string> ConvertValues(IAspectPropertyValue value)
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
