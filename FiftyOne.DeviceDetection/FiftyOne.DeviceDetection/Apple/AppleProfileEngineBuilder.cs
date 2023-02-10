using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
