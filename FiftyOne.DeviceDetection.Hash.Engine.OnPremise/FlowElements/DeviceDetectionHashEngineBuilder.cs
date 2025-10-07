/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;

namespace FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements
{
    /// <summary>
    /// Complete builder implementation for creating on premise
    /// <see cref="DeviceDetectionHashEngine"/> engines.
    /// </summary>
    public class DeviceDetectionHashEngineBuilder :
        DeviceDetectionHashEngineBuilderBase<DeviceDetectionHashEngine>
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="loggerFactory"></param>
        public DeviceDetectionHashEngineBuilder(
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }
        /// <summary>
        /// Constructor that also provides the DataUpdateService.
        /// </summary>
        public DeviceDetectionHashEngineBuilder(
            ILoggerFactory loggerFactory, 
            IDataUpdateService dataUpdateService) 
            : base(loggerFactory, dataUpdateService)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DeviceDetectionHashEngine"/>.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="deviceDataFactory"></param>
        /// <param name="tempDataFilePath"></param>
        /// <returns></returns>
        protected override DeviceDetectionHashEngine CreateEngine(
            ILoggerFactory loggerFactory,
            Func<IPipeline, FlowElementBase<
                IDeviceDataHash, 
                IFiftyOneAspectPropertyMetaData>, 
                IDeviceDataHash> deviceDataFactory, 
            string tempDataFilePath)
        {
            return new DeviceDetectionHashEngine(
                loggerFactory,
                deviceDataFactory, 
                tempDataFilePath);
        }
    }
}
