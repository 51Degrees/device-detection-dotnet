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

using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Data
{
    /// <summary>
    /// Implementation of <see cref="IMultiDeviceData"/> that holds 
    /// multiple <see cref="IDeviceData"/> profiles and manages 
    /// associated <see cref="IFlowData"/> for disposal.
    /// </summary>
    public class MultiDeviceData :
        AspectDataBase,
        IMultiDeviceData
    {
        private readonly string _key;
        private readonly List<IFlowData> _flowDatas = new List<IFlowData>();
        private bool _disposedValue;

        /// <inheritdoc/>
        public IReadOnlyList<IDeviceData> Profiles => GetDeviceList();

        /// <summary>
        /// Constructs a new instance of <see cref="MultiDeviceData"/>.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="pipeline"></param>
        /// <param name="engine"></param>
        /// <param name="missingPropertyService"></param>
        public MultiDeviceData(
            ILogger<MultiDeviceData> logger,
            IPipeline pipeline,
            IAspectEngine engine,
            IMissingPropertyService missingPropertyService)
            : base(logger, pipeline, engine, missingPropertyService)
        {
            _key = "Profiles";
            this[_key] = new List<IDeviceData>();
        }

        /// <summary>
        /// Add a flow data instance. The IDeviceData from the flow data 
        /// is extracted and added to the profiles list. The flow data is
        /// tracked for disposal.
        /// </summary>
        /// <param name="flowData"></param>
        public void AddFlowData(IFlowData flowData)
        {
            _flowDatas.Add(flowData);
            GetDeviceList().Add(flowData.Get<IDeviceData>());
        }

        /// <summary>
        /// Not supported. Use <see cref="AddFlowData"/> instead.
        /// </summary>
        /// <param name="profile"></param>
        public void AddProfile(IDeviceData profile)
        {
            throw new NotImplementedException(
                "Use AddFlowData(IFlowData data) instead because the " +
                "instance of associated flow data needs to be tracked and " +
                "released by this implementation.");
        }

        /// <summary>
        /// Disposes all tracked flow data instances.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                foreach (var data in _flowDatas)
                {
                    data.Dispose();
                }
                _disposedValue = true;
            }
        }

        private List<IDeviceData> GetDeviceList()
        {
            return this[_key] as List<IDeviceData>;
        }
    }
}
