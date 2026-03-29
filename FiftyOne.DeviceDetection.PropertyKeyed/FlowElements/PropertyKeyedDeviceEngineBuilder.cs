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
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// Builder for <see cref="PropertyKeyedDeviceEngine"/>.
    /// Supports configuring the key property, validation, and element data key.
    /// </summary>
    public class PropertyKeyedDeviceEngineBuilder :
        PropertyKeyedEngineBuilderBase<
            PropertyKeyedDeviceEngineBuilder,
            PropertyKeyedDeviceEngine>
    {
        private string _keyProperty;
        private string _elementDataKey;
        private Func<string, IFlowData, bool> _validator;

        /// <summary>
        /// Constructs a new instance of <see cref="PropertyKeyedDeviceEngineBuilder"/>.
        /// </summary>
        /// <param name="loggerFactory">The factory used to create loggers.</param>
        /// <param name="dataUpdateService">The data update service, if any.</param>
        public PropertyKeyedDeviceEngineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService = null)
            : base(loggerFactory, dataUpdateService)
        {
        }

        /// <summary>
        /// Sets the property name to use for lookups.
        /// This is required before building the engine.
        /// </summary>
        /// <param name="keyProperty">The property name (e.g. "TAC", "NativeModel").</param>
        /// <returns>This builder.</returns>
        public PropertyKeyedDeviceEngineBuilder SetKeyProperty(string keyProperty)
        {
            _keyProperty = keyProperty;
            return this;
        }

        /// <summary>
        /// Sets the unique key for this engine's data in flow data.
        /// If not set, defaults to "profiles-{keyProperty}".
        /// </summary>
        /// <param name="elementDataKey">The element data key.</param>
        /// <returns>This builder.</returns>
        public PropertyKeyedDeviceEngineBuilder SetElementDataKey(string elementDataKey)
        {
            _elementDataKey = elementDataKey;
            return this;
        }

        /// <summary>
        /// Sets a custom validation function for the key property value.
        /// The function receives the value and IFlowData, and should return
        /// true if valid, or false (and optionally add errors to flow data).
        /// </summary>
        /// <param name="validator">The validation function.</param>
        /// <returns>This builder.</returns>
        public PropertyKeyedDeviceEngineBuilder SetValidator(
            Func<string, IFlowData, bool> validator)
        {
            _validator = validator;
            return this;
        }

        /// <inheritdoc/>
        public override PropertyKeyedDeviceEngineBuilder SetPerformanceProfile(
            Pipeline.Engines.PerformanceProfiles profile)
        {
            _logger.LogWarning(Messages.PerformanceProfileNotSupported);
            return this;
        }

        /// <summary>
        /// Sets the concurrency. Not supported for property keyed engines.
        /// </summary>
        /// <param name="concurrency">The concurrency value (ignored).</param>
        /// <returns>This builder.</returns>
        public PropertyKeyedDeviceEngineBuilder SetConcurrency(ushort concurrency)
        {
            _logger.LogWarning(Messages.ConcurrencyNotSupported);
            return this;
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDeviceEngine CreateEngine(
            List<string> properties)
        {
            if (string.IsNullOrEmpty(_keyProperty))
            {
                throw new PipelineConfigurationException(
                    "KeyProperty must be set before building. " +
                    "Call SetKeyProperty() or use a specialized builder " +
                    "such as TacEngineBuilder or NativeModelEngineBuilder.");
            }

            var elementDataKey = _elementDataKey ?? 
                $"profiles-{_keyProperty.ToLowerInvariant()}";

            return new PropertyKeyedDeviceEngine(
                _loggerFactory,
                properties,
                _keyProperty,
                elementDataKey,
                _validator);
        }

        /// <summary>
        /// Build the engine without a data file.
        /// PropertyKeyedDeviceEngine does not use a DataFile directly — 
        /// it resolves its data from DeviceDetectionHashEngine via 
        /// AddPipeline at runtime.
        /// </summary>
        /// <returns>The built engine.</returns>
        public new PropertyKeyedDeviceEngine Build()
        {
            return BuildEngine();
        }

    }
}