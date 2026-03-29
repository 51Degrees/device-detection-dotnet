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
 * including the attribution notice(s) required under Article 5 of the EUPL
 * in the end user terms of the application under an appropriate heading,
 * such notice(s) shall fulfill the requirements of that article.
 * ********************************************************************* */

using FiftyOne.Pipeline.Core.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    public class TacEngine : PropertyKeyedDeviceBaseEngine
    {
        public TacEngine(
            ILoggerFactory loggerFactory, 
            IReadOnlyList<string> indexedProperties, 
            string keyProperty, 
            string elementDataKey) : base(
                loggerFactory, 
                indexedProperties, 
                keyProperty, 
                elementDataKey)
        {
        }

        /// <summary>
        /// Validates TAC format: must be exactly 8 numeric digits.
        /// </summary>
        protected override bool Validate(
            string keyPropertyValue, 
            IFlowData data)
        {
            if (keyPropertyValue.Length == 8 && 
                int.TryParse(keyPropertyValue, out _))
            {
                return true;
            }

            data.AddError(
                new ArgumentException(string.Format(
                    Messages.IncorrectTacEvidence,
                    keyPropertyValue)),
                data.Pipeline.GetElement<PropertyKeyedDeviceBaseEngine>());
            return false;
        }
    }
}
