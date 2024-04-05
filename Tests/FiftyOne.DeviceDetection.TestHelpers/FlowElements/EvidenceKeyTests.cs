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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FiftyOne.DeviceDetection.TestHelpers.FlowElements
{
    public class EvidenceKeyTests
    {
        public static void ContainsUserAgent(IWrapper wrapper)
        {
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("header.user-agent"));
        }

        public static void ContainsHeaderNames(IWrapper wrapper)
        {
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("header.device-stock-ua"));
        }

        public static void ContainsQueryParams(IWrapper wrapper)
        {
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("query.user-agent"));
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("query.device-stock-ua"));
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("query.51D_deviceId"));
        }

        public static void CaseInsensitiveKeys(IWrapper wrapper)
        {
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("header.User-Agent"));
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("header.user-agent"));
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("header.USER-AGENT"));
            Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("HEADER.USER-AGENT"));
        }

        public static void ContainsOverrides(IWrapper wrapper)
        {

            if (wrapper.GetEngine().Properties
                .Where(p => p.Name == "ScreenPixelsHeightJavaScript" ||
                            p.Name == "ScreenPixelsWidthJavaScript")
                .Count() == 2)
            {
                Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("query.51d_screenpixelswidth"));
                Assert.IsTrue(wrapper.GetEngine().EvidenceKeyFilter.Include("cookie.51d_screenpixelswidth"));
            } 
            else
            {
                Assert.Inconclusive("ScreenPixels Width & Height JavaSript properties are not in the data set.");
            }
        }
    }
}
