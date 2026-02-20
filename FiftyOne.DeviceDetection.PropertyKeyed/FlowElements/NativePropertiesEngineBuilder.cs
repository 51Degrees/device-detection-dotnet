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

using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// Builder for <see cref="NativePropertiesEngine"/>.
    /// </summary>
    public class NativePropertiesEngineBuilder :
        PropertyKeyedEngineBuilderBase<
            NativePropertiesEngineBuilder,
            NativePropertiesEngine>
    {
        private PerformanceProfiles _performanceProfile =
            PerformanceProfiles.MaxPerformance;
        private ushort _concurrency = 2;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public NativePropertiesEngineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService = null)
            : base(loggerFactory, dataUpdateService)
        {
        }

        /// <inheritdoc/>
        public override NativePropertiesEngineBuilder SetPerformanceProfile(
            PerformanceProfiles profile)
        {
            _performanceProfile = profile;
            return this;
        }

        /// <summary>
        /// Sets the performance profile from a string value.
        /// </summary>
        public NativePropertiesEngineBuilder SetPerformanceProfile(
            string profile)
        {
            foreach (var matched in Enum.GetValues(typeof(PerformanceProfiles))
                .Cast<PerformanceProfiles>()
                .Where(i => profile.Equals(i.ToString())))
            {
                return SetPerformanceProfile(matched);
            }
            throw new PipelineConfigurationException(
                String.Format(
                    "'{0}' is not a valid PerformanceProfiles", profile));
        }

        /// <summary>
        /// Sets the concurrency.
        /// </summary>
        public NativePropertiesEngineBuilder SetConcurrency(
            ushort concurrency)
        {
            _concurrency = concurrency;
            return this;
        }

        /// <inheritdoc/>
        protected override NativePropertiesEngine CreateEngine(
            List<string> properties)
        {
            return new NativePropertiesEngine(
                _loggerFactory,
                properties,
                _performanceProfile,
                _concurrency);
        }
    }
}
