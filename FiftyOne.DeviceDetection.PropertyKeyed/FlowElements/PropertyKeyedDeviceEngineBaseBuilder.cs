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

using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.FlowElements;
using Microsoft.Extensions.Logging;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// Builder for <see cref="PropertyKeyedDeviceBaseEngine"/>.
    /// Supports configuring the key property and element data key.
    /// </summary>
    public abstract class
        PropertyKeyedDeviceEngineBaseBuilder<TBuilder, TEngine> :
        PropertyKeyedEngineBuilderBase<TBuilder, TEngine>
        where TBuilder : PropertyKeyedDeviceEngineBaseBuilder<TBuilder, TEngine>
        where TEngine : IAspectEngine
    {
        protected string _keyProperty;
        protected string _elementDataKey;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="PropertyKeyedDeviceEngineBaseBuilder{TBuilder, TEngine}"/>.
        /// </summary>
        /// <param name="loggerFactory">
        /// The factory used to create loggers.
        /// </param>
        protected PropertyKeyedDeviceEngineBaseBuilder(
            ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Sets the property name to use for lookups. This is required before
        /// building the engine.
        /// </summary>
        /// <remarks>
        /// This is not the same as the returned properties for the engine
        /// which are set with 
        /// <see cref="AspectEngineBuilderBase{TBuilder, TEngine}.SetProperties"/>
        /// or 
        /// <see cref="AspectEngineBuilderBase{TBuilder, TEngine}.SetProperty"/>
        /// methods.
        /// </remarks>
        /// <param name="keyProperty">
        /// The property name (e.g. "TAC", "NativeModel").
        /// </param>
        /// <returns>This builder.</returns>
        public TBuilder SetKeyProperty(string keyProperty)
        { 
            _keyProperty = keyProperty;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the unique key for this engine's data in flow data.
        /// If not set, defaults to "profiles-{keyProperty}".
        /// </summary>
        /// <param name="elementDataKey">The element data key.</param>
        /// <returns>This builder.</returns>
        public TBuilder SetElementDataKey(string elementDataKey)
        {
            _elementDataKey = elementDataKey;
            return (TBuilder)this;
        }

        /// <inheritdoc/>
        public TBuilder SetPerformanceProfile(
            Pipeline.Engines.PerformanceProfiles profile)
        {
            _logger.LogWarning(Messages.PerformanceProfileNotSupported);
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the concurrency. Not supported for property keyed engines.
        /// </summary>
        /// <param name="concurrency">The concurrency value (ignored).</param>
        /// <returns>This builder.</returns>
        public TBuilder SetConcurrency(ushort concurrency)
        {
            _logger.LogWarning(Messages.ConcurrencyNotSupported);
            return (TBuilder)this;
        }
    }
}