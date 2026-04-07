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

using FiftyOne.DeviceDetection.RobotsTxt.Data;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.RobotsTxt.FlowElements
{
    /// <summary>
    /// Builder for <see cref="RobotsTxtEngine"/>.
    /// </summary>
    public class RobotsTxtEngineBuilder : PropertyKeyedEngineBuilderBase<
        RobotsTxtEngineBuilder,
        RobotsTxtEngine>
    {
        public RobotsTxtEngineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService = null) :
            base(loggerFactory, dataUpdateService)
        { }

        public override RobotsTxtEngineBuilder SetPerformanceProfile(
            PerformanceProfiles profile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build the engine without a data file.
        /// PropertyKeyedDeviceEngine does not use a DataFile directly —
        /// it resolves its data from DeviceDetectionHashEngine via
        /// AddPipeline at runtime.
        /// </summary>
        /// <returns>The built engine.</returns>
        protected override RobotsTxtEngine Build()
        {
            return BuildEngine();
        }

        protected override RobotsTxtEngine CreateEngine(
            List<string> properties)
        {
            return new RobotsTxtEngine(properties, _loggerFactory);
        }
    }
}