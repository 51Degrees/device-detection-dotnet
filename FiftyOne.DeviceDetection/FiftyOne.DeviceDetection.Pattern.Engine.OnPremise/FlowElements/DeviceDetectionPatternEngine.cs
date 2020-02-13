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
using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Wrappers;
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.DeviceDetection.Shared.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.FlowElements
{
    /// <summary>
    /// Pattern device detection engine. This engine takes User-Agents and
    /// other relevant HTTP headers and returns properties about the device
    /// which produced them e.g. DeviceType or ReleaseDate.
    /// </summary>
    public class DeviceDetectionPatternEngine : 
        OnPremiseDeviceDetectionEngineBase<IDeviceDataPattern>
    {
        private ISwigFactory _swigFactory;
        private IEngineSwigWrapper _engine;

        private IEvidenceKeyFilter _evidenceKeyFilter;

        private IList<IFiftyOneAspectPropertyMetaData> _properties;
        private IList<IComponentMetaData> _components;

        private IConfigSwigWrapper _config;
        private IRequiredPropertiesConfigSwigWrapper _propertiesConfigSwig;

        // The component used for metric properties.
        private ComponentMetaDataDefault _deivceMetricsComponent = new ComponentMetaDataPattern("Metrics");
        
        private readonly object _propertiesLock = new object();

        /// <summary>
        /// This event is fired whenever the data that this engine makes use
        /// of has been updated.
        /// </summary>
        public override event EventHandler<EventArgs> RefreshCompleted;

        /// <summary>
        /// Construct a new instance of the Pattern engine.
        /// </summary>
        /// <param name="loggerFactory">Logger to use</param>
        /// <param name="dataFile">Meta data related to the data file</param>
        /// <param name="config">Configuration instance</param>
        /// <param name="properties">Properties to be initialised</param>
        /// <param name="deviceDataFactory">
        /// Method used to get an aspect data instance
        /// </param>
        /// <param name="tempDataFilePath">
        /// The directory to use when storing temporary copies of the 
        /// data file(s) used by this engine.
        /// </param>
        /// <param name="swigFactory">
        /// The factory object to use when creating swig wrapper instances.
        /// Usually a <see cref="SwigFactory"/> instance.
        /// Unit tests can override this to mock behaviour as needed.
        /// </param>
        internal DeviceDetectionPatternEngine(
            ILoggerFactory loggerFactory,
            IAspectEngineDataFile dataFile,
            IConfigSwigWrapper config,
            IRequiredPropertiesConfigSwigWrapper properties,
            Func<IPipeline, FlowElementBase<IDeviceDataPattern, IFiftyOneAspectPropertyMetaData>, IDeviceDataPattern> deviceDataFactory,
            string tempDataFilePath,
            ISwigFactory swigFactory)
            : base(
                  loggerFactory.CreateLogger<DeviceDetectionPatternEngine>(),
                  deviceDataFactory,
                  tempDataFilePath)
        {
            _config = config;
            _propertiesConfigSwig = properties;
            _swigFactory = swigFactory;
            AddDataFile(dataFile);
        }

        public override string ElementDataKey => "device";

        internal IMetaDataSwigWrapper MetaData => _engine.getMetaData();


        public override IList<IFiftyOneAspectPropertyMetaData> Properties
        {
            get
            {
                return _properties;
            }
        }

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

        public override IEnumerable<IComponentMetaData> Components
        {
            get
            {
                return _components;
            }
        }

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

        public override string DataSourceTier => _engine.getType();

        public bool AutomaticUpdatesEnabled => _engine.getAutomaticUpdatesEnabled();

        public override IEvidenceKeyFilter EvidenceKeyFilter => _evidenceKeyFilter;
        
        public override void RefreshData(string dataFileIdentifier)
        {
            var dataFile = DataFiles.Single();
            if (_engine == null)
            {
                _engine = _swigFactory.CreateEngine(dataFile.DataFilePath, _config, _propertiesConfigSwig);
            }
            else
            {
                _engine.refreshData();
            }
            GetEngineMetaData();
            RefreshCompleted?.Invoke(this, null);

        }

        public override void RefreshData(string dataFileIdentifier, Stream stream)
        {
            var data = ReadBytesFromStream(stream);

            if (_engine == null)
            {
                _engine = _swigFactory.CreateEngine(data, data.Length, _config, _propertiesConfigSwig);
            }
            else
            {
                _engine.refreshData(data, data.Length);
            }
            GetEngineMetaData();
            RefreshCompleted?.Invoke(this, null);
        }

        protected override void ProcessEngine(IFlowData data, IDeviceDataPattern deviceData)
        {
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
                (deviceData as DeviceDataPattern).SetResults(_engine.process(relevantEvidence));
            }
        }

        protected override void UnmanagedResourcesCleanup()
        {
            if (_engine != null)
            {
                _engine.Dispose();
            }
        }

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
            result.Add(_deivceMetricsComponent);
            return result;
        }

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

        private void GetEngineMetaData()
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

        /// <summary>
        /// Get the match metric properties. 
        /// </summary>
        /// <returns>Metric Properties</returns>
        private IEnumerable<IFiftyOneAspectPropertyMetaData> GetMetricProperties()
        {
            var dataFileList = new List<string>() { "Lite", "Premium", "Enterprise" };

            yield return new FiftyOneAspectPropertyMetaDataPattern(
                this,
                "Method",
                typeof(string),
                "Device Metrics",
                dataFileList,
                true,
                _deivceMetricsComponent,
                new ValueMetaDataDefault("NONE"),
                "Provides information about the algorithm that was used to perform detection for a particular User-Agent.");
            yield return new FiftyOneAspectPropertyMetaDataPattern(
                this,
                "Rank",
                typeof(int),
                "Device Metrics",
                dataFileList,
                true,
                _deivceMetricsComponent,
                new ValueMetaDataDefault("0"),
                "An integer value that indicates how popular the device is. The lower the rank the more popular the signature.");
            yield return new FiftyOneAspectPropertyMetaDataPattern(
                this,
                "Difference",
                typeof(int),
                "Device Metrics",
                dataFileList,
                true,
                _deivceMetricsComponent,
                new ValueMetaDataDefault("0"),
                "Used when detection method is not Exact or None. This is an integer value and the larger the value the less confident the detector is in this result.");
            yield return new FiftyOneAspectPropertyMetaDataPattern(
                this,
                "SignaturesCompared",
                typeof(int),
                "Device Metrics",
                dataFileList,
                true, 
                _deivceMetricsComponent, 
                new ValueMetaDataDefault("0"),
                "The number of device signatures that have been compared before finding a result.");
            yield return new FiftyOneAspectPropertyMetaDataPattern(
                this,
                "DeviceId",
                typeof(string),
                "Device Metrics",
                dataFileList,
                true,
                _deivceMetricsComponent,
                new ValueMetaDataDefault("0-0-0-0"),
                "Consists of four components separated by a hyphen symbol: Hardware-Platform-Browser-IsCrawler where each Component represents an ID of the corresponding Profile.");
            yield return new FiftyOneAspectPropertyMetaDataPattern(
                this,
                "UserAgents",
                typeof(IReadOnlyList<string>),
                "Device Metrics",
                dataFileList,
                true,
                _deivceMetricsComponent,
                new ValueMetaDataDefault("n/a"),
                "The matched User-Agents.");
        }


        private DateTime GetDataFilePublishedDate()
        {
            if (_engine != null)
            {
                var value = _engine.getPublishedTime();
                return new DateTime(
                    value.getYear(),
                    value.getMonth(),
                    value.getDay());
            }
            return new DateTime();
        }
        private DateTime GetDataFileUpdateAvailableTime()
        {
            if (_engine != null)
            {
                var value = _engine.getUpdateAvailableTime();
                return new DateTime(
                    value.getYear(),
                    value.getMonth(),
                    value.getDay());
            }
            return new DateTime();
        }
        private string GetDataFileTempPath()
        {
            return _engine?.getDataFileTempPath();
        }

        public override string GetDataDownloadType(string identifier)
        {
            return _engine.getType();
        }
    }

}
