/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.DeviceDetection.Apple;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiftyOne.DeviceDetection.Tests.Core
{
    [TestClass]
    public class AppleDecisionTreeNodeTests
    {
        /// <summary>
        /// Verify that the match method on AppleDecisionTreeNodeRange works as expected.
        /// </summary>
        [TestMethod]
        public void TestRange()
        {
            var node = new AppleDecisionTreeNodeRange()
            {
                EvidenceMin = 10.1F,
                EvidenceMax = 20.2F
            };

            Assert.IsFalse(node.Match(10.09F));
            Assert.IsTrue(node.Match(10.1F));
            Assert.IsTrue(node.Match(20.1F));
            Assert.IsTrue(node.Match(20.2F));
            Assert.IsFalse(node.Match(20.21F));
        }
    }
}
