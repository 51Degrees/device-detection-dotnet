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
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;

namespace FiftyOne.DeviceDetection.PropertyKeyed
{
    public static class Extensions
    {
        /// <summary>
        /// Returns the <see cref="DeviceDetectionHashEngine"/> if present in
        /// the pipeline.
        /// </summary>
        /// <param name="pipeline"></param>
        /// <returns>
        /// The instance of <see cref="DeviceDetectionHashEngine"/> from the
        /// pipeline.
        /// </returns>
        /// <exception cref="PipelineConfigurationException">
        /// Thrown if there is no instance of 
        /// <see cref="DeviceDetectionHashEngine"/> available.
        /// </exception>
        public static DeviceDetectionHashEngine GetDeviceDetectionHashEngine(
            this IPipeline pipeline)
        {
            var engine = pipeline.GetElement<DeviceDetectionHashEngine>();
            if (engine == null)
            {
                throw new PipelineConfigurationException(
                    $"Ensure {nameof(DeviceDetectionHashEngine)} is added " +
                    $"to the pipeline");
            }
            return engine;
        }
    }
}
