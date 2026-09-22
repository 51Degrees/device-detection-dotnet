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
using Microsoft.Extensions.Logging;
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

            // A TAC that cannot be used is caller input, not a server
            // fault, so it is reported on the warnings channel and never as
            // an error.
            //
            // This used to call AddError. Any entry in FlowData.Errors makes
            // Pipeline.Process throw once processing finishes unless the host
            // sets SuppressProcessExceptions - the shouldThrow flag on the
            // individual error only filters which exceptions are wrapped, not
            // whether the throw happens. The cloud does not suppress, so one
            // unusable value returned an errors array instead of the response,
            // client pipelines discarded the whole result, and a single bad
            // row aborted an entire batch import.
            //
            // A value that matches nothing already yields no profiles rather
            // than an error, and a malformed one has to behave the same way.
            data.AddWarning(
                string.Format(
                    Messages.IncorrectTacEvidence,
                    keyPropertyValue),
                data.Pipeline.GetElement<TacEngine>());
            return false;
        }
    }
}
