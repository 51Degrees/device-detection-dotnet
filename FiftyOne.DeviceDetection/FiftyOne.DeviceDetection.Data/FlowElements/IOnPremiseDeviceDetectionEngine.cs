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

using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Shared.FlowElements
{
    /// <summary>
    /// Interface for additional members that are unique to 
    /// on-premise device detection engines.
    /// See the <see href="https://github.com/51Degrees/specifications/blob/main/device-detection-specification/pipeline-elements/device-detection-on-premise.md">Specification</see>
    /// </summary>
    public interface IOnPremiseDeviceDetectionEngine
    {
        /// <summary>
        /// This event is fired whenever the data that this engine makes use
        /// of has been updated.
        /// </summary>
        event EventHandler<EventArgs> RefreshCompleted;

        /// <summary>
        /// Get meta-data for all components that are populated by
        /// this engine.
        /// </summary>
        IEnumerable<IComponentMetaData> Components { get; }

        /// <summary>
        /// Get meta-data for all profiles that can the returned by
        /// this engine.
        /// </summary>
        IEnumerable<IProfileMetaData> Profiles { get; }
    }
}
