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
    /// <summary>
    /// These tests verify that the DeviceDataBaseOnPremise class is passing 'get property value'
    /// calls to the appropriate type-specific method (which is overridden in the inheriting class) 
    /// </summary>
    [TestClass]
    public class DeviceDataOnPremiseTests
    {
        private Mock<ILogger<AspectDataBase>> _logger;

        private Mock<IFlowData> _flowData;

        private Mock<IAspectEngine> _engine;

        private Mock<IMissingPropertyService> _missingPropertyService;

        private const string _testPropertyNameBool = "testpropertybool";
        private const string _testPropertyNameDouble = "testpropertydouble";
        private const string _testPropertyNameInteger = "testpropertyinteger";
        private const string _testPropertyNameString = "testpropertystring";
        private const string _testPropertyNameJavaScript = "testpropertyjavascript";
        private const string _testPropertyNameList = "testpropertylist";

        private static readonly bool _testValueBool = true;
        private static readonly double _testValueDouble = 1.1;
        private static readonly int _testValueInteger = 1;
        private static readonly string _testValueString = "string";
        private static readonly JavaScript _testValueJavaScript = new JavaScript("javscript");
        private static readonly IReadOnlyList<string> _testValueList = new List<string>();

        private static readonly Dictionary<string, Tuple<Type, object>> _testProperties = 
            new Dictionary<string, Tuple<Type, object>>()
        {
            { _testPropertyNameBool, new Tuple<Type, object>(typeof(bool), _testValueBool) },
            { _testPropertyNameDouble, new Tuple<Type, object>(typeof(double), _testValueDouble) },
            { _testPropertyNameInteger, new Tuple<Type, object>(typeof(int), _testValueInteger) },
            { _testPropertyNameString, new Tuple<Type, object>(typeof(string), _testValueString) },
            { _testPropertyNameJavaScript, new Tuple<Type, object>(typeof(JavaScript), _testValueJavaScript) },
            { _testPropertyNameList, new Tuple<Type, object>(typeof(IReadOnlyList<string>), _testValueList) }
        };

        /// <summary>
        /// Test class to extend the results wrapper and add a single property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class TestResults : DeviceDataBaseOnPremise<IDisposable>
        {
            private Dictionary<string, object> _values = new Dictionary<string, object>();

            internal TestResults(
                ILogger<AspectDataBase> logger,
                IPipeline pipeline,
                IAspectEngine engine,
                IMissingPropertyService missingPropertyService)
                : base(logger, pipeline, engine, missingPropertyService)
            {
                // The ResultManager needs to have something added to it 
                // in order to allow access to the values.
                // For this test, we can just give it a null reference.
                Results.AddResult(null);
            }

            public void AddValue(string propertyName, object value)
            {
                _values.Add(propertyName, value);
            }

            protected override IAspectPropertyValue<bool> GetValueAsBool(string propertyName)
            {
                return GetValueInternal<bool>(propertyName);
            }

            protected override IAspectPropertyValue<double> GetValueAsDouble(string propertyName)
            {
                return GetValueInternal<double>(propertyName);
            }

            protected override IAspectPropertyValue<int> GetValueAsInteger(string propertyName)
            {
                return GetValueInternal<int>(propertyName);
            }

            protected override IAspectPropertyValue<string> GetValueAsString(string propertyName)
            {
                return GetValueInternal<string>(propertyName);
            }

            protected override IAspectPropertyValue<JavaScript> GetValueAsJavaScript(string propertyName)
            {
                return GetValueInternal<JavaScript>(propertyName);
            }

            public override IAspectPropertyValue<IReadOnlyList<string>> GetValues(string propertyName)
            {
                return GetValueInternal<IReadOnlyList<string>>(propertyName);
            }

            private IAspectPropertyValue<T> GetValueInternal<T>(string propertyName)
            {
                if (_values.ContainsKey(propertyName))
                {
                    return new AspectPropertyValue<T>((T)_values[propertyName]);
                }
                else
                {
                    throw new PropertyMissingException();
                }
            }

            protected override bool PropertyIsAvailable(string propertyName)
            {
                return _testProperties.ContainsKey(propertyName);
            }
        }

        private void SetupElementProperties()
        {
            var properties = new Dictionary<string, IElementPropertyMetaData>();
            foreach (var entry in _testProperties)
            {
                var property = new ElementPropertyMetaData(
                    _engine.Object,
                    entry.Key,
                    entry.Value.Item1,
                    true,
                    "category");
                properties.Add(entry.Key, property);
            }

            var elementProperties = new Dictionary<string, IReadOnlyDictionary<string, IElementPropertyMetaData>>();
            elementProperties.Add(_engine.Object.ElementDataKey, properties);
            _flowData.SetupGet(f => f.Pipeline.ElementAvailableProperties)
                .Returns(elementProperties);
        }

        private TestResults _results;

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
            SetupElementProperties();

            IReadOnlyList<string> expected = new List<string>();
            _results = new TestResults(
                    _logger.Object,
                    _flowData.Object.Pipeline,
                    _engine.Object,
                    _missingPropertyService.Object);
            foreach(var property in _testProperties)
            {
                _results.AddValue(property.Key, property.Value.Item2);
            }
        }

        /// <summary>
        /// Check that a string list is returned from the internal results
        /// instance using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetList()
        { 
            TestAccessingValue(_testPropertyNameList, _testValueList);
        }

        /// <summary>
        /// Check that a string is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetString()
        {
            TestAccessingValue(_testPropertyNameString, _testValueString);
        }

        /// <summary>
        /// Check that a bool is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetBool()
        {
            TestAccessingValue(_testPropertyNameBool, _testValueBool);
        }

        /// <summary>
        /// Check that a int is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetInt()
        {
            TestAccessingValue(_testPropertyNameInteger, _testValueInteger);
        }

        /// <summary>
        /// Check that a double is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetDouble()
        {
            TestAccessingValue(_testPropertyNameDouble, _testValueDouble);
        }

        /// <summary>
        /// Check that a double is returned from the internal results instance
        /// using the correct get method.
        /// </summary>
        [TestMethod]
        public void GetJavaScript()
        {
            TestAccessingValue(_testPropertyNameJavaScript, _testValueJavaScript);
        }

        private void TestAccessingValue(string propertyName, object expectedResult)
        {
            var value = _results[propertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(value.GetType()));
            Assert.AreEqual(expectedResult, ((IAspectPropertyValue)value).Value);

            var dict = _results.AsDictionary();
            Assert.IsTrue(dict.ContainsKey(_testPropertyNameInteger));
            var dictValue = dict[propertyName];
            Assert.IsTrue(typeof(IAspectPropertyValue).IsAssignableFrom(dictValue.GetType()));
            Assert.AreEqual(expectedResult, ((IAspectPropertyValue)dictValue).Value);
        }
    }
}
