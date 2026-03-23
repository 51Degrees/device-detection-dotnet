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

using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Data
{
    /// <summary>
    /// Implementation of <see cref="IMultiDeviceData"/> that stores
    /// profile IDs and resolves them to <see cref="IDeviceData"/> 
    /// instances on demand. Manages lifecycle of created flow data
    /// instances for proper disposal.
    /// </summary>
    public class MultiDeviceData :
        AspectDataBase,
        IMultiDeviceData,
        IDisposable
    {
        private const string ProfilesKey = "Profiles";
        
        private readonly List<uint> _profileIds = new List<uint>();
        private readonly PropertyKeyedDataSet _dataSet;
        private List<IDeviceData> _cachedProfiles;
        private readonly List<Pipeline.Core.Data.IFlowData> _ownedFlowDatas = 
            new List<Pipeline.Core.Data.IFlowData>();
        private readonly object _lock = new object();
        private bool _disposed;

        /// <inheritdoc/>
        public IReadOnlyList<IDeviceData> Profiles
        {
            get
            {
                if (_cachedProfiles == null)
                {
                    lock (_lock)
                    {
                        if (_cachedProfiles == null)
                        {
                            _cachedProfiles = ResolveProfiles();
                        }
                    }
                }
                return _cachedProfiles.AsReadOnly();
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="MultiDeviceData"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="pipeline">The pipeline this data belongs to.</param>
        /// <param name="engine">The engine that created this data.</param>
        /// <param name="missingPropertyService">Service for handling missing properties.</param>
        /// <param name="dataSet">The data set containing pipeline reference for profile resolution.</param>
        public MultiDeviceData(
            ILogger<MultiDeviceData> logger,
            IPipeline pipeline,
            IAspectEngine engine,
            IMissingPropertyService missingPropertyService,
            PropertyKeyedDataSet dataSet)
            : base(logger, pipeline, engine, missingPropertyService)
        {
            _dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
            this[ProfilesKey] = new List<IDeviceData>();
        }

        /// <inheritdoc/>
        public void AddProfileId(uint profileId)
        {
            _profileIds.Add(profileId);
            // Invalidate cache when new profile is added
            _cachedProfiles = null;
        }

        /// <summary>
        /// Adds a profile directly. Used internally during resolution.
        /// </summary>
        /// <param name="profile">The device data to add.</param>
        public void AddProfile(IDeviceData profile)
        {
            var list = this[ProfilesKey] as List<IDeviceData>;
            list?.Add(profile);
        }

        /// <summary>
        /// Resolves all stored profile IDs to IDeviceData instances.
        /// </summary>
        private List<IDeviceData> ResolveProfiles()
        {
            var result = new List<IDeviceData>();
            
            if (_dataSet?.Pipeline == null)
            {
                return result;
            }

            foreach (var profileId in _profileIds)
            {
                var flowData = _dataSet.Pipeline.CreateFlowData();
                flowData.AddEvidence(
                    Shared.Constants.EVIDENCE_PROFILE_IDS_KEY,
                    profileId.ToString());
                flowData.Process();
                
                var deviceData = flowData.Get<IDeviceData>();
                if (deviceData != null)
                {
                    result.Add(deviceData);
                    // Track for disposal
                    _ownedFlowDatas.Add(flowData);
                }
                else
                {
                    // If no device data, dispose immediately
                    flowData.Dispose();
                }
            }
            
            return result;
        }

        /// <summary>
        /// Disposes all owned flow data instances.
        /// </summary>
        /// <param name="disposing">True if called from Dispose().</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    foreach (var flowData in _ownedFlowDatas)
                    {
                        flowData?.Dispose();
                    }
                    _ownedFlowDatas.Clear();
                    _cachedProfiles?.Clear();
                }
                _disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
