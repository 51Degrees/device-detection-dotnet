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

using FiftyOne.DeviceDetection.PropertyKeyed.Services;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    /// <summary>
    /// Exercises <see cref="DataSetService.BuildProperties"/>, which
    /// wraps the engine's profile properties under the "Profiles"
    /// container and prepends a synthetic ProfileId leaf.
    /// </summary>
    [TestClass]
    public class DataSetServiceTests
    {
        private static Mock<IFiftyOneAspectPropertyMetaData> ProfileProp(
            string name)
        {
            var mock = new Mock<IFiftyOneAspectPropertyMetaData>();
            mock.SetupGet(p => p.Name).Returns(name);
            return mock;
        }

        [TestMethod]
        public void BuildProperties_WrapsEverythingInProfilesContainer()
        {
            var element = Mock.Of<IFlowElement>();

            var result = DataSetService.BuildProperties(
                Array.Empty<IFiftyOneAspectPropertyMetaData>(),
                element);

            Assert.HasCount(1, result);
            Assert.AreEqual("Profiles", result[0].Name);
        }

        [TestMethod]
        public void BuildProperties_PrependsProfileIdBeforeProfileProperties()
        {
            var element = Mock.Of<IFlowElement>();
            var cpu = ProfileProp("CPU").Object;
            var isMobile = ProfileProp("IsMobile").Object;

            var result = DataSetService.BuildProperties(
                new[] { cpu, isMobile }, element);

            var items = result[0].ItemProperties.ToList();
            Assert.HasCount(3, items);
            Assert.AreEqual("ProfileId", items[0].Name);
            Assert.AreEqual("CPU", items[1].Name);
            Assert.AreEqual("IsMobile", items[2].Name);
        }

        [TestMethod]
        public void BuildProperties_ProfileIdIsALeaf()
        {
            var element = Mock.Of<IFlowElement>();

            var result = DataSetService.BuildProperties(
                Array.Empty<IFiftyOneAspectPropertyMetaData>(),
                element);

            var profileId = result[0].ItemProperties[0];
            Assert.IsNull(
                profileId.ItemProperties,
                "ProfileId should be a leaf (ItemProperties = null) " +
                "so callers can tell it apart from container properties.");
        }

        [TestMethod]
        public void BuildProperties_ProfileIdIsTypedAsString()
        {
            var element = Mock.Of<IFlowElement>();

            var result = DataSetService.BuildProperties(
                Array.Empty<IFiftyOneAspectPropertyMetaData>(),
                element);

            var profileId = result[0].ItemProperties[0];
            Assert.AreEqual(typeof(string), profileId.Type);
        }

        [TestMethod]
        public void BuildProperties_EmptyInputStillProducesProfileId()
        {
            var element = Mock.Of<IFlowElement>();

            var result = DataSetService.BuildProperties(
                Array.Empty<IFiftyOneAspectPropertyMetaData>(),
                element);

            var items = result[0].ItemProperties;
            Assert.HasCount(1, items);
            Assert.AreEqual("ProfileId", items[0].Name);
        }
    }
}
