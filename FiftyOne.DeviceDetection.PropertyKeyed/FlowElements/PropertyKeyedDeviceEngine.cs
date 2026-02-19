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
        private readonly Pipeline.Engines.PerformanceProfiles _performanceProfile;
        private readonly ushort _concurrency;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="indexedProperties">
        /// The properties to be indexed (e.g. "TAC", "NativeModel").
        /// </param>
        /// <param name="performanceProfile">
        /// Performance profile for the inner DeviceDetectionHashEngine.
        /// </param>
        /// <param name="concurrency">
        /// Expected concurrent operations count.
        /// </param>
        protected PropertyKeyedDeviceEngine(
            ILoggerFactory loggerFactory,
            IReadOnlyList<string> indexedProperties,
            Pipeline.Engines.PerformanceProfiles performanceProfile,
            ushort concurrency) : base(
                loggerFactory,
                indexedProperties)
        {
            _loggerMultiDd = loggerFactory.CreateLogger<MultiDeviceData>();
            _performanceProfile = performanceProfile;
            _concurrency = concurrency;
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
        protected override InnerEngineContext CreateInnerEngine(
            string dataFilePath)
        {
            var engine = CreateHashEngineBuilder()
                .Build(dataFilePath, false);
            return BuildContext(engine);
        }

        /// <inheritdoc/>
        protected override InnerEngineContext CreateInnerEngine(
            Stream data)
        {
            var engine = CreateHashEngineBuilder().Build(data);
            return BuildContext(engine);
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetProfilePropertyValues(
            IElementData data,
            IFiftyOneAspectPropertyMetaData property)
        {
            var dd = data as IDeviceData;
            if (dd != null)
            {
                var value = dd[property.Name] as IAspectPropertyValue;
                if (value != null)
                {
                    return ConvertValues(value);
                }
            }
            return Enumerable.Empty<string>();
        }

        /// <inheritdoc/>
        protected override IList<IFiftyOneAspectPropertyMetaData>
            BuildResultProperties(
                IList<IFiftyOneAspectPropertyMetaData> keyProperties,
                IFlowElement engine)
        {
            return new List<IFiftyOneAspectPropertyMetaData>
            {
                new DevicePropertyMetaData(
                    engine,
                    PropertyKeyedDataSet.PROPERTY_PREFIX_NAME,
                    keyProperties.SelectMany(i =>
                        i.ItemProperties.OfType<IFiftyOneAspectPropertyMetaData>())
                        .ToList().AsReadOnly(),
                    typeof(IReadOnlyList<IDeviceData>))
            };
        }

        /// <inheritdoc/>
        protected override PropertyKeyedIndex CreatePropertyIndex(
            IFiftyOneAspectPropertyMetaData source)
        {
            return new PropertyKeyedIndex(
                new DevicePropertyMetaData(this, source));
        }

        /// <summary>
        /// Creates and configures the DeviceDetectionHashEngineBuilder.
        /// </summary>
        /// <returns></returns>
        private DeviceDetectionHashEngineBuilder CreateHashEngineBuilder()
        {
            var builder = new DeviceDetectionHashEngineBuilder(LoggerFactory);
            builder.SetAutoUpdate(false);
            builder.SetDataFileSystemWatcher(false);
            builder.SetConcurrency(_concurrency);
            builder.SetPerformanceProfile(_performanceProfile);
            return builder;
        }

        /// <summary>
        /// Creates an <see cref="InnerEngineContext"/> from a built 
        /// DeviceDetectionHashEngine.
        /// </summary>
        private InnerEngineContext BuildContext(
            DeviceDetectionHashEngine engine)
        {
            var pipeline = new PipelineBuilder(LoggerFactory)
                .SetAutoDisposeElements(true)
                .AddFlowElement(engine)
                .Build();

            return new InnerEngineContext
            {
                Pipeline = pipeline,
                InnerEngineDataKey = engine.ElementDataKey,
                ElementDataKey = engine.ElementDataKey,
                DataSourceTier = engine.DataSourceTier,
                Profiles = engine.Profiles,
                ProfileIdEvidenceKey =
                    Shared.Constants.EVIDENCE_PROFILE_IDS_KEY,
                Properties = engine.Properties
            };
        }
    }
}
