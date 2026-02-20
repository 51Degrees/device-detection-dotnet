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

using FiftyOne.Pipeline.Engines;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// An engine that looks up device profiles using the NativeModel
    /// property — the internal device model name exposed by the OS.
    /// No special validation is needed for NativeModel values.
    /// </summary>
    public class NativePropertiesEngine : PropertyKeyedDeviceEngine
    {
        /// <inheritdoc/>
        public override string ElementDataKey => "hardware";

        /// <summary>
        /// Constructs a new instance of 
        /// <see cref="NativePropertiesEngine"/>.
        /// </summary>
        public NativePropertiesEngine(
            ILoggerFactory loggerFactory,
            IReadOnlyList<string> indexedProperties,
            PerformanceProfiles performanceProfile,
            ushort concurrency) : base(
                loggerFactory,
                indexedProperties,
                performanceProfile,
                concurrency)
        {
        }
    }
}
