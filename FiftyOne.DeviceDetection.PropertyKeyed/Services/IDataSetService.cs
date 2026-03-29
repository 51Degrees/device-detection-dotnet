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
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Services
{
    public interface IDataSetService
    {
        /// <summary>
        /// Builds the data set from the pipeline provided.
        /// </summary>
        /// <param name="element">
        /// The element that already exists in the pipeline that will consume
        /// the resulting data set.
        /// </param>
        /// <param name="pipeline">
        /// The pipeline that must contained an instance of
        /// <see cref="DeviceDetectionHashEngine"/>.
        /// </param>
        /// <param name="stopToken"></param>
        /// <returns>
        /// A new instance of a data set populated with property values from
        /// the engine.
        /// </returns>>
        Task<PropertyKeyedDataSet> BuildDataSet(
            IFlowElement element,
            IPipeline pipeline,
            CancellationToken stopToken);

        /// <summary>
        /// Create a <see cref="PropertyKeyedDataSet"/> from the engine and
        /// target flow element provided.
        /// </summary>
        /// <param name="element">
        /// The element that already exists in the pipeline that will consume
        /// the resulting data set.
        /// </param>
        /// <param name="engine">
        /// The source engine for property values.
        /// </param>
        /// <param name="stopToken"></param>
        /// <returns>
        /// A new instance of a data set populated with property values from
        /// the engine.
        /// </returns>
        Task<PropertyKeyedDataSet> BuildDataSet(
            IFlowElement element,
            DeviceDetectionHashEngine engine,
            CancellationToken stopToken);
    }
}
