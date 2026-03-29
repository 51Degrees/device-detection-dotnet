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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Services
{
    public class DataSetService
    {
        /// <summary>
        /// The properties in the <see cref="DeviceDetectionHashEngine"/> to
        /// index.
        /// </summary>
        public IReadOnlyList<string> IndexedProperties { get; }

        private readonly LoggerFactory _loggerFactory;

        private readonly ILogger<DataSetService> _logger;

        /// <summary>
        /// Constructs a new instance of <see cref="DataSetService"/>.
        /// </summary>
        /// <param name="indexedProperties">
        /// The properties in the <see cref="DeviceDetectionHashEngine"/> to
        /// index.
        /// </param>
        /// <param name="loggerFactory"></param>
        public DataSetService(
            IReadOnlyList<string> indexedProperties,
            LoggerFactory loggerFactory)
        {
            IndexedProperties = indexedProperties;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<DataSetService>();
        }

        /// <inheritdoc/>
        public async Task<PropertyKeyedDataSet> BuildDataSet(
            IFlowElement element,
            IPipeline pipeline,
            CancellationToken stopToken)
        {
            var engine = pipeline.GetElement<DeviceDetectionHashEngine>();
            if (engine == null)
            {
                throw new PipelineConfigurationException(
                    $"{GetType().Name} requires a " +
                    $"{nameof(DeviceDetectionHashEngine)} " +
                    $"instance to be present in the pipeline. Ensure " +
                    $"{nameof(DeviceDetectionHashEngine)} is added.");
            }
            return await BuildDataSet(element, engine, stopToken);
        }

        /// <inheritdoc/>
        public async Task<PropertyKeyedDataSet> BuildDataSet(
            IFlowElement element,
            DeviceDetectionHashEngine engine,
            CancellationToken stopToken)
        {
            // Create lookup dictionary for O(1) property access
            var propertyLookup = engine.Properties
                .ToDictionary(
                    p => p.Name,
                    p => p,
                    StringComparer.OrdinalIgnoreCase);

            // Create keys for the indexed properties.
            var indexes = new List<PropertyKeyedIndex>();
            foreach (var property in IndexedProperties)
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

            // Build context pipeline for profile resolution
            var contextPipeline = new PipelineBuilder(_loggerFactory)
                .SetAutoDisposeElements(false) // Don't dispose the shared engine
                .AddFlowElement(engine)
                .Build();

            // Populate the indexes using the engine's list of profiles
            Parallel.ForEach(
                engine.Profiles,
                new ParallelOptions() { CancellationToken = stopToken },
                profile => ProcessProfile(
                    engine,
                    profile,
                    indexes,
                    contextPipeline));

            return new PropertyKeyedDataSet(
                contextPipeline,
                engine.ElementDataKey,
                engine.DataSourceTier,
                element,
                indexes,
                BuildProperties);
        }

        private static List<IFiftyOneAspectPropertyMetaData> BuildProperties(
            IList<IFiftyOneAspectPropertyMetaData> itemProperties, 
            IFlowElement element)
        {
            return new List<IFiftyOneAspectPropertyMetaData>
                {
                    new DevicePropertyMetaData(
                        element,
                        PropertyKeyedDataSet.PROPERTY_PREFIX_NAME,
                        itemProperties.SelectMany(i =>
                            (i.ItemProperties ?? 
                            Enumerable.Empty<IElementPropertyMetaData>())
                            .OfType<IFiftyOneAspectPropertyMetaData>())
                            .ToList()
                            .AsReadOnly(),
                        typeof(IReadOnlyList<IDeviceData>))
                };
        }

        /// <summary>
        /// Extracts any relevant values from the profile and adds when as
        /// keys for the profile.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="profile"></param>
        /// <param name="indexes"></param>
        /// <param name="contextPipeline"></param>
        private void ProcessProfile(
            DeviceDetectionHashEngine engine, 
            IProfileMetaData profile, 
            List<PropertyKeyedIndex> indexes, 
            IPipeline contextPipeline)
        {
            using var data = contextPipeline.CreateFlowData();

            data.AddEvidence(
                Shared.Constants.EVIDENCE_PROFILE_IDS_KEY,
                profile.ProfileId.ToString());
            data.Process();

            var dd = data.Get<IDeviceData>();
            if (dd != null)
            {
                foreach (var index in indexes)
                {
                    var value = dd[index.MetaData.Name] as IAspectPropertyValue;
                    if (value != null && value.HasValue)
                    {
                        lock (index)
                        {
                            foreach (var val in ConvertValues(value))
                            {
                                index.Add(val, profile.ProfileId);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Profile id '{0}' missing property '{1}'",
                            profile.ProfileId,
                            index.MetaData.Name);
                    }
                }
            }
            else
            {
                _logger.LogWarning(
                    "Profile id '{0}' missing {1}", 
                    profile.ProfileId,
                    nameof(IDeviceData));
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
                foreach(var item in value.Value as IEnumerable<string>)
                {
                    yield return item;
                }
            }
            else
            {
                yield return value.Value.ToString();
            }
        }
    }
}
