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
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
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
    public class PropertyValueQueryService : IPropertyValueQueryService
    {
        /// <inheritdoc/>
        public IReadOnlyList<string> IndexedProperties { get; }

        private readonly ILoggerFactory _loggerFactory;

        private readonly ILogger<PropertyValueQueryService> _logger;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="PropertyValueQueryService"/>.
        /// </summary>
        /// <param name="indexedProperties">
        /// The properties in the <see cref="DeviceDetectionHashEngine"/> to
        /// index.
        /// </param>
        /// <param name="loggerFactory"></param>
        public PropertyValueQueryService(
            IReadOnlyList<string> indexedProperties,
            ILoggerFactory loggerFactory)
        {
            IndexedProperties = indexedProperties;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<PropertyValueQueryService>();
        }

        /// <inheritdoc/>
        public IPipeline CreateContext(DeviceDetectionHashEngine engine)
        {
            return new PipelineBuilder(_loggerFactory)
                // Don't dispose the shared engine
                .SetAutoDisposeElements(false)
                .AddFlowElement(engine)
                .Build();
        }

        /// <inheritdoc/>
        public IEnumerable<(IProfileMetaData, IDeviceData)> Query(
            IPipeline contextPipeline)
        {
            var engine = contextPipeline.GetDeviceDetectionHashEngine();

            // Get the components that relate to the properties being queried.
            var components = engine.Properties.Where(i => 
                IndexedProperties.Contains(
                    i.Name, 
                    StringComparer.InvariantCultureIgnoreCase)).Select(i => 
                    i.Component).ToHashSet();

            // Populate the indexes using the engine's list of profiles
            // considering only those that relate to the required properties
            // components.
            foreach (var profile in engine.Profiles.Where(i => 
                components.Contains(i.Component)))
            {
                using var data = contextPipeline.CreateFlowData();

                data.AddEvidence(
                    Shared.Constants.EVIDENCE_PROFILE_IDS_KEY,
                    profile.ProfileId.ToString());
                data.Process();

                var dd = data.Get<IDeviceData>();
                if (dd != null)
                {
                    yield return (profile, dd);
                }
                else
                {
                    _logger.LogWarning(
                        "Profile '{0}' missing {1}",
                        profile,
                        nameof(IDeviceData));
                }
            }
        }
    }
}
