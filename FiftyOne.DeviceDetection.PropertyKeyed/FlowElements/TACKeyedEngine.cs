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
    /// Adds TAC-specific validation: a TAC must be exactly 8 numeric digits.
    /// </summary>
    public class TACKeyedEngine : PropertyKeyedDeviceEngine
    {
        private string[] _tacKeys;
        private string[] _otherKeys;

        /// <inheritdoc/>
        public override string ElementDataKey => "hardware";

        /// <summary>
        /// Constructs a new instance of <see cref="TACKeyedEngine"/>.
        /// </summary>
        public TACKeyedEngine(
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

        /// <summary>
        /// Overrides to add TAC-specific validation.
        /// </summary>
        protected override IEnumerable<KeyValuePair<PropertyKeyedIndex, string>>
            GetQueryValues(IFlowData data)
        {
            EnsureKeysInitialised();

            foreach (var key in _tacKeys)
            {
                if (data.TryGetEvidence(key, out string tac))
                {
                    if (tac.Length == 8 && int.TryParse(tac, out var _))
                    {
                        yield return BuildResult(key, tac);
                    }
                    else
                    {
                        data.AddError(
                            new ArgumentException(String.Format(
                                Messages.IncorrectTacEvidence,
                                tac)),
                            this);
                    }
                }
            }
            foreach (var key in _otherKeys)
            {
                if (data.TryGetEvidence(key, out string value))
                {
                    yield return BuildResult(key, value);
                }
            }
        }

        private KeyValuePair<PropertyKeyedIndex, string> BuildResult(
            string key, string value)
        {
            var property = DataSet.PropertyIndexes.First(
                i => i.EvidenceKeys.Contains(key));
            return new KeyValuePair<PropertyKeyedIndex, string>(
                property, value);
        }

        private void EnsureKeysInitialised()
        {
            if (_tacKeys == null)
            {
                var list = DataSet.EvidenceKeyFilter.Whitelist;
                _tacKeys = list.Where(i =>
                    i.Key.EndsWith("tac")).Select(i => i.Key).ToArray();
                _otherKeys = list.Where(i =>
                    _tacKeys.Contains(i.Key) == false).Select(i =>
                    i.Key).ToArray();
            }
        }
    }
}
