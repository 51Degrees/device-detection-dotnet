/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Interop;
using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.Wrappers;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiftyOne.DeviceDetection.Pattern.Tests.Core.FlowElements
{
    [TestClass]
    public class EngineAndBuilderTests
    {
        DeviceDetectionPatternEngineBuilder _builder;

        /// <summary>
        /// Setup the builder to create a mock swig engine that will mostly 
        /// return empty values.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            _builder = new DeviceDetectionPatternEngineBuilder(new LoggerFactory());

            // Setup values returned by IMetaDataSwigWrapper
            var properties = new Mock<IPropertyCollectionSwigWrapper>();
            properties.Setup(c => c.GetEnumerator()).Returns(new List<IFiftyOneAspectPropertyMetaData>().GetEnumerator());
            var components = new Mock<IComponentCollectionSwigWrapper>();
            components.Setup(c => c.GetEnumerator()).Returns(new List<IComponentMetaData>().GetEnumerator());

            // Setup values returned by IEngineSwigWrapper
            var swigMetaData = new Mock<IMetaDataSwigWrapper>();
            swigMetaData.Setup(d => d.getProperties(It.IsAny<DeviceDetectionPatternEngine>())).Returns(properties.Object);
            swigMetaData.Setup(d => d.getComponents(It.IsAny<DeviceDetectionPatternEngine>())).Returns(components.Object);
            var date = new Mock<IDateSwigWrapper>();
            date.Setup(d => d.getYear()).Returns(2019);
            date.Setup(d => d.getMonth()).Returns(12);
            date.Setup(d => d.getDay()).Returns(10);

            // Setup swig engine wrapper
            var swigEngine = new Mock<IEngineSwigWrapper>();
            swigEngine.Setup(e => e.getKeys()).Returns(new VectorStringSwig());
            swigEngine.Setup(e => e.getMetaData()).Returns(swigMetaData.Object);
            swigEngine.Setup(e => e.getPublishedTime()).Returns(date.Object);
            swigEngine.Setup(e => e.getUpdateAvailableTime()).Returns(date.Object);

            // Setup factory to create our mock engine rather than a real one
            var factory = new SwigFactory();
            factory.EngineFromData = (a, b, c, d) => { return swigEngine.Object; };

            // Assign the factory to the builder
            _builder.SwigFactory = factory;
            _builder.SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false);
        }

        /// <summary>
        /// Create a new engine and check that the data file associated 
        /// with the engine has had its 'Engine' property set correctly.
        /// </summary>
        [TestMethod]
        public void EngineBuilder_CheckDataFileEngineSet()
        {
            var engine = _builder.Build(new MemoryStream());
            Assert.AreEqual(engine, engine.DataFiles.ElementAt(0).Engine);
        }
    }
}
