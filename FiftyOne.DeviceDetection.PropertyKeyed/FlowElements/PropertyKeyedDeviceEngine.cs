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
        private readonly IEngineProvider _engineProvider;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="loggerFactory">
        /// The factory used to create loggers.
        /// </param>
        /// <param name="indexedProperties">
        /// The properties to be indexed (e.g. "TAC", "NativeModel").
        /// </param>
        /// <param name="engineProvider">
        /// The provider used to obtain a <see cref="DeviceDetectionHashEngine"/> instance.
        /// If null, a default <see cref="NewEngineProvider"/> is used.
        /// </param>
        protected PropertyKeyedDeviceEngine(
            ILoggerFactory loggerFactory,
            IReadOnlyList<string> indexedProperties,
            IEngineProvider engineProvider = null) : base(
                loggerFactory,
                indexedProperties)
        {
            _loggerMultiDd = loggerFactory.CreateLogger<MultiDeviceData>();
            _engineProvider = engineProvider ?? new NewEngineProvider(loggerFactory);
        }

        /// <summary>
        /// Creates the element data for each request.
        /// </summary>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        protected override IMultiDeviceData CreateElementData(
            IPipeline pipeline)
        {
            return new MultiDeviceData(
                _loggerMultiDd,
                pipeline,
                this as Pipeline.Engines.FlowElements.IAspectEngine,
                Pipeline.Engines.Services.MissingPropertyService.Instance);
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
        protected override PropertyKeyedDataSet BuildDataSet(
            string dataFilePath)
        {
            var engine = _engineProvider.GetEngine(dataFilePath, null);
            return BuildContext(engine);
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDataSet BuildDataSet(
            Stream data)
        {
            var engine = _engineProvider.GetEngine(null, data);
            return BuildContext(engine);
        }

        /// <summary>
        /// Creates an <see cref="PropertyKeyedDataSet"/> from a built 
        /// DeviceDetectionHashEngine.
        /// </summary>
        private PropertyKeyedDataSet BuildContext(
            DeviceDetectionHashEngine engine)
        {
            var pipeline = new PipelineBuilder(LoggerFactory)
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
                var profileData = pipeline.CreateFlowData();
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
                pipeline,
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
