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

using FiftyOne.DeviceDetection.PropertyKeyed.Data;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// An engine that looks up device profiles using TAC (Type Allocation 
    /// Code) values — the first 8 digits of an IMEI number.
    /// This engine is configured to key on the "TAC" property and validates 
    /// that the provided evidence is exactly 8 numeric digits.
    /// </summary>
    public class TacKeyedEngine : PropertyKeyedDeviceEngine
    {

        /// <inheritdoc/>
        public override string ElementDataKey => "hardware";

        /// <summary>
        /// Constructs a new instance of <see cref="TacKeyedEngine"/>.
        /// </summary>
        public TacKeyedEngine(
            ILoggerFactory loggerFactory,
            IReadOnlyList<string> indexedProperties,
            IEngineProvider engineProvider = null) : base(
                loggerFactory,
                indexedProperties,
                engineProvider)
        {
        }

        /// <inheritdoc/>
        protected override string GetKeyPropertyName()
        {
            return "TAC";
        }

        /// <inheritdoc/>
        protected override bool Validate(string keyPropertyValue, IFlowData data)
        {
            if (keyPropertyValue.Length == 8 && int.TryParse(keyPropertyValue, out var _))
            {
                return true;
            }

            data.AddError(
                new ArgumentException(String.Format(
                    Messages.IncorrectTacEvidence,
                    keyPropertyValue)),
                this);
            return false;
        }
    }
}
