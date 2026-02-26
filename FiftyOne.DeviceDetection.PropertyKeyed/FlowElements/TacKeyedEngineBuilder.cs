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
    /// Builder for <see cref="TacKeyedEngine"/>.
    /// </summary>
    public class TacKeyedEngineBuilder :
        PropertyKeyedEngineBuilderBase<TacKeyedEngineBuilder, TacKeyedEngine>
    {
        private IEngineProvider _engineProvider;

        /// <summary>
        /// Constructs a new instance of <see cref="TacKeyedEngineBuilder"/>.
        /// </summary>
        /// <param name="loggerFactory">The factory used to create loggers.</param>
        /// <param name="dataUpdateService">The data update service, if any.</param>
        public TacKeyedEngineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService = null)
            : base(loggerFactory, dataUpdateService)
        {
        }

        /// <summary>
        /// Sets the engine provider for creating the inner device detection engine.
        /// </summary>
        public TacKeyedEngineBuilder SetEngineProvider(IEngineProvider engineProvider)
        {
            _engineProvider = engineProvider;
            return this;
        }

        /// <inheritdoc/>
        public override TacKeyedEngineBuilder SetPerformanceProfile(
            FiftyOne.Pipeline.Engines.PerformanceProfiles profile)
        {
            if (_loggerFactory != null)
            {
                _loggerFactory.CreateLogger<TacKeyedEngineBuilder>()
                    .LogWarning("SetPerformanceProfile is obsolete for PropertyKeyedDeviceEngine. Use SetEngineProvider instead.");
            }
            return this;
        }

        /// <summary>
        /// Sets the concurrency.
        /// </summary>
        public TacKeyedEngineBuilder SetConcurrency(ushort concurrency)
        {
            if (_loggerFactory != null)
            {
                _loggerFactory.CreateLogger<TacKeyedEngineBuilder>()
                    .LogWarning("SetConcurrency is obsolete for PropertyKeyedDeviceEngine. Use SetEngineProvider instead.");
            }
            return this;
        }

        /// <inheritdoc/>
        protected override TacKeyedEngine CreateEngine(
            List<string> properties)
        {
            return new TacKeyedEngine(
                _loggerFactory,
                properties,
                _engineProvider);
        }
    }
}
