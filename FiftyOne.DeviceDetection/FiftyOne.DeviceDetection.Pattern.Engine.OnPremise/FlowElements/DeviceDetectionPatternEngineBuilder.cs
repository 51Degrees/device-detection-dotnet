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

using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Data;
using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Interop;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Wrappers;
using FiftyOne.DeviceDetection.Shared.FlowElements;

namespace FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.FlowElements
{
    /// <summary>
    /// Builder for the <see cref="DeviceDetectionPatternEngine"/>. All options
    /// for the engine should be set here.
    /// </summary>
    public class DeviceDetectionPatternEngineBuilder
        : OnPremiseDeviceDetectionEngineBuilderBase<DeviceDetectionPatternEngineBuilder, DeviceDetectionPatternEngine>
    {
        #region Private Properties

        private readonly ILoggerFactory _loggerFactory;

        private IConfigSwigWrapper _config;

        #endregion

        internal ISwigFactory SwigFactory { get; set; } = new SwigFactory();

        private IConfigSwigWrapper SwigConfig
        {
            get
            {
                if (_config == null)
                {
                    _config = SwigFactory.CreateConfig();
                    _config.setConcurrency((ushort)Environment.ProcessorCount);
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
        public DeviceDetectionPatternEngineBuilder(
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
        public DeviceDetectionPatternEngineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService)
            : base(dataUpdateService)
        {
            _loggerFactory = loggerFactory;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set whether or not an existing temp file should be used if one is
        /// found in the temp directory.
        /// </summary>
        /// <param name="reuse">True if an existing file should be used</param>
        /// <returns>This builder</returns>
        public DeviceDetectionPatternEngineBuilder SetReuseTempFile(bool reuse)
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
        public DeviceDetectionPatternEngineBuilder SetUpdateMatchedUserAgent(
            bool update)
        {
            SwigConfig.setUpdateMatchedUserAgent(update);
            return this;
        }

        /// <summary>
        /// Set the performance profile to use when constructing the data set.
        /// </summary>
        /// <param name="profileName">Name of the profile to use</param>
        /// <returns>This builder</returns>
        public DeviceDetectionPatternEngineBuilder SetPerformanceProfile(
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
        public override DeviceDetectionPatternEngineBuilder SetPerformanceProfile(
            PerformanceProfiles profile)
        {
            switch (profile)
            {
                case PerformanceProfiles.LowMemory:
                    SwigConfig.setLowMemory();
                    break;
                case PerformanceProfiles.MaxPerformance:
                    SwigConfig.setMaxPerformance();
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
                default:
                    throw new ArgumentException(
                        $"The performance profile '{profile}' is not valid " +
                        $"for a DeviceDetectionPatternEngine.",
                        "profile");
            }
            return this;
        }

        /// <summary>
        /// Set the size of the User-Agent cache to use within the Pattern
        /// engine. For a low concurrency environment, a larger cache will
        /// improve performance. However, in a high concurrency environment,
        /// no cache may be preferable.
        /// </summary>
        /// <param name="capacity">The size of the cache to use</param>
        /// <returns>This builder</returns>
        public DeviceDetectionPatternEngineBuilder SetUserAgentCache(uint capacity)
        {
            SwigConfig.setUserAgentCacheCapacity(capacity);
            return this;
        }

        /// <summary>
        /// Set the expected number of concurrent operations using the engine.
        /// This sets the concurrency of the internal caches to avoid excessive
        /// locking.
        /// </summary>
        /// <param name="concurrency">Expected concurrent accesses</param>
        /// <returns>This builder</returns>
        public override DeviceDetectionPatternEngineBuilder SetConcurrency(ushort concurrency)
        {
            SwigConfig.setConcurrency(concurrency);
            return this;
        }

        /// <summary>
        /// Set number of signatures that should be looked at when finding the
        /// closest matching signature.
        /// </summary>
        /// <param name="closestSignatures">Max compared signatures</param>
        /// <returns></returns>
        public DeviceDetectionPatternEngineBuilder SetClosestSignatures(
            int closestSignatures)
        {
            SwigConfig.setClosestSignatures(closestSignatures);
            return this;
        }

        /// <summary>
        /// Set the maximum difference to allow when processing HTTP headers.
        /// The difference is a combination of the difference in character
        /// position of matched substrings, and the difference in ASCII value
        /// of each character of matched substrings. By default this is 10.
        /// </summary>
        /// <param name="difference">Difference threshold</param>
        /// <returns>This builder</returns>
        public override DeviceDetectionPatternEngineBuilder SetDifference(
            int difference)
        {
            _config.setDifference(difference);
            return this;
        }

        /// <summary>
        /// If set to false, a non-matching User-Agent will result in
        /// properties without set values. If set to true, a non-matching
        /// User-Agent will cause the 'default profiles' to be returned. This
        /// means that properties will always have values (i.e. no need to
        /// check .HasValue) but some may be inaccurate. By default, this is
        /// false.
        /// </summary>
        /// <param name="allow"/>
        /// True if results with no matched substrings should be considered
        /// valid
        /// </param>
        /// <returns>This builder</returns>
        public override DeviceDetectionPatternEngineBuilder SetAllowUnmatched(bool allow)
        {
            _config.setAllowUnmatched(allow);
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
        protected override DeviceDetectionPatternEngine NewEngine(
            List<string> properties)
        {
            if (DataFiles.Count != 1)
            {
                throw new PipelineConfigurationException(
                    "This builder requires one and only one configured file " +
                    $"but it has {DataFiles.Count}");
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

            var engine = new DeviceDetectionPatternEngine(
                _loggerFactory,
                dataFile,
                SwigConfig,
                SwigFactory.CreateRequiredProperties(new VectorStringSwig(properties)),
                CreateAspectData,
                TempDir,
                SwigFactory);
            dataFile.Engine = engine;
            return engine;
        }
        
        protected override string DefaultDataDownloadType => "BinaryV32";
        #endregion

        private IDeviceDataPattern CreateAspectData(IPipeline pipeline, 
            FlowElementBase<IDeviceDataPattern, IFiftyOneAspectPropertyMetaData> engine)
        {
            return new DeviceDataPattern(
                _loggerFactory.CreateLogger<DeviceDataPattern>(),
                pipeline,
                engine as DeviceDetectionPatternEngine,
                MissingPropertyService.Instance);
        }

    }
}
