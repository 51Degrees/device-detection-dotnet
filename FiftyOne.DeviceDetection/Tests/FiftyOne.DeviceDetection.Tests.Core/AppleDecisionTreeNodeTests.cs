using FiftyOne.DeviceDetection.Apple;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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
