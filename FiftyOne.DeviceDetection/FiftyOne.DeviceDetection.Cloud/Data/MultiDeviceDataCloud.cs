﻿using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Cloud.Data
{
    /// <summary>
    /// A data class that is used to access details of cloud responses
    /// that contain multiple device data records.
    /// </summary>
    public class MultiDeviceDataCloud : AspectDataBase, IMultiProfileData<IDeviceData>
    {
        /// <summary>
        /// The key of the 'profiles' list in the base data store.
        /// </summary>
        private const string DEVICE_LIST_KEY = "profiles";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">
        /// The logger instance to use.
        /// </param>
        /// <param name="pipeline">
        /// The Pipeline that created this data instance.
        /// </param>
        /// <param name="engine">
        /// The engine that create this data instance.
        /// </param>
        public MultiDeviceDataCloud(
            ILogger<AspectDataBase> logger, 
            IPipeline pipeline, 
            IAspectEngine engine) :
            base(logger, pipeline, engine)
        {
            Initialize();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">
        /// The logger instance to use.
        /// </param>
        /// <param name="pipeline">
        /// The Pipeline that created this data instance.
        /// </param>
        /// <param name="engine">
        /// The engine that create this data instance.
        /// </param>
        /// <param name="missingPropertyService">
        /// The <see cref="IMissingPropertyService"/> to use if a requested
        /// property does not exist.
        /// </param>
        public MultiDeviceDataCloud(
            ILogger<AspectDataBase> logger, 
            IPipeline pipeline,
            IAspectEngine engine, 
            IMissingPropertyService missingPropertyService) :
            base(logger, pipeline, engine, missingPropertyService)
        {
            Initialize();
        }

        /// <summary>
        /// Create the empty list of device data.
        /// </summary>
        private void Initialize()
        {
            this[DEVICE_LIST_KEY] = new List<IDeviceData>();
        }

        /// <summary>
        /// Get the list of devices.
        /// </summary>
        public IReadOnlyList<IDeviceData> Profiles => GetDeviceList();

        /// <summary>
        /// Add a 'profile' to this list.
        /// </summary>
        /// <remarks>
        /// Note that 'profile' is usually used to refer to data for 
        /// an individual component such as 'hardware' or 'browser'.
        /// In this case, a complete <see cref="IDeviceData"/> is passed,
        /// which may contain data from profiles for multiple components. 
        /// </remarks>
        /// <param name="profile">
        /// The data instance to add to the list.
        /// </param>
        public void AddProfile(IDeviceData profile)
        {
            GetDeviceList().Add(profile);
        }

        private List<IDeviceData> GetDeviceList()
        {
            return this[DEVICE_LIST_KEY] as List<IDeviceData>;
        }
    }
}
