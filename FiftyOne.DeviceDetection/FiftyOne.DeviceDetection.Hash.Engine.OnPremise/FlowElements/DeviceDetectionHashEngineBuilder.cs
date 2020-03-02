/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL) 
 * v.1.2 and is subject to its terms as set out below.
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
using FiftyOne.DeviceDetection.Shared.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements
{
    /// <summary>
    /// Builder for the <see cref="DeviceDetectionHashEngine"/>. All options
    /// for the engine should be set here.
    /// </summary>
    public class DeviceDetectionHashEngineBuilder
       : OnPremiseDeviceDetectionEngineBuilderBase<DeviceDetectionHashEngineBuilder, DeviceDetectionHashEngine>
    {
        #region Private Properties

        private readonly ILoggerFactory _loggerFactory;

        private IConfigSwigWrapper _config = null;

        #endregion

        internal ISwigFactory SwigFactory { get; set; } = new SwigFactory();

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

        #region Constructor

        /// <summary>
        /// Construct a new instance of the builder.
        /// </summary>
        /// <param name="loggerFactory">
        /// Factory used to create loggers for the engine
        /// </param>
        public DeviceDetectionHashEngineBuilder(
            ILoggerFactory loggerFactory)
            : this(loggerFactory, null)
        {
        }

        /// <summary>
        /// Construct a new instance of the builder.
        /// </summary>
        /// <param name="loggerFactory">
        /// Factory used to create loggers for the engine
        /// </param>
        /// <param name="dataUpdateService">
        /// Data update service used to keep the engine's data up to date.
        /// </param>
        public DeviceDetectionHashEngineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService)
            : base(dataUpdateService)
        {
            _loggerFactory = loggerFactory;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the maximum difference in hash value to allow when processing
        /// HTTP headers.
        /// </summary>
        /// <param name="difference">Difference to allow</param>
        /// <returns>This builder</returns>
        public override DeviceDetectionHashEngineBuilder SetDifference(int difference)
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
        public DeviceDetectionHashEngineBuilder SetDrift(int drift)
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
        public override DeviceDetectionHashEngineBuilder SetAllowUnmatched(bool allow)
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
        public DeviceDetectionHashEngineBuilder SetReuseTempFile(bool reuse)
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
        public DeviceDetectionHashEngineBuilder SetUpdateMatchedUserAgent(
            bool update)
        {
            SwigConfig.setUpdateMatchedUserAgent(update);
            return this;
        }

        public DeviceDetectionHashEngineBuilder SetUsePerformanceGraph(
            bool use)
        {
            SwigConfig.setUsePerformanceGraph(use);
            return this;
        }

        public DeviceDetectionHashEngineBuilder SetUsePredictiveGraph(
            bool use)
        {
            SwigConfig.setUsePredictiveGraph(use);
            return this;
        }

        /// <summary>
        /// Set the performance profile to use when constructing the data set.
        /// </summary>
        /// <param name="profileName">Name of the profile to use</param>
        /// <returns>This builder</returns>
        public DeviceDetectionHashEngineBuilder SetPerformanceProfile(
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
        public override DeviceDetectionHashEngineBuilder SetPerformanceProfile(
            PerformanceProfiles profile)
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
                        "profile");
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
        public override DeviceDetectionHashEngineBuilder SetConcurrency(
            ushort concurrency)
        {
            SwigConfig.setConcurrency(concurrency);
            return this;
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
        protected override DeviceDetectionHashEngine NewEngine(
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
                var tempDirs = new VectorStringSwig();
                tempDirs.Add(TempDir);
                SwigConfig.setTempDirectories(tempDirs);
                SwigConfig.setUseTempFile(true);
            }

            return new DeviceDetectionHashEngine(
                _loggerFactory,
                dataFile,
                SwigConfig,
                SwigFactory.CreateRequiredProperties(new VectorStringSwig(properties)),
                CreateAspectData,
                TempDir,
                SwigFactory);
        }
        
        protected override string DefaultDataDownloadType => "HashTrieV34";
        #endregion


        private IDeviceDataHash CreateAspectData(IPipeline pipeline, 
            FlowElementBase<IDeviceDataHash, IFiftyOneAspectPropertyMetaData> engine)
        {
            return new DeviceDataHash(
                _loggerFactory.CreateLogger<DeviceDataHash>(),
                pipeline,
                engine as DeviceDetectionHashEngine,
                MissingPropertyService.Instance);
        }
    }
}
