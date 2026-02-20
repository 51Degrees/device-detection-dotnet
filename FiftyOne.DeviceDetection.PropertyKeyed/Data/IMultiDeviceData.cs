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

namespace FiftyOne.DeviceDetection.PropertyKeyed.Data
{
    /// <summary>
    /// Extends <see cref="Pipeline.Engines.Data.IMultiProfileData{T}"/> 
    /// for device data profiles with the ability to track and 
    /// dispose associated <see cref="IFlowData"/> instances.
    /// </summary>
    public interface IMultiDeviceData
        : Pipeline.Engines.Data.IMultiProfileData<IDeviceData>
    {
        /// <summary>
        /// Adds a flow data instance that will be disposed when this 
        /// instance is disposed.
        /// </summary>
        /// <param name="flowData"></param>
        void AddFlowData(IFlowData flowData);
    }
}
