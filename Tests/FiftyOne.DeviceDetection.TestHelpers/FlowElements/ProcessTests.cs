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

using FiftyOne.DeviceDetection.Shared.FlowElements;
using FiftyOne.DeviceDetection.Shared.Services;
using FiftyOne.DeviceDetection.TestHelpers.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FiftyOne.DeviceDetection.TestHelpers.FlowElements
{
    public class ProcessTests
    {
        public static void NoEvidence(IWrapper wrapper, IDataValidator validator)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.Process();
                validator.ValidateData(data, false);
            }
        }

        public static void EmptyUserAgent(IWrapper wrapper, IDataValidator validator)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.user-agent", "")
                    .Process();
                validator.ValidateData(data, false);
            }
        }

        public static void NoHeaders(IWrapper wrapper, IDataValidator validator)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("irrelevant.evidence", "some evidence")
                    .Process();
                validator.ValidateData(data, false);
            }
        }

        public static void NoUsefulHeaders(IWrapper wrapper, IDataValidator validator)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.irrelevant-header", "some evidence")
                    .Process();
                validator.ValidateData(data, false);
            }
        }

        public static void CaseInsensitiveEvidenceKeys(IWrapper wrapper, IDataValidator validator)
        {
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.USER-AGENT", Constants.MobileUserAgent)
                    .Process();
                validator.ValidateData(data);
            }
        }

        public static void ProfileOverride(IWrapper wrapper, IDataValidator validator)
        {
            string[] profileIds = { "12280", "17779", "17470", "18092" };
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence("header.user-agent", "some user agent")
                    .AddEvidence(Shared.Constants.EVIDENCE_PROFILE_IDS_KEY, string.Join("|", profileIds))
                    .Process();
                validator.ValidateProfileIds(data, profileIds);
            }
        }

        public static void ProfileOverrideNoHeaders(IWrapper wrapper, IDataValidator validator)
        {
            string[] profileIds = { "12280", "17779", "17470", "18092" };
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence(Shared.Constants.EVIDENCE_PROFILE_IDS_KEY, string.Join("|", profileIds))
                    .Process();
                validator.ValidateProfileIds(data, profileIds);
            }
        }

        public static void DeviceId(IWrapper wrapper, IDataValidator validator)
        {
            string[] profileIds = { "12280", "17779", "17470", "18092" };
            using (var data = wrapper.Pipeline.CreateFlowData())
            {
                data.AddEvidence(Shared.Constants.EVIDENCE_PROFILE_IDS_KEY, string.Join("-", profileIds))
                    .Process();
                validator.ValidateProfileIds(data, profileIds);
            }
        }

        public static void MetaDataService_DefaultProfilesIds(IWrapper wrapper)
        {
            var service = new MetaDataService(
                wrapper.Pipeline.FlowElements
                    .Where(e => typeof(IOnPremiseDeviceDetectionEngine).IsAssignableFrom(e.GetType()))
                    .Cast<IOnPremiseDeviceDetectionEngine>()
                    .ToArray());
            var defaultProfiles = service.DefaultProfilesIds();
            // Expect 5 components:
            // hardware, platform, browser, crawler and metrics.
            // metrics does not actually exist in the data file and
            // does not have a default profile so it will be null.
            Assert.AreEqual(5, defaultProfiles.Count);
            Assert.IsTrue(defaultProfiles.ContainsKey(1));
            Assert.IsTrue(defaultProfiles.ContainsKey(2));
            Assert.IsTrue(defaultProfiles.ContainsKey(3));
            Assert.IsTrue(defaultProfiles.ContainsKey(4));
            Assert.IsTrue(defaultProfiles.ContainsKey(255));
            Assert.AreEqual((uint)15364, defaultProfiles[1]);
            Assert.AreEqual((uint)17017, defaultProfiles[2]);
            Assert.AreEqual((uint)17470, defaultProfiles[3]);
            Assert.AreEqual((uint)18092, defaultProfiles[4]);
            Assert.IsNull(defaultProfiles[255]);
        }

        public static void MetaDataService_DefaultProfileIdForComponent(IWrapper wrapper)
        {
            var service = new MetaDataService(
                wrapper.Pipeline.FlowElements
                    .Where(e => typeof(IOnPremiseDeviceDetectionEngine).IsAssignableFrom(e.GetType()))
                    .Cast<IOnPremiseDeviceDetectionEngine>()
                    .ToArray());
            var defaultProfile1 = service.DefaultProfileIdForComponent(1);
            var defaultProfile99 = service.DefaultProfileIdForComponent(99);
            var defaultProfile255 = service.DefaultProfileIdForComponent(255);
            // Expect 5 components:
            // hardware, platform, browser, crawler and metrics.
            // metrics does not actually exist in the data file and
            // does not have a default profile so it will be null.
            Assert.AreEqual((uint)15364, defaultProfile1);
            Assert.IsNull(defaultProfile99);
            Assert.IsNull(defaultProfile255);
        }

        public static void MetaDataService_ComponentIdForProfile(IWrapper wrapper)
        {
            var service = new MetaDataService(
                wrapper.Pipeline.FlowElements
                    .Where(e => typeof(IOnPremiseDeviceDetectionEngine).IsAssignableFrom(e.GetType()))
                    .Cast<IOnPremiseDeviceDetectionEngine>()
                    .ToArray());
            var comonentFor15364 = service.ComponentIdForProfile(15364);
            var comonentFor17017 = service.ComponentIdForProfile(17017);
            var comonentFor17470 = service.ComponentIdForProfile(17470);
            var comonentFor18092 = service.ComponentIdForProfile(18092);
            var comonentFor999999999 = service.ComponentIdForProfile(999999999);
            var comonentFor0 = service.ComponentIdForProfile(0);
            // Expect 5 components:
            // hardware, platform, browser, crawler and metrics.
            // metrics does not actually exist in the data file and
            // does not have a default profile so it will be null.
            Assert.AreEqual((byte)1, comonentFor15364);
            Assert.AreEqual((byte)2, comonentFor17017);
            Assert.AreEqual((byte)3, comonentFor17470);
            Assert.AreEqual((byte)4, comonentFor18092);
            Assert.IsNull(comonentFor999999999);
            Assert.IsNull(comonentFor0);
        }
    }
}
