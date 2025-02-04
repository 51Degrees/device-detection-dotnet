/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// The builder for <see cref="AppleProfileEngine"/>
    /// </summary>
    public class AppleProfileEngineBuilder : FiftyOneOnPremiseAspectEngineBuilderBase<
        AppleProfileEngineBuilder, AppleProfileEngine>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<AppleProfileData> _dataLogger;

        /// <summary>
        /// The <see cref="AppleProfileEngine"/> does not use the 51Degrees distributor service.
        /// Therefore, this property is not required.
        /// </summary>
        protected override string DefaultDataDownloadType => "N/A";

        /// <summary>
        /// Construct a new instance of the builder.
        /// </summary>
        /// <param name="loggerFactory">
        /// Factory used to create loggers for the engine
        /// </param>
        public AppleProfileEngineBuilder(
            ILoggerFactory loggerFactory) :
            this(loggerFactory, null)
        {
        }

        public AppleProfileEngineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService) : 
            base(dataUpdateService)
        {
            _loggerFactory = loggerFactory;
            _dataLogger = _loggerFactory.CreateLogger<AppleProfileData>();
        }

        protected override AppleProfileEngine NewEngine(List<string> properties)
        {
            if (DataFiles.Count != 1)
            {
                throw new PipelineConfigurationException(
                    "This builder requires one and only one configured file " +
                    $"but it has {DataFileConfigs.Count}");
            }
            // Note that the data file will be added to the engine by the 'ConfigureEngine' method
            // in OnPremiseAspectEngineBuilderBase.

            return new AppleProfileEngine(
                _loggerFactory.CreateLogger<AppleProfileEngine>(),
                CreateAspectData,
                TempDir);
        }

        private IAppleProfileData CreateAspectData(IPipeline pipeline,
            FlowElementBase<IAppleProfileData, IFiftyOneAspectPropertyMetaData> engine)
        {
            return new AppleProfileData(
                _dataLogger,
                pipeline,
                engine as AppleProfileEngine,
                MissingPropertyService.Instance);
        }

        public override AppleProfileEngineBuilder SetPerformanceProfile(PerformanceProfiles profile)
        {
            throw new NotImplementedException();
        }
    }
}
