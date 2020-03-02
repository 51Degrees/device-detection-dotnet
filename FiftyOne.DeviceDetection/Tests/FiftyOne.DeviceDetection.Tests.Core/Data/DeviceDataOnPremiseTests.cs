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

using FiftyOne.DeviceDetection.Shared;
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Tests.Core.Data
{
    [TestClass]
    public class DeviceDataOnPremiseTests
    {
        private Mock<ILogger<AspectDataBase>> _logger;

        private Mock<IFlowData> _flowData;

        private Mock<IAspectEngine> _engine;

        private Mock<IMissingPropertyService> _missingPropertyService;

        private static string _testPropertyName = "testproperty";

        /// <summary>
        /// Test class to extend the results wrapper and add a single property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class TestResults<T> : DeviceDataBaseOnPremise
        {
            private object _value;

            internal TestResults(
                ILogger<AspectDataBase> logger,
                IPipeline pipeline,
                IAspectEngine engine,
                IMissingPropertyService missingPropertyService,
                object value)
                : base(logger, pipeline, engine, missingPropertyService)
            {
                _value = value;
            }

            protected override IAspectPropertyValue<bool> GetValueAsBool(string propertyName)
            {
                if (propertyName == _testPropertyName)
                {
                    return new AspectPropertyValue<bool>((bool)_value);
                }
                else
                {
                    throw new PropertyMissingException();
                }
            }

            protected override IAspectPropertyValue<double> GetValueAsDouble(string propertyName)
            {
                if (propertyName == _testPropertyName)
                {
                    return new AspectPropertyValue<double>((double)_value);
                }
                else
                {
                    throw new PropertyMissingException();
                }
            }

            protected override IAspectPropertyValue<int> GetValueAsInteger(string propertyName)
            {
                if (propertyName == _testPropertyName)
                {
                    return new AspectPropertyValue<int>((int)_value);
                }
                else
                {
                    throw new PropertyMissingException();
                }
            }

            protected override IAspectPropertyValue<string> GetValueAsString(string propertyName)
            {
                if (propertyName == _testPropertyName)
                {
                    return new AspectPropertyValue<string>((string)_value);
                }
                else
                {
                    throw new PropertyMissingException();
                }
            }

            protected override IAspectPropertyValue<JavaScript> GetValueAsJavaScript(string propertyName)
            {
                if (propertyName == _testPropertyName)
                {
                    return new AspectPropertyValue<JavaScript>(new JavaScript((string)_value));
                }
                else
                {
                    throw new PropertyMissingException();
                }
            }

            public override IAspectPropertyValue<IReadOnlyList<string>> GetValues(string propertyName)
            {
                if (propertyName == _testPropertyName)
                {
                    return new AspectPropertyValue<IReadOnlyList<string>>((IReadOnlyList<string>)_value);
                }
                else
                {
                    throw new PropertyMissingException();
                }
            }
            protected override bool PropertyIsAvailable(string propertyName)
            {
                return propertyName == _testPropertyName;
            }
        }

        private void SetupElementProperties(Type type)
        {
            var properties = new Dictionary<string, IElementPropertyMetaData>();
            var property = new ElementPropertyMetaData(
                _engine.Object,
                _testPropertyName,
                type,
                true,
                "category");
            properties.Add(_testPropertyName, property);
            var elementProperties = new Dictionary<string, IReadOnlyDictionary<string, IElementPropertyMetaData>>();
            elementProperties.Add(_engine.Object.ElementDataKey, properties);
            _flowData.SetupGet(f => f.Pipeline.ElementAvailableProperties)
                .Returns(elementProperties);
        }

        /// <summary>
        /// Initialise the test instance.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            _logger = new Mock<ILogger<AspectDataBase>>();
            _missingPropertyService = new Mock<IMissingPropertyService>();
            _engine = new Mock<IAspectEngine>();
            _engine.SetupGet(e => e.ElementDataKey).Returns("test");
            _flowData = new Mock<IFlowData>();
            var pipeline = new Mock<IPipeline>();
            _flowData.Setup(f => f.Pipeline).Returns(pipeline.Object);
        }

        /// <summary>
        /// Check that a string list is returned from the internal results
        /// instance using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetList()
        {
            SetupElementProperties(typeof(IReadOnlyList<string>));
            IReadOnlyList<string> expected = new List<string>();
            TestResults<IReadOnlyList<string>> results =
                new TestResults<IReadOnlyList<string>>(
                    _logger.Object,
                    _flowData.Object.Pipeline,
                    _engine.Object,
                    _missingPropertyService.Object,
                    expected);

            var value = results[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(value.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)value).Value);
            var dict = results.AsDictionary();
            Assert.IsTrue(dict.ContainsKey(_testPropertyName));
            var dictValue = dict[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(dictValue.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)dictValue).Value);
        }

        /// <summary>
        /// Check that a string is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetString()
        {
            SetupElementProperties(typeof(string));
            string expected = "string";
            TestResults<string> results =
                new TestResults<string>(
                    _logger.Object,
                    _flowData.Object.Pipeline,
                    _engine.Object,
                    _missingPropertyService.Object,
                    expected);

            var value = results[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(value.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)value).Value);
            var dict = results.AsDictionary();
            Assert.IsTrue(dict.ContainsKey(_testPropertyName));
            var dictValue = dict[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(dictValue.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)dictValue).Value);
        }

        /// <summary>
        /// Check that a bool is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetBool()
        {
            SetupElementProperties(typeof(bool));
            bool expected = true;
            TestResults<bool> results =
                new TestResults<bool>(
                    _logger.Object,
                    _flowData.Object.Pipeline,
                    _engine.Object,
                    _missingPropertyService.Object,
                    expected);

            var value = results[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(value.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)value).Value);
            var dict = results.AsDictionary();
            Assert.IsTrue(dict.ContainsKey(_testPropertyName));
            var dictValue = dict[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(dictValue.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)dictValue).Value);
        }

        /// <summary>
        /// Check that a int is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetInt()
        {
            SetupElementProperties(typeof(int));
            int expected = 1;
            TestResults<int> results =
                new TestResults<int>(
                    _logger.Object,
                    _flowData.Object.Pipeline,
                    _engine.Object,
                    _missingPropertyService.Object,
                    expected);

            var value = results[_testPropertyName];
            Assert.IsTrue(value is IAspectPropertyValue);
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(value.GetType()));
            var dict = results.AsDictionary();
            Assert.IsTrue(dict.ContainsKey(_testPropertyName));
            var dictValue = dict[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(dictValue.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)dictValue).Value);
        }

        /// <summary>
        /// Check that a double is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetDouble()
        {
            SetupElementProperties(typeof(double));
            double expected = 1;
            TestResults<double> results =
                new TestResults<double>(
                    _logger.Object,
                    _flowData.Object.Pipeline,
                    _engine.Object,
                    _missingPropertyService.Object,
                    expected);

            var value = results[_testPropertyName];
            Assert.IsTrue(value is IAspectPropertyValue);
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(value.GetType()));
            var dict = results.AsDictionary();
            Assert.IsTrue(dict.ContainsKey(_testPropertyName));
            var dictValue = dict[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(dictValue.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)dictValue).Value);
        }

        /// <summary>
        /// Check that a double is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetJavaScript()
        {
            SetupElementProperties(typeof(JavaScript));
            string expectedString = "javascript";
            JavaScript expected = new JavaScript(expectedString);
            TestResults<JavaScript> results =
                new TestResults<JavaScript>(
                    _logger.Object,
                    _flowData.Object.Pipeline,
                    _engine.Object,
                    _missingPropertyService.Object,
                    expectedString);

            var value = results[_testPropertyName];
            Assert.IsTrue(value is IAspectPropertyValue);
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(value.GetType()));
            var dict = results.AsDictionary();
            Assert.IsTrue(dict.ContainsKey(_testPropertyName));
            var dictValue = dict[_testPropertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(dictValue.GetType()));
            Assert.AreEqual(expected, ((IAspectPropertyValue)dictValue).Value);
        }
    }
}
