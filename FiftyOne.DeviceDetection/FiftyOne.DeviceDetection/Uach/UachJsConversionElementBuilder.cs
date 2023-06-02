/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2023 51 Degrees Mobile Experts Limited, Davidson House,
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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Uach
{
    /// <summary>
    /// Builder for <see cref="UachJsConversionElement"/> instances.
    /// </summary>
    public class UachJsConversionElementBuilder
    {
        private ILoggerFactory _loggerFactory;

        // Since there are no data properties on UachJsConversionData, we just use
        // a static instance (per pipeline) to avoid unnecessary allocations.
        private static Dictionary<IPipeline, UachJsConversionData> _staticData = 
            new Dictionary<IPipeline, UachJsConversionData>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerFactory">
        /// The logger factory for this builder to use when creating new 
        /// instances.
        /// </param>
        public UachJsConversionElementBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Build and return a new <see cref="UachJsConversionElement"/> with
        /// the currently configured settings.
        /// </summary>
        /// <returns>
        /// a new <see cref="UachJsConversionElement"/>
        /// </returns>
        public UachJsConversionElement Build()
        {
            return new UachJsConversionElement(
                _loggerFactory.CreateLogger<UachJsConversionElement>(), GetData);
        }

        /// <summary>
        /// Factory method for returning the <see cref="UachJsConversionData"/> instance that
        /// will be used by <see cref="UachJsConversionElement"/>.
        /// Since this element writes directly to evidence, rather than setting output values,
        /// there are no properties on the data object and we can just re-use a single static 
        /// instance per pipeline.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline that this is part of.
        /// </param>
        /// <param name="element">
        /// The <see cref="UachJsConversionElement"/> the is creating this data instance.
        /// </param>
        /// <returns>
        /// The static <see cref="UachJsConversionData"/> instance.
        /// </returns>
        private UachJsConversionData GetData(
            IPipeline pipeline,
            FlowElementBase<UachJsConversionData, IElementPropertyMetaData> element)
        {
            if(_staticData.TryGetValue(pipeline, out var data) == false)
            {
                lock (_staticData)
                {
                    if (_staticData.TryGetValue(pipeline, out data) == false)
                    {
                        data = new UachJsConversionData(
                            _loggerFactory.CreateLogger<UachJsConversionData>(),
                            pipeline);
                        _staticData.Add(pipeline, data);
                    }
                }
            }
            return data;
        }
    }
}
