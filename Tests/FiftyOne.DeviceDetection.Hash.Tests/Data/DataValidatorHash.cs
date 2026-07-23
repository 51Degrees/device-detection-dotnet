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
using FiftyOne.DeviceDetection.TestHelpers.Data;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FiftyOne.DeviceDetection.Hash.Tests.Data
{
    public class DataValidatorHash : IDataValidator
    {
        private DeviceDetectionHashEngine _engine;

        public DataValidatorHash(DeviceDetectionHashEngine engine)
        {
            _engine = engine;
        }

        public void ValidateData(IFlowData data, bool validEvidence = true)
        {
            var elementData = data.GetFromElement(_engine);
            var dict = elementData.AsDictionary();

            foreach (var property in _engine.Properties
                .Where(p => p.Available &&
                    // The JavascriptImageOptimiser property is deprecated.
                    // It exists in the meta-data but is never populated
                    // so we need to ignore it here.
                    p.Name.Equals("JavascriptImageOptimiser", StringComparison.OrdinalIgnoreCase) == false &&
                    p.Name.Equals("JavascriptGetHighEntropyValues", StringComparison.OrdinalIgnoreCase) == false))
            {
                Assert.IsTrue(dict.ContainsKey(property.Name));
                IAspectPropertyValue value = dict[property.Name] as IAspectPropertyValue;
                if (validEvidence)
                {
                    Assert.IsTrue(value.HasValue);
                }
                else
                {
                    if (property.Category.Equals("Device Metrics"))
                    {
                        Assert.IsTrue(value.HasValue);
                    }
                    else
                    {
                        Assert.IsFalse(value.HasValue);
                        if (EvidenceContainsUserAgent(data) == false)
                        {
                            Assert.AreEqual("The evidence required to determine" +
                                " this property was not supplied. The most" +
                                " common evidence passed to this engine is" +
                                " 'header.user-agent'.", value.NoValueMessage);
                        } else
                        {
                            // A User-Agent was supplied but nothing matched.
                            // Since device-detection-cxx #362 removed the
                            // single-User-Agent fast path, detection no longer
                            // produces one coalesced result for this case, so
                            // no result is selected for the property and the
                            // reason reported is NO_RESULT_FOR_PROPERTY rather
                            // than NO_MATCHED_NODES.
                            Assert.AreEqual("None of the results contain a " +
                                "value for the requested property.",
                                value.NoValueMessage);
                        }
                    }
                }
            }
            Assert.IsFalse(string.IsNullOrEmpty(elementData.DeviceId.Value));
            if (validEvidence == false)
            {
                Assert.AreEqual("0-0-0-0", elementData.DeviceId.Value);
            }

            // Since the detection result shape was unified in
            // device-detection-cxx (issue #362), the number of results - and
            // therefore the number of matched User-Agents - is determined by
            // the components the engine can populate, not by how many evidence
            // keys were supplied. Supplying redundant evidence no longer
            // changes it. The exact number depends on which components the data
            // file makes available, so assert the bound rather than a fixed
            // value, and that valid evidence produces at least one result.
            var matchedUserAgents = elementData.UserAgents.Value;
            Assert.IsLessThanOrEqualTo(
                _engine.Components.Count(),
                matchedUserAgents.Count(),
                "There should be no more matched User-Agents than there are " +
                "components populated by the engine.");
            if (validEvidence)
            {
                Assert.IsNotEmpty(matchedUserAgents,
                    "Valid evidence should produce at least one result.");
            }
        }

        public void ValidateProfileIds(IFlowData data, string[] profileIds)
        {
            var elementData = data.GetFromElement(_engine);
            var matchedProfiles = elementData.DeviceId.Value.Split('-');
            foreach (var profileId in profileIds)
            {
                Assert.IsTrue(matchedProfiles.Contains(profileId),
                    $"The profile '{profileId}' was not set in the result.");
            }
        }

        private bool EvidenceContainsUserAgent(IFlowData data)
        {
            return data.TryGetEvidence("header.user-agent", out object _);
        }
    }
}
