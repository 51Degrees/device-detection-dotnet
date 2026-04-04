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
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Services
{
    public interface IPropertyValueQueryService
    {
        /// <summary>
        /// The properties in the <see cref="DeviceDetectionHashEngine"/> to
        /// index.
        /// </summary>
        public IReadOnlyList<string> IndexedProperties { get; }

        /// <summary>
        /// Creates a pipeline that only contains the engine provided.
        /// </summary>
        /// <param name="engine"></param>
        /// <returns>
        /// A pipeline ready to be passed to <see cref="Query(IPipeline)"/>.
        /// </returns>
        IPipeline CreateContext(DeviceDetectionHashEngine engine);

        /// <summary>
        /// Iterates over the profiles and associated data where the profile
        /// contains the properties the service is filtering for.
        /// </summary>
        /// <param name="contextPipeline">
        /// A pipeline created specifically for querying for profiles.
        /// </param>
        /// <returns>
        /// All the relevant profiles and meta data.
        /// </returns>
        IEnumerable<(IProfileMetaData Profile, IDeviceData Data)> Query(
            IPipeline contextPipeline);
    }
}
