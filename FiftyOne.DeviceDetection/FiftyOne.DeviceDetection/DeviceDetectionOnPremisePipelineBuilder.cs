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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Shared.FlowElements;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Configuration;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;

namespace FiftyOne.DeviceDetection
{
    /// <summary>
    /// Builder used to create pipelines with an on-premise 
    /// device detection engine.
    /// </summary>
    public class DeviceDetectionOnPremisePipelineBuilder :
        PrePackagedPipelineBuilderBase<DeviceDetectionOnPremisePipelineBuilder>
    {
        private string _filename;
        private bool _createTempDataCopy;
        private Stream _engineDataStream;

        private bool? _autoUpdateEnabled = null;
        private bool? _dataUpdateOnStartUpEnabled = null;
        private bool? _dataFileSystemWatcherEnabled = null;
        private int? _updatePollingInterval = null;
        private int? _updateRandomisationMax = null;
        private string _dataUpdateLicenseKey = null;
        private ushort? _concurrency = null;
        private int? _difference = null;
        private bool? _allowUnmatched = null;
        private bool? _usePerformanceGraph = null;
        private bool? _usePredictiveGraph = null;
        private PerformanceProfiles _performanceProfile = 
            PerformanceProfiles.Balanced;
        private DeviceDetectionAlgorithm _algorithm =
            DeviceDetectionAlgorithm.Hash;
        private bool _shareUsageEnabled = true;

        private IDataUpdateService _dataUpdateService;
        private HttpClient _httpClient;

        /// <summary>
        /// Internal constructor
        /// This builder should only be created through the 
        /// <see cref="DeviceDetectionPipelineBuilder"/> 
        /// </summary>
        /// <param name="loggerFactory">
        /// The <see cref="ILoggerFactory"/> to use when creating loggers.
        /// </param>
        /// <param name="dataUpdateService">
        /// The <see cref="IDataUpdateService"/> to use when registering 
        /// data files for automatic updates.
        /// </param>
        /// <param name="httpClient">
        /// The <see cref="HttpClient"/> to use for any web requests.
        /// </param>
        internal DeviceDetectionOnPremisePipelineBuilder(
            ILoggerFactory loggerFactory,
            IDataUpdateService dataUpdateService,
            HttpClient httpClient) : base(loggerFactory)
        {
            _dataUpdateService = dataUpdateService;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Set the filename of the device detection data file that the
        /// engine should use.
        /// </summary>
        /// <param name="filename">
        /// The data file
        /// </param>
        /// <param name="createTempDataCopy">
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        /// <exception cref="PipelineConfigurationException">
        /// Thrown if the filename has an unknown extension.
        /// </exception>
        [Obsolete("Call the overload that takes a license key instead. " +
            "This method will be removed in a future version")]
        internal DeviceDetectionOnPremisePipelineBuilder SetFilename(string filename, bool createTempDataCopy)
        {
            return SetFilename(filename, _dataUpdateLicenseKey, createTempDataCopy);
        }

        /// <summary>
        /// Set the filename of the device detection data file that the
        /// engine should use.
        /// </summary>
        /// <param name="filename">
        /// The data file
        /// </param>
        /// <param name="key">
        /// The license key to use when checking for updates to the
        /// data file.
        /// This parameter can be set to null, but doing so will disable 
        /// automatic updates. 
        /// </param>
        /// <param name="createTempDataCopy">
        /// True to create a temporary copy of the data file when 
        /// the engine is built.
        /// This is required in order for automatic updates
        /// to work correctly.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        /// <exception cref="PipelineConfigurationException">
        /// Thrown if the filename has an unknown extension.
        /// </exception>
        internal DeviceDetectionOnPremisePipelineBuilder SetFilename(
            string filename, 
            string key, 
            bool createTempDataCopy = true)
        {
            _filename = filename;
            _createTempDataCopy = createTempDataCopy;
            _dataUpdateLicenseKey = key;
            if (filename.EndsWith(".hash", StringComparison.OrdinalIgnoreCase))
            {
                _algorithm = DeviceDetectionAlgorithm.Hash;
            }
            else
            {
                throw new PipelineConfigurationException(
                    string.Format(CultureInfo.InvariantCulture,
                        Messages.ExceptionUnrecognizedFileExtension, 
                        filename));
            }
            return this;
        }


        /// <summary>
        /// Set the byte array to use as a data source when 
        /// creating the engine.
        /// </summary>
        /// <param name="dataStream">
        /// The entire device detection data file as a <see cref="Stream"/>.
        /// </param>
        /// <param name="algorithm">
        /// The detection algorithm that the supplied data supports.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        [Obsolete("Call the overload that takes a license key instead. " +
            "This method will be removed in a future version")]
        internal DeviceDetectionOnPremisePipelineBuilder SetEngineData(Stream dataStream, DeviceDetectionAlgorithm algorithm)
        {
            _engineDataStream = dataStream;
            _algorithm = algorithm;
            return this;
        }



        /// <summary>
        /// Set the byte array to use as a data source when 
        /// creating the engine.
        /// </summary>
        /// <param name="dataStream">
        /// The entire device detection data file as a <see cref="Stream"/>.
        /// </param>
        /// <param name="algorithm">
        /// The detection algorithm that the supplied data supports.
        /// </param>
        /// <param name="key">
        /// The license key to use when checking for updates to the
        /// data file.
        /// This parameter can be set to null, but doing so will disable 
        /// automatic updates. 
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        internal DeviceDetectionOnPremisePipelineBuilder SetEngineData(
            Stream dataStream,
            DeviceDetectionAlgorithm algorithm, 
            string key)
        {
            _engineDataStream = dataStream;
            _algorithm = algorithm;
            _dataUpdateLicenseKey = key;
            return this;
        }

        /// <summary>
        /// Set share usage enabled/disabled.
        /// Defaults to enabled.
        /// </summary>
        /// <param name="enabled">
        /// true to enable usage sharing. False to disable.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetShareUsage(bool enabled)
        {
            _shareUsageEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Enable/Disable auto update.
        /// Defaults to enabled.
        /// If enabled, the auto update system will automatically download
        /// and apply new data files for device detection.
        /// </summary>
        /// <param name="enabled">
        /// true to enable auto update. False to disable.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetAutoUpdate(bool enabled)
        {
            _autoUpdateEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Enable/Disable update on startup.
        /// Defaults to enabled.
        /// If enabled, the auto update system will be used to check for
        /// an update before the device detection engine is created.
        /// If an update is available, it will be downloaded and applied
        /// before the pipeline is built and returned for use so this may 
        /// take some time.
        /// </summary>
        /// <param name="enabled">
        /// True to enable update on startup. False to disable.
        /// </param>
        /// <returns>
        /// This builder.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetDataUpdateOnStartUp(bool enabled)
        {
            _dataUpdateOnStartUpEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Set the license key used when checking for new 
        /// device detection data files.
        /// Defaults to null.
        /// </summary>
        /// <param name="key">
        /// The license key
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetDataUpdateLicenseKey(string key)
        {
            _dataUpdateLicenseKey = key;
            return this;
        }

        /// <summary>
        /// Set the time between checks for a new data file made by the FiftyOne.Pipeline.Engines.Services.DataUpdateService.
        /// Default = 30 minutes.
        /// </summary>
        /// <param name="pollingIntervalSeconds">
        /// The number of seconds between checks.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetUpdatePollingInterval(int pollingIntervalSeconds)
        {
            _updatePollingInterval = pollingIntervalSeconds;
            return this;
        }

        /// <summary>
        /// Set the time between checks for a new data file made by the FiftyOne.Pipeline.Engines.Services.DataUpdateService.
        /// Default = 30 minutes.
        /// </summary>
        /// <param name="pollingInterval">
        /// The time between checks.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetUpdatePollingInterval(TimeSpan pollingInterval)
        {
            _updatePollingInterval = (int)pollingInterval.TotalSeconds;
            return this;
        }

        /// <summary>
        /// A random element can be added to the FiftyOne.Pipeline.Engines.Services.DataUpdateService
        /// polling interval. This option sets the maximum length of this random addition.
        /// Default = 10 minutes.
        /// </summary>
        /// <param name="maximumDeviationSeconds">
        /// The maximum time added to the data update polling interval.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetUpdateRandomisationMax(int maximumDeviationSeconds)
        {
            _updateRandomisationMax = maximumDeviationSeconds;
            return this;
        }

        /// <summary>
        /// A random element can be added to the FiftyOne.Pipeline.Engines.Services.DataUpdateService
        /// polling interval. This option sets the maximum length of this random addition.
        /// Default = 10 minutes.
        /// </summary>
        /// <param name="maximumDeviation">
        /// The maximum time added to the data update polling interval.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetUpdateRandomisationMax(TimeSpan maximumDeviation)
        {
            _updateRandomisationMax = (int)maximumDeviation.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Set the performance profile for the device detection engine.
        /// Defaults to balanced.
        /// </summary>
        /// <param name="profile">
        /// The performance profile to use.
        /// </param>
        /// <returns>
        /// This builder instance.
        /// </returns>
        public DeviceDetectionOnPremisePipelineBuilder SetPerformanceProfile(PerformanceProfiles profile)
        {
            _performanceProfile = profile;
            return this;
        }

        /// <summary>
        /// Set the expected number of concurrent operations using the engine.
        /// This sets the concurrency of the internal caches to avoid excessive
        /// locking.
        /// </summary>
        /// <param name="concurrency">Expected concurrent accesses</param>
        /// <returns>This builder</returns>
        public DeviceDetectionOnPremisePipelineBuilder SetConcurrency(
            ushort concurrency)
        {
            _concurrency = concurrency;
            return this;
        }

        /// <summary>
        /// Set the maximum difference to allow when processing HTTP headers.
        /// The difference is the difference in hash value between the 
        /// hash that was found, and the hash that is being searched for. 
        /// By default this is 0.
        /// </summary>
        /// <param name="difference">Difference to allow</param>
        /// <returns>This builder</returns>
        public DeviceDetectionOnPremisePipelineBuilder SetDifference(
            int difference)
        {
            _difference = difference;
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
        /// <param name="allow">
        /// True if results with no matched hash nodes should be considered
        /// valid
        /// </param>
        public DeviceDetectionOnPremisePipelineBuilder SetAllowUnmatched(
            bool allow)
        {
            _allowUnmatched = allow;
            return this;
        }

        /// <summary>
        /// Specify if the 'performance' evaluation graph should be used 
        /// or not.
        /// The performance graph is faster than predictive but can
        /// be less accurate.
        /// Note that the performance graph will always be evaluated first 
        /// if it is enableds so if you have both performance and predictive 
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
        public DeviceDetectionOnPremisePipelineBuilder SetUsePerformanceGraph(
            bool use)
        {
            _usePerformanceGraph = use;
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
        public DeviceDetectionOnPremisePipelineBuilder SetUsePredictiveGraph(
            bool use)
        {
            _usePredictiveGraph = use;
            return this;
        }

        /// <summary>
        /// The <see cref="DataUpdateService"/> has the ability to watch a 
        /// file on disk and refresh the engine as soon as that file is 
        /// updated.
        /// This setting enables/disables that feature.
        /// </summary>
        /// <param name="enabled">
        /// Pass true to enable the feature.
        /// </param>
        /// <returns></returns>
        public DeviceDetectionOnPremisePipelineBuilder SetDataFileSystemWatcher(bool enabled)
        {
            _dataFileSystemWatcherEnabled = enabled;
            return this;
        }

        /// <summary>
        /// Build and return a pipeline that can perform device detection.
        /// </summary>
        /// <returns></returns>
        public override IPipeline Build()
        {
            IAspectEngine deviceDetectionEngine = null;

            // Create the device detection engine based on the configuration.
            switch (_algorithm)
            {
                case DeviceDetectionAlgorithm.Hash:
                    var hashBuilder = new DeviceDetectionHashEngineBuilder(LoggerFactory, _dataUpdateService);
                    deviceDetectionEngine = ConfigureAndBuild(hashBuilder);
                    break;
                default:
                    throw new PipelineConfigurationException(
                        $"Unrecognized algorithm '{_algorithm}'.");
            }
            
            if (deviceDetectionEngine != null)
            {
                // Add the share usage element to the list if enabled
                if (_shareUsageEnabled)
                {
                    FlowElements.Add(new ShareUsageBuilder(LoggerFactory, _httpClient).Build());
                }
                // Add the device detection engine to the list
                FlowElements.Add(deviceDetectionEngine);
            }
            else
            {
                throw new PipelineException(Messages.ExceptionErrorOnStartup);
            }

            // Create and return the pipeline
            return base.Build();
        }

        /// <summary>
        /// Private method used to set configuration options common to 
        /// both hash and pattern engines and build the engine.
        /// </summary>
        /// <typeparam name="TBuilder">
        /// The type of the builder. Can be inferred from the builder parameter.
        /// </typeparam>
        /// <typeparam name="TEngine">
        /// The type of the engine. Can be inferred from the builder parameter.
        /// </typeparam>
        /// <param name="builder">
        /// The builder to configure.
        /// </param>
        /// <returns>
        /// A new device detection engine instance.
        /// </returns>
        private TEngine ConfigureAndBuild<TBuilder, TEngine>(OnPremiseDeviceDetectionEngineBuilderBase<TBuilder, TEngine> builder)
            where TBuilder : OnPremiseDeviceDetectionEngineBuilderBase<TBuilder, TEngine>
            where TEngine : IFiftyOneAspectEngine
        {
            // Configure caching
            if (ResultsCache)
            {
                builder.SetCache(new CacheConfiguration() { Size = ResultsCacheSize });
            }
            // Configure lazy loading
            if (LazyLoading)
            {
                builder.SetLazyLoading(new LazyLoadingConfiguration(
                    (int)LazyLoadingTimeout.TotalMilliseconds,
                    LazyLoadingCancellationToken));
            }

            // Configure auto update.
            if (_autoUpdateEnabled.HasValue)
            {
                builder.SetAutoUpdate(_autoUpdateEnabled.Value);
            }
            // Configure data update on startup.
            if (_dataUpdateOnStartUpEnabled.HasValue)
            {
                builder.SetDataUpdateOnStartup(_dataUpdateOnStartUpEnabled.Value);
            }
            // Configure file system watcher.
            if (_dataFileSystemWatcherEnabled.HasValue)
            {
                builder.SetDataFileSystemWatcher(_dataFileSystemWatcherEnabled.Value);
            }
            // Configure update poilling interval.
            if (_updatePollingInterval.HasValue)
            {
                builder.SetUpdatePollingInterval(_updatePollingInterval.Value);
            }
            // Configure update polling interval randomisation.
            if (_updateRandomisationMax.HasValue)
            {
                builder.SetUpdateRandomisationMax(_updateRandomisationMax.Value);
            }

            builder.SetDataUpdateLicenseKey(_dataUpdateLicenseKey);

            // Configure performance profile
            builder.SetPerformanceProfile(_performanceProfile);
            // Configure concurrency
            if (_concurrency.HasValue)
            {
                builder.SetConcurrency(_concurrency.Value);
            }
            // Configure difference
            if (_difference.HasValue)
            {
                builder.SetDifference(_difference.Value);
            }
            // Configure unmatched
            if (_allowUnmatched.HasValue)
            {
                builder.SetAllowUnmatched(_allowUnmatched.Value);
            }
            // Configure performance graph
            if (_usePerformanceGraph.HasValue)
            {
                builder.SetUsePerformanceGraph(_usePerformanceGraph.Value);
            }
            // Configure predictive graph
            if (_usePredictiveGraph.HasValue)
            {
                builder.SetUsePredictiveGraph(_usePredictiveGraph.Value);
            }

            // Build the engine
            TEngine engine = default(TEngine);

            if (string.IsNullOrEmpty(_filename) == false)
            {
                engine = builder.Build(_filename, _createTempDataCopy);
            }
            else if (_engineDataStream != null ||
                _dataUpdateOnStartUpEnabled == true)
            {
                engine = builder.Build(_engineDataStream);
            }
            else
            {
                throw new PipelineConfigurationException(
                    Messages.ExceptionNoEngineData);
            }

            return engine;
        }

    }
}
