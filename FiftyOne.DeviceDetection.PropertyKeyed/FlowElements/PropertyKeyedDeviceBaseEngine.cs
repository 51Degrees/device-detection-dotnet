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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.PropertyKeyed.Data;
using FiftyOne.DeviceDetection.PropertyKeyed.Services;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// A generic device detection engine that uses property-keyed 
    /// lookups against a <see cref="DeviceDetectionHashEngine"/>.
    /// Can be configured to validate input and use any property as the key.
    /// </summary>
    public abstract class PropertyKeyedDeviceBaseEngine :
        PropertyKeyedEngine<IMultiDeviceData, IDeviceData>
    {
        private readonly IDataSetService _dataSetService;
        private readonly ILogger<MultiDeviceData> _loggerMultiDd;
        private readonly string _keyProperty;
        private readonly string _elementDataKeyValue;
        private readonly object _dataSetLock = new object();
        private bool _isDataSetInitialized;

        /// <inheritdoc/>
        public override string ElementDataKey => _elementDataKeyValue;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="loggerFactory">
        /// The factory used to create loggers.
        /// </param>
        /// <param name="indexedProperties">
        /// The properties to be indexed.
        /// </param>
        /// <param name="keyProperty">
        /// The property name to use for lookups (e.g. "TAC", "NativeModel").
        /// </param>
        /// <param name="elementDataKey">
        /// The unique key for this engine's data in flow data.
        /// </param>
        public PropertyKeyedDeviceBaseEngine(
            ILoggerFactory loggerFactory,
            IReadOnlyList<string> indexedProperties,
            string keyProperty,
            string elementDataKey) : base(
                loggerFactory,
                indexedProperties)
        {
            if (string.IsNullOrEmpty(keyProperty))
            {
                throw new ArgumentException(
                    "keyProperty is required and cannot be null or empty.",
                    nameof(keyProperty));
            }
            if (string.IsNullOrEmpty(elementDataKey))
            {
                throw new ArgumentException(
                    "elementDataKey is required and cannot be null or empty.",
                    nameof(elementDataKey));
            }

            _loggerMultiDd = loggerFactory.CreateLogger<MultiDeviceData>();
            _keyProperty = keyProperty;
            _elementDataKeyValue = elementDataKey;
            _dataSetService = new DataSetService(
                new PropertyValueQueryService(
                    indexedProperties,
                    loggerFactory),
                loggerFactory.CreateLogger<DataSetService>());
        }

        /// <inheritdoc/>
        protected override string GetKeyPropertyName() => _keyProperty;

        /// <summary>
        /// Creates the element data for each request.
        /// </summary>
        /// <param name="pipeline">The pipeline instance.</param>
        /// <returns>A new MultiDeviceData instance.</returns>
        protected override IMultiDeviceData CreateElementData(
            IPipeline pipeline)
        {
            EnsureDataSetInitialized(pipeline);
            
            return new MultiDeviceData(
                _loggerMultiDd,
                pipeline,
                this,
                Pipeline.Engines.Services.MissingPropertyService.Instance,
                DataSet);
        }

        /// <summary>
        /// Called when pipeline is added. Attempts early initialization.
        /// </summary>
        /// <param name="pipeline">The pipeline being added.</param>
        public override void AddPipeline(IPipeline pipeline)
        {
            if (_isDataSetInitialized == true)
            {
                throw new Exception("Data set already initialized.");
            }
            base.AddPipeline(pipeline);
            EnsureDataSetInitialized(pipeline);
        }

        /// <summary>
        /// Ensures that the DataSet is initialized.
        /// This is a fallback in case AddPipeline wasn't called.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to query for the hash engine.
        /// </param>
        private void EnsureDataSetInitialized(IPipeline pipeline)
        {
            if (_isDataSetInitialized == false)
            {
                lock (_dataSetLock)
                {
                    if (_isDataSetInitialized == false)
                    {
                        DataSet = _dataSetService.BuildDataSet(
                            this, 
                            pipeline);
                        _isDataSetInitialized = true;
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void ProcessProfileMatch(
            IFlowData data,
            IMultiDeviceData aspectData,
            uint profileId)
        {
            aspectData.AddProfileId(profileId);
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDataSet BuildDataSet(
            string dataFilePath)
        {
            throw new NotSupportedException(
                $"{GetType().Name} does not support direct data file " +
                $"loading. It resolves data from " +
                $"{nameof(DeviceDetectionHashEngine)} in the pipeline. " +
                $"Ensure {nameof(DeviceDetectionHashEngine)} is added to " +
                $"the pipeline before this engine.");
        }

        /// <inheritdoc/>
        protected override PropertyKeyedDataSet BuildDataSet(
            Stream data)
        {
            throw new NotSupportedException(
                $"{GetType().Name} does not support direct data stream " +
                $"loading. It resolves data from " +
                $"{nameof(DeviceDetectionHashEngine)} in the pipeline. " +
                $"Ensure {nameof(DeviceDetectionHashEngine)} is added to " +
                $"the pipeline before this engine.");
        }
    }
}
