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
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.DeviceDetection.Shared.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements
{
    /// <summary>
    /// Hash device detection engine. This engine takes User-Agents and
    /// other relevant HTTP headers and returns properties about the device
    /// which produced them e.g. DeviceType or ReleaseDate.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/pipeline-elements/device-detection-on-premise.md">Specification</see>
    /// </summary>
    public class DeviceDetectionHashEngine : 
        OnPremiseDeviceDetectionEngineBase<IDeviceDataHash>        
    {
        /// <summary>
        /// The underlying native engine used by this .net instance. Set by
        /// <see cref="SwigFactory"/> when <see cref="RefreshData(string)"/>
        /// or <see cref="RefreshData(string, Stream)"/> is called.
        /// </summary>
        private IEngineSwigWrapper _engine;

        /// <summary>
        /// Evidence key filter for the engine. Set in 
        /// <see cref="InitEngineMetaData"/> after refresh.
        /// </summary>
        private IEvidenceKeyFilter _evidenceKeyFilter;

        /// <summary>
        /// Properties for the engine. Set in <see cref="InitEngineMetaData"/> 
        /// after refresh.
        /// </summary>
        private IList<IFiftyOneAspectPropertyMetaData> _properties;

        /// <summary>
        /// Components for the engine. Set in <see cref="InitEngineMetaData"/> 
        /// after refresh.
        /// </summary>
        private IList<IComponentMetaData> _components;

        /// <summary>
        /// Factory used to create a new <see cref="IEngineSwigWrapper"/> when
        /// <see cref="RefreshData(string)"/> or 
        /// <see cref="RefreshData(string, Stream)"/> is called.
        /// </summary>
        /// <remarks>
        /// Must be set after construction and before usage.
        /// </remarks>
        internal ISwigFactory SwigFactory;

        /// <summary>
        /// Wrapper to pass general configuration from managed code to unmanaged 
        /// code.
        /// </summary>
        /// <remarks>
        /// Must be set after construction and before usage.
        /// </remarks>
        internal IConfigSwigWrapper Config;

        /// <summary>
        /// Wrapper to pass property configuration from managed code to 
        /// unmanaged code.
        /// </summary>
        internal IRequiredPropertiesConfigSwigWrapper PropertiesConfigSwig;

        /// <summary>
        /// Used to randomize the next update date time.
        /// </summary>
        private static Random _rng = new Random();

        /// <summary>
        /// The component used for metric properties. 
        /// </summary>
        private ComponentMetaDataDefault _deviceMetricsComponent = 
            new ComponentMetaDataHash("Metrics");

        /// <summary>
        /// This event is fired whenever the data that this engine makes use
        /// of has been updated.
        /// </summary>
        public override event EventHandler<EventArgs> RefreshCompleted;

        /// <summary>
        /// Construct a new instance of <see cref="DeviceDetectionHashEngine"/>
        /// from <see cref="DeviceDetectionHashEngineBuilderBase{T}"/> builder.
        /// </summary>
        /// <remarks>
        /// The constructor is intended to only be called from 
        /// <see cref="DeviceDetectionHashEngineBuilderBase{T}.CreateEngine(ILoggerFactory, Func{IPipeline, FlowElementBase{IDeviceDataHash, IFiftyOneAspectPropertyMetaData}, IDeviceDataHash}, string)"/>
        /// and should not be called directly.
        /// </remarks>
        /// <param name="loggerFactory">Logger to use</param>
        /// <param name="deviceDataFactory">
        /// Method used to get an aspect data instance.
        /// </param>
        /// <param name="tempDataFilePath">
        /// The directory to use when storing temporary copies of the 
        /// data file(s) used by this engine.
        /// </param>
        internal protected DeviceDetectionHashEngine(
            ILoggerFactory loggerFactory,
            Func<IPipeline, FlowElementBase<
                IDeviceDataHash, 
                IFiftyOneAspectPropertyMetaData>, 
                IDeviceDataHash> deviceDataFactory,
            string tempDataFilePath)
            : base(
                  loggerFactory.CreateLogger<DeviceDetectionHashEngine>(),
                  deviceDataFactory,
                  tempDataFilePath)
        {
        }

        /// <summary>
        /// The key to use for this element's data in a 
        /// <see cref="IFlowData"/> instance.
        /// </summary>
        public override string ElementDataKey => "device";

        /// <summary>
        /// Wrapper to pass metadata from managed code to 
        /// unmanaged code.
        /// </summary>
        internal IMetaDataSwigWrapper MetaData => _engine.getMetaData();

        /// <summary>
        /// Get the meta-data for properties populated by this engine.
        /// </summary>
        public override IList<IFiftyOneAspectPropertyMetaData> Properties
        {
            get
            {
                return _properties;
            }
        }

        /// <summary>
        /// Get the meta-data for profiles that may be returned by this
        /// engine.
        /// </summary>
        public override IEnumerable<IProfileMetaData> Profiles
        {
            get
            {
                using (var profiles = _engine.getMetaData().getProfiles(this))
                {
                    foreach (var profile in profiles)
                    {
                        yield return profile;
                    }
                }
            }
        }

        /// <summary>
        /// Get the meta-data for components populated by this engine.
        /// </summary>
        public override IEnumerable<IComponentMetaData> Components
        {
            get
            {
                return _components;
            }
        }

        /// <summary>
        /// Get the meta-data for values that can be returned by this engine.
        /// </summary>
        public override IEnumerable<IValueMetaData> Values
        {
            get
            {
                using (var values = _engine.getMetaData().getValues(this))
                {
                    foreach (var value in values)
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// The tier of the data that is currently being used by this engine.
        /// For example, 'Lite' or 'Enterprise'
        /// </summary>
        public override string DataSourceTier => _engine.getProduct();

        /// <summary>
        /// True if the data used by this engine will automatically be
        /// updated when a new file is available.
        /// False if the data will only be updated manually.
        /// </summary>
        public bool AutomaticUpdatesEnabled => _engine.getAutomaticUpdatesEnabled();

        /// <summary>
        /// A filter that defines the evidence that this engine can 
        /// make use of.
        /// </summary>
        public override IEvidenceKeyFilter EvidenceKeyFilter => _evidenceKeyFilter;

        /// <summary>
        /// Called when update data is available in order to get the 
        /// engine to refresh it's internal data structures.
        /// This overload is used if the data is a physical file on disk.
        /// </summary>
        /// <param name="dataFileIdentifier">
        /// The identifier of the data file to update.
        /// This engine only uses one data file so this parameter is ignored.
        /// </param>
        public override void RefreshData(string dataFileIdentifier)
        {
            var dataFile = DataFiles.Single();
            if (_engine == null)
            {
                _engine = SwigFactory.CreateEngine(dataFile.DataFilePath, Config, PropertiesConfigSwig);
            }
            else
            {
                try
                {
                    _engine.refreshData();
                }
                catch (ApplicationException e)
                {
                    Logger.LogError(e, $"Failed to refresh data from '{dataFile.DataFilePath}'. " +
                        $"Update service will try again later. If issue persists, please contact " +
                        $"support@51degrees.com");
                    // Just return instead of running any post-update logic.
                    return;
                }
            }
            InitEngineMetaData();
            RefreshCompleted?.Invoke(this, null);
        }

        /// <summary>
        /// Called when update data is available in order to get the 
        /// engine to refresh it's internal data structures.
        /// This overload is used when the data is presented as a 
        /// <see cref="Stream"/>, usually a <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="dataFileIdentifier">
        /// The identifier of the data file to update.
        /// This engine only uses one data file so this parameter is ignored.
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> containing the data to refresh the
        /// engine with.
        /// </param>
        public override void RefreshData(string dataFileIdentifier, Stream stream)
        {
            var data = ReadBytesFromStream(stream);

            if (_engine == null)
            {
                _engine = SwigFactory.CreateEngine(data, data.Length, Config, PropertiesConfigSwig);
            }
            else
            {
                _engine.refreshData(data, data.Length);
            }
            InitEngineMetaData();
            RefreshCompleted?.Invoke(this, null);
        }

        /// <summary>
        /// Perform processing for this engine
        /// </summary>
        /// <param name="data">
        /// The <see cref="IFlowData"/> instance containing data for the 
        /// current request.
        /// </param>
        /// <param name="deviceData">
        /// The <see cref="IDeviceDataHash"/> instance to populate with
        /// property values
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if a required parameter is null
        /// </exception>
        protected override void ProcessEngine(IFlowData data, IDeviceDataHash deviceData)
        {
            if (data == null) { throw new ArgumentNullException(nameof(data)); }
            if (deviceData == null) { throw new ArgumentNullException(nameof(deviceData)); }

            using (var relevantEvidence = new EvidenceDeviceDetectionSwig())
            {
                foreach (var evidenceItem in data.GetEvidence().AsDictionary())
                {
                    if (EvidenceKeyFilter.Include(evidenceItem.Key))
                    {
                        relevantEvidence.Add(new KeyValuePair<string, string>(
                            evidenceItem.Key,
                            evidenceItem.Value.ToString()));
                    }
                }

                // The results object is disposed in the dispose method of the 
                // DeviceDataHash object.
#pragma warning disable CA2000 // Dispose objects before losing scope
                (deviceData as DeviceDataHash).SetResults(new ResultsSwigWrapper(_engine.process(relevantEvidence)));
#pragma warning restore CA2000 // Dispose objects before losing scope
            }
        }

        /// <summary>
        /// Dispose of any unmanaged resources.
        /// </summary>
        protected override void UnmanagedResourcesCleanup()
        {
            if (Config.Object != null)
            {
                Config.Object.Dispose();
            }
            if (_engine != null)
            {
                _engine.Dispose();
            }
        }

        /// <summary>
        /// Constructs a list of the Components associated with the engine.
        /// </summary>
        /// <returns></returns>
        private IList<IComponentMetaData> ConstructComponents()
        {
            var result = new List<IComponentMetaData>();
            using (var components = _engine.getMetaData().getComponents(this))
            {
                foreach (var component in components)
                {
                    result.Add(component);
                }
            }
            result.Add(_deviceMetricsComponent);
            return result;
        }

        /// <summary>
        /// Constructs a list of the properties associated with the engine.
        /// </summary>
        /// <returns></returns>
        private IList<IFiftyOneAspectPropertyMetaData> ConstructProperties()
        {
            var result = new List<IFiftyOneAspectPropertyMetaData>();
            using (var properties = _engine.getMetaData().getProperties(this))
            {
                foreach (var property in properties)
                {
                    result.Add(property);
                }
            }
            result = result.Concat(GetMetricProperties()).ToList();
            return result;
        }

        /// <summary>
        /// Initialize the meta data for the new instance of the underlying
        /// engine.
        /// </summary>
        private void InitEngineMetaData()
        {
            _evidenceKeyFilter = new EvidenceKeyFilterWhitelist(
                new List<string>(_engine.getKeys()),
                StringComparer.InvariantCultureIgnoreCase);
            
            _properties = ConstructProperties();
            _components = ConstructComponents();
            
            // Populate these data file properties from the native engine.
            var dataFileMetaData = GetDataFileMetaData() as IFiftyOneDataFile;
            if (dataFileMetaData != null)
            {
                dataFileMetaData.DataPublishedDateTime = GetDataFilePublishedDate();
                dataFileMetaData.UpdateAvailableTime = GetDataFileUpdateAvailableTime();
                dataFileMetaData.TempDataFilePath = GetDataFileTempPath();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", 
            "CA2000:Dispose objects before losing scope", 
            Justification = "The created instances are used and disposed " +
            "by the parent objects")]
        private IEnumerable<IFiftyOneAspectPropertyMetaData> GetMetricProperties()
        {
            var dataFileList = new List<string>() { "Lite", "Premium", "Enterprise", "TAC", "CloudFree" };
            yield return new FiftyOneAspectPropertyMetaDataHash(
                this,
                "MatchedNodes",
                typeof(int),
                "Device Metrics",
                dataFileList,
                true,
                _deviceMetricsComponent,
                new ValueMetaDataDefault("0"),
                Enumerable.Empty<ValueMetaDataDefault>(),
                "Indicates the number of hash nodes matched within the evidence.");
            yield return new FiftyOneAspectPropertyMetaDataHash(
                this,
                "Difference",
                typeof(int),
                "Device Metrics",
                dataFileList,
                true,
                _deviceMetricsComponent,
                new ValueMetaDataDefault("0"),
                Enumerable.Empty<ValueMetaDataDefault>(),
                "Used when detection method is not Exact or None. This is an integer value and the larger the value the less confident the detector is in this result.");
            yield return new FiftyOneAspectPropertyMetaDataHash(
                this,
                "Drift",
                typeof(int),
                "Device Metrics",
                dataFileList,
                true,
                _deviceMetricsComponent,
                new ValueMetaDataDefault("0"),
                Enumerable.Empty<ValueMetaDataDefault>(),
                "Total difference in character positions where the substrings hashes were found away from where they were expected.");
            yield return new FiftyOneAspectPropertyMetaDataHash(
                this,
                "DeviceId",
                typeof(string),
                "Device Metrics",
                dataFileList,
                true,
                _deviceMetricsComponent,
                new ValueMetaDataDefault("0-0-0-0"),
                Enumerable.Empty<ValueMetaDataDefault>(),
                "Consists of four components separated by a hyphen symbol: Hardware-Platform-Browser-IsCrawler where each Component represents an ID of the corresponding Profile.");
            yield return new FiftyOneAspectPropertyMetaDataHash(
                this,
                "UserAgents",
                typeof(IReadOnlyList<string>),
                "Device Metrics",
                dataFileList,
                true,
                _deviceMetricsComponent,
                new ValueMetaDataDefault("n/a"),
                Enumerable.Empty<ValueMetaDataDefault>(),
                "The matched User-Agents.");
            yield return new FiftyOneAspectPropertyMetaDataHash(
                this,
                "Iterations",
                typeof(int),
                "Device Metrics",
                dataFileList,
                true,
                _deviceMetricsComponent,
                new ValueMetaDataDefault("0"),
                Enumerable.Empty<ValueMetaDataDefault>(),
                "The number of iterations carried out in order to find a match. This is the number of nodes in the graph which have been visited.");
            yield return new FiftyOneAspectPropertyMetaDataHash(
                this,
                "Method",
                typeof(string),
                "Device Metrics",
                dataFileList,
                true,
                _deviceMetricsComponent,
                new ValueMetaDataDefault("NONE"),
                new List<ValueMetaDataDefault>()
                {
                    new ValueMetaDataDefault("NONE"),
                    new ValueMetaDataDefault("PERFORMANCE"),
                    new ValueMetaDataDefault("COMBINED"),
                    new ValueMetaDataDefault("PREDICTIVE")
                },
                "The method used to determine the match result.");
        }

        /// <summary>
        /// Retrieves and returns the 'PublishedDate' from the datafile 
        /// associated with the engine.
        /// </summary>
        /// <returns></returns>
        private DateTime GetDataFilePublishedDate()
        {
            if (_engine != null)
            {
                var value = _engine.getPublishedTime();
                return new DateTime(
                    value.getYear(),
                    value.getMonth(),
                    value.getDay(),
                    0,
                    0, 
                    0,
                    DateTimeKind.Utc);
            }
            return new DateTime();
        }

        /// <summary>
        /// Retrieves and returns the next available update time from the 
        /// datafile associated with the engine.
        /// </summary>
        /// <returns></returns>
        private DateTime GetDataFileUpdateAvailableTime()
        {
            if (_engine != null)
            {
                var value = _engine.getUpdateAvailableTime();
                return new DateTime(
                    value.getYear(),
                    value.getMonth(),
                    value.getDay(), 
                    12,
                    _rng.Next(0, 60), 
                    0,
                    DateTimeKind.Utc);
            }
            return new DateTime();
        }
        /// <summary>
        /// Gets the datafile's temporary file path.
        /// </summary>
        /// <returns></returns>
        private string GetDataFileTempPath()
        {
            return _engine?.getDataFileTempPath();
        }

        /// <summary>
        /// Get the value to use for the 'Type' parameter when calling
        /// the 51Degrees Distributor service to check for a newer 
        /// data file.
        /// </summary>
        /// <param name="identifier">
        /// The identifier of the data file to get the type for.
        /// This engine only uses one file so this parameter is ignored.
        /// </param>
        /// <returns>
        /// A string
        /// </returns>
        public override string GetDataDownloadType(string identifier)
        {
            return _engine.getType();
        }
    }
}
