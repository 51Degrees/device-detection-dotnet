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

using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// Builder for creating a <see cref="PropertyKeyedDeviceEngine"/>
    /// configured for TAC (Type Allocation Code) lookups.
    /// TAC values must be exactly 8 numeric digits.
    /// </summary>
    public class TacEngineBuilder :
        PropertyKeyedEngineBuilderBase<
            TacEngineBuilder,
            PropertyKeyedDeviceEngine>
    {
        /// <summary>
        /// Constructs a new instance of <see cref="TacEngineBuilder"/>.
        /// </summary>
        /// <param name="loggerFactory">The factory used to create loggers.</param>
        /// <param name="dataUpdateService">The data update service, if any.</param>
        public TacEngineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService = null)
            : base(loggerFactory, dataUpdateService)
        {
            SetProperty("TAC");
        }

        /// <inheritdoc/>
        public override TacEngineBuilder SetPerformanceProfile(
            Pipeline.Engines.PerformanceProfiles profile)
        {
            _logger.LogWarning(Messages.PerformanceProfileNotSupported);
            return this;
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDeviceEngine CreateEngine(
            List<string> properties)
        {
            return new PropertyKeyedDeviceEngine(
                _loggerFactory,
                properties,
                "TAC",
                "tac-profiles",
                ValidateTac);
        }

        /// <summary>
        /// Build the engine without a data file.
        /// The engine resolves its data from DeviceDetectionHashEngine
        /// via AddPipeline at runtime.
        /// </summary>
        /// <returns>The built engine.</returns>
        public new PropertyKeyedDeviceEngine Build()
        {
            return BuildEngine();
        }

        /// <summary>
        /// Validates TAC format: must be exactly 8 numeric digits.
        /// </summary>
        private static bool ValidateTac(string value, IFlowData data)
        {
            if (value.Length == 8 && int.TryParse(value, out _))
            {
                return true;
            }

            data.AddError(
                new ArgumentException(string.Format(
                    Messages.IncorrectTacEvidence,
                    value)),
                data.Pipeline.GetElement<PropertyKeyedDeviceEngine>());
            return false;
        }
    }
}
