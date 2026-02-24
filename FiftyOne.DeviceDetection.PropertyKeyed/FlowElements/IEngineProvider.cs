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
using System.IO;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// Factory-like interface for obtaining an instance of a 
    /// <see cref="DeviceDetectionHashEngine"/>.
    /// Used by property-keyed engines to decouple engine creation logic.
    /// </summary>
    public interface IEngineProvider
    {
        /// <summary>
        /// Gets an engine instance.
        /// </summary>
        /// <param name="dataFilePath">Optional path to the data file.</param>
        /// <param name="data">Optional stream containing data.</param>
        /// <returns>A built <see cref="DeviceDetectionHashEngine"/> instance.</returns>
        DeviceDetectionHashEngine GetEngine(string dataFilePath = null, Stream data = null);
    }
}
