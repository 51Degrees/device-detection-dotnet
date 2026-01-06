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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers;
using FiftyOne.DeviceDetection.Shared.FlowElements;
using FiftyOne.Pipeline.Core.Attributes;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Configuration;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements
{
    /// <summary>
    /// Builder for the <see cref="DeviceDetectionHashEngine"/>. All options
    /// for the engine should be set here.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/pipeline-elements/device-detection-on-premise.md">Specification</see>
    /// </summary>
    public abstract class DeviceDetectionHashEngineBuilderBase<TEngine>
       : OnPremiseDeviceDetectionEngineBuilderBase<
           DeviceDetectionHashEngineBuilderBase<TEngine>, 
           TEngine>
        where TEngine : DeviceDetectionHashEngine
    {
        #region Constants
        private const string NATIVE_DEFAULTS = "The default value for this property comes " +
            "from the native C/C++ code. You can find these defaults in the following files: " +
            "https://github.com/51Degrees/common-cxx/blob/master/config.h, " +
            "https://github.com/51Degrees/device-detection-cxx/blob/master/src/config-dd.h";
        #endregion

        #region Private Properties

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<DeviceDataHash> _dataLogger;

        private IConfigSwigWrapper _config = null;

        #endregion

        #region Public Properties
        /// <summary>
        /// Factory used to create a new <see cref="SwigConfig"/>. 
        /// </summary>
        /// <remarks>
        /// Must be set after construction and before usage.
        /// </remarks>
        internal ISwigFactory SwigFactory { get; set; } = new SwigFactory();

        /// <summary>
        /// Wrapper used to build a Configuration file for the engine.
        /// </summary>
        private IConfigSwigWrapper SwigConfig
        {
            get
            {
                if (_config == null)
                {
                    _config = SwigFactory.CreateConfig();
                }
                return _config;
            }
        }
        
        #endregion

        #region Constructor

        /// <summary>
        /// Construct a new instance of 
        /// <see cref="DeviceDetectionHashEngineBuilderBase{TEngine}"/>
        /// </summary>
        /// <param name="loggerFactory">
        /// Factory used to create loggers for the engine
        /// </param>
        public DeviceDetectionHashEngineBuilderBase(
            ILoggerFactory loggerFactory)
            : this(loggerFactory, null)
        {
        }

        /// <summary>
        /// Construct a new instance of 
        /// <see cref="DeviceDetectionHashEngineBuilderBase{TEngine}"/>
        /// </summary>
        /// <param name="loggerFactory">
        /// Factory used to create loggers for the engine
        /// </param>
        /// <param name="dataUpdateService">
        /// Data update service used to keep the engine's data up to date.
        /// </param>
        public DeviceDetectionHashEngineBuilderBase(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService)
            : base(dataUpdateService)
        {
            _loggerFactory = loggerFactory;
            _dataLogger = _loggerFactory.CreateLogger<DeviceDataHash>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the maximum difference in hash value to allow when processing
        /// HTTP headers.
        /// </summary>
        /// <param name="difference">Difference to allow</param>
        /// <returns>This builder</returns>
        [DefaultValue("0 - " + NATIVE_DEFAULTS)]
        public override DeviceDetectionHashEngineBuilderBase<TEngine> 
            SetDifference(int difference)
        {
            SwigConfig.setDifference(difference);
            return this;
        }

        /// <summary>
        /// Set maximum drift in hash position to allow when processing HTTP
        /// headers.
        /// </summary>
        /// <param name="drift">Drift to allow</param>
        /// <returns>This builder</returns>
        [DefaultValue("0 - " + NATIVE_DEFAULTS)]
        public DeviceDetectionHashEngineBuilderBase<TEngine> 
            SetDrift(int drift)
        {
            SwigConfig.setDrift(drift);
            return this;
        }

        /// <summary>
        /// If set to false, a non-matching User-Agent will result in
        /// properties without set values. If set to true, a non-matching
        /// User-Agent will cause the 'default profiles' to be returned. This
        /// means that properties will always have values (i.e. no need to
        /// check .HasValue) but some may be inaccurate.. By default, this is
        /// false.
        /// </summary>
        /// <param name="allow">
        /// True if results with no matched hash nodes should be considered
        /// valid
        /// </param>
        /// <returns>This builder</returns>
        [DefaultValue("false - " + NATIVE_DEFAULTS)]
        public override DeviceDetectionHashEngineBuilderBase<TEngine> 
            SetAllowUnmatched(bool allow)
        {
            SwigConfig.setAllowUnmatched(allow);
            return this;
        }

        /// <summary>
        /// Set whether or not an existing temp file should be used if one is
        /// found in the temp directory.
        /// </summary>
        /// <param name="reuse">True if an existing file should be used</param>
        /// <returns>This builder</returns>
        [DefaultValue("false - " + NATIVE_DEFAULTS)]
        public DeviceDetectionHashEngineBuilderBase<TEngine> 
            SetReuseTempFile(bool reuse)
        {
            SwigConfig.setReuseTempFile(reuse);
            return this;
        }

        /// <summary>
        /// Set whether or not the matched characters of the User-Agent should
        /// be stored to be returned in the results.
        /// </summary>
        /// <param name="update">
        /// True if the matched User-Agent should be stored
        /// </param>
        /// <returns>This builder</returns>
        [DefaultValue("true - " + NATIVE_DEFAULTS)]
        public DeviceDetectionHashEngineBuilderBase<TEngine> 
            SetUpdateMatchedUserAgent(bool update)
        {
            SwigConfig.setUpdateMatchedUserAgent(update);
            return this;
        }

        /// <summary>
        /// Specify if the 'performance' evaluation graph should be used 
        /// or not.
        /// The performance graph is faster than predictive but can
        /// be less accurate.
        /// Note that the performance graph will always be evaluated first 
        /// if it is enabled so if you have both performance and predictive 
        /// enabled, you will often be getting results from just the 
        /// performance graph.
        /// In that situation, predictive will only be used if a match cannot
        /// be found using the performance graph.
        /// </summary>
        /// <param name="use">
        /// True to use the performance graph, false to ignore it.
        /// </param>
        /// <returns>
        /// This builder.
        /// </returns>
        [DefaultValue("false - " + NATIVE_DEFAULTS)]
        public override DeviceDetectionHashEngineBuilderBase<TEngine> 
            SetUsePerformanceGraph(bool use)
        {
            SwigConfig.setUsePerformanceGraph(use);
            return this;
        }

        /// <summary>
        /// Specify if the 'predictive' evaluation graph should be used 
        /// or not.
        /// The predictive graph is more accurate than performance
        /// but is also slower.
        /// Note that the performance graph will always be evaluated first 
        /// if it is enabled, so if you have both performance and predictive 
        /// enabled, you will often be getting results from just the 
        /// performance graph.
        /// In that situation, predictive will only be used if a match cannot
        /// be found using the performance graph.
        /// </summary>
        /// <param name="use">
        /// True to use the performance graph, false to ignore it.
        /// </param>
        /// <returns>
        /// This builder.
        /// </returns>
        [DefaultValue("true - " + NATIVE_DEFAULTS)]
        public override DeviceDetectionHashEngineBuilderBase<TEngine> 
            SetUsePredictiveGraph(bool use)
        {
            SwigConfig.setUsePredictiveGraph(use);
            return this;
        }

        /// <summary>
        /// Set the performance profile to use when constructing the data set.
        /// </summary>
        /// <param name="profileName">Name of the profile to use</param>
        /// <returns>This builder</returns>
        [DefaultValue("Balanced - Performance profiles are defined in the native C code. " +
            "See https://github.com/51Degrees/device-detection-cxx/blob/master/src/hash/hash.c#L175")]
        public DeviceDetectionHashEngineBuilderBase<TEngine> SetPerformanceProfile(
            string profileName)
        {
            PerformanceProfiles profile;
            if (Enum.TryParse<PerformanceProfiles>(
                profileName,
                out profile))
            {
                return SetPerformanceProfile(profile);
            }
            else
            {
                var available = Enum.GetNames(typeof(PerformanceProfiles))
                    .Select(i => "'" + i + "'");
                throw new ArgumentException(
                    $"'{profileName}' is not a valid performance profile. " +
                    $"Available profiles are {string.Join(", ", available)}.");
            }
        }

        /// <summary>
        /// Set the performance profile to use when constructing the data set.
        /// </summary>
        /// <param name="profile">Profile to use</param>
        /// <returns>This builder</returns>
        [CodeConfigOnly]
        public override DeviceDetectionHashEngineBuilderBase<TEngine> 
            SetPerformanceProfile(PerformanceProfiles profile)
        {
            switch (profile)
            {
                case PerformanceProfiles.LowMemory:
                    SwigConfig.setLowMemory();
                    break;
                case PerformanceProfiles.Balanced:
                    SwigConfig.setBalanced();
                    break;
                case PerformanceProfiles.BalancedTemp:
                    SwigConfig.setBalancedTemp();
                    break;
                case PerformanceProfiles.HighPerformance:
                    SwigConfig.setHighPerformance();
                    break;
                case PerformanceProfiles.MaxPerformance:
                    SwigConfig.setMaxPerformance();
                    break;
                default:
                    throw new ArgumentException(
                        $"The performance profile '{profile}' is not valid " +
                        $"for a DeviceDetectionHashEngine.",
                        nameof(profile));
            }
            return this;
        }

        /// <summary>
        /// Set the expected number of concurrent operations using the engine.
        /// This sets the concurrency of the internal caches to avoid excessive
        /// locking.
        /// </summary>
        /// <param name="concurrency">Expected concurrent accesses</param>
        /// <returns>This builder</returns>
        [DefaultValue("Environment.ProcessorCount")]
        public override DeviceDetectionHashEngineBuilderBase<TEngine> SetConcurrency(
            ushort concurrency)
        {            
            SwigConfig.setConcurrency(concurrency);
            return this;
        }

        /// <summary>
        /// Override the SetCache method so it cannot be called successfully
        /// for the Device-detection Hash Engine.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        [CodeConfigOnly]
        public override DeviceDetectionHashEngineBuilderBase<TEngine> SetCache(CacheConfiguration cacheConfig)
        {
            throw new NotSupportedException(Messages.ExceptionSetCache);
        }

        /// <summary>
        /// Override the SetCacheSize method so it cannot be called successfully
        /// for the Device-detection Hash Engine.
        /// </summary>
        /// <param name="cacheSize"></param>
        /// <exception cref="NotSupportedException"></exception>
        [DefaultValue("Not supported")]
        public override DeviceDetectionHashEngineBuilderBase<TEngine> SetCacheSize(int cacheSize)
        {
            throw new NotSupportedException(Messages.ExceptionSetCache);
        }

        #endregion

        #region Protected Overrides
        /// <summary>
        /// Called by the 'BuildEngine' method to handle
        /// creation of the engine instance.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>
        /// An <see cref="IAspectEngine"/>.
        /// </returns>
        protected override TEngine NewEngine(
            List<string> properties)
        {
            if (DataFiles.Count != 1)
            {
                throw new PipelineConfigurationException(
                    "This builder requires one and only one configured file " +
                    $"but it has {DataFileConfigs.Count}");
            }
            var dataFile = DataFiles.First();
            // We remove the data file configuration from the list.
            // This is because the on-premise engine builder base class 
            // adds all the data file configs after engine creation.  
            // However, the device detection data files are supplied 
            // directly to the constructor.
            // Consequently, we remove it here to stop it from being added 
            // again by the base class.
            DataFiles.Remove(dataFile);

            // Update the swig configuration object.
            SwigConfig.setUseUpperPrefixHeaders(false);
            if (dataFile.Configuration.CreateTempCopy && String.IsNullOrEmpty(TempDir) == false)
            {
                using (var tempDirs = new VectorStringSwig())
                {
                    tempDirs.Add(TempDir);
                    SwigConfig.setTempDirectories(tempDirs);
                }
                SwigConfig.setUseTempFile(true);
            }

            // Create swig property configuration object.
            IRequiredPropertiesConfigSwigWrapper propertyConfig = null;
            using (var vProperties = new VectorStringSwig(properties)) {
                propertyConfig = SwigFactory.CreateRequiredProperties(vProperties);
            }

            // Create a new instance of the required engine type.
            var engine = CreateEngine(
                _loggerFactory,        
                CreateAspectData,
                TempDir);

            // Then set the instances of properties that remain internal to
            // the package.
            engine.SwigFactory = SwigFactory;
            engine.PropertiesConfigSwig = propertyConfig;
            engine.Config = SwigConfig;

            // This must be last as the call results in RefreshData being
            // triggered which requires the prior three properties to be set.
            engine.AddDataFile(dataFile);

            return engine;
        }
        
        /// <summary>
        /// Get the default value for the 'Type' parameter that is passed
        /// to the 51Degrees Distributor service when checking for updated
        /// data files.
        /// </summary>
        protected override string DefaultDataDownloadType => "HashV41";
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Overridden in the implementing class to create a new instance of
        /// TEngine with the constructor parameters provided.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="deviceDataFactory"></param>
        /// <param name="tempDataFilePath"></param>
        /// <returns></returns>
        protected abstract TEngine CreateEngine(
            ILoggerFactory loggerFactory,
            Func<IPipeline, FlowElementBase<
                IDeviceDataHash,
                IFiftyOneAspectPropertyMetaData>,
                IDeviceDataHash> deviceDataFactory,
            string tempDataFilePath);
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="DeviceDataHash"/>. 
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="engine"></param>
        /// <returns></returns>
        private IDeviceDataHash CreateAspectData(IPipeline pipeline, 
            FlowElementBase<IDeviceDataHash, IFiftyOneAspectPropertyMetaData> engine)
        {
            return new DeviceDataHash(
                _dataLogger,
                pipeline,
                engine as DeviceDetectionHashEngine,
                MissingPropertyService.Instance);
        }
    }
}
