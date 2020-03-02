/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL) 
 * v.1.2 and is subject to its terms as set out below.
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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.TestHelpers.Data;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                .Where(p => p.Available))
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
                    }
                }
            }
            Assert.IsTrue(string.IsNullOrEmpty(elementData.DeviceId.Value) == false);
            if (validEvidence == false)
            {
                Assert.AreEqual("0-0-0-0", elementData.DeviceId.Value);
            }

            var validKeys = data.GetEvidence().AsDictionary().Keys.Where(
                k => _engine.EvidenceKeyFilter.Include(k)).Count();
            Assert.AreEqual(validKeys, elementData.UserAgents.Value.Count);
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
    }
}
