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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Interop;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Wrappers;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FiftyOne.DeviceDetection.Hash.Tests.FlowElements
{
    [TestClass]
    public class DeviceDataTests : TestsBase
    {
        private static readonly Mock<ILogger<AspectDataBase>> _logger =
            new Mock<ILogger<AspectDataBase>>();

        private static readonly Mock<IMissingPropertyService> _missingPropertyService =
            new Mock<IMissingPropertyService>();

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            InitWrapperAndUserAgents(PerformanceProfiles.HighPerformance);
            var reason = new MissingPropertyResult();
            reason.Description = "test description";
            reason.Reason = MissingPropertyReason.PropertyExcludedFromEngineConfiguration;
            _missingPropertyService.Setup(s => s.GetMissingPropertyReason(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<IAspectEngine>>()))
                .Returns(reason);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            base.Cleanup();
        }

        /// <summary>
        /// Check that a DeviceData is closed by the FlowData which contains it,
        /// and that it in turn closes the native results.
        /// </summary>
        [TestMethod]
        public void DeviceData_ResultsClose()
        {
            using (var flowData = Wrapper.Pipeline.CreateFlowData())
            {
                var data = new DeviceDataHash(
                    _logger.Object,
                    flowData.Pipeline,
                    Wrapper.GetEngine() as DeviceDetectionHashEngine,
                    _missingPropertyService.Object);

                var results = new Mock<IResultsSwigWrapper>();
                data.SetResults(results.Object);
                flowData.GetOrAdd(
                    Wrapper.GetEngine().ElementDataKey,
                    (p) => { return data; });
                flowData.Dispose();
                results.Verify(r => r.Dispose(), Times.Once);
            }
        }

        /// </summary>
        /// <param name="data">instance to call method on</param>
        /// <param name="results">results to be checked</param>
        /// <param name="getMethod">to call on the data</param>
        /// <param name="expectedCount">number of calls to results</param>
        private void verifyCallsToContainsProperty(
            DeviceDataHash data,
            Mock<IResultsSwigWrapper> results,
            MethodInfo getMethod,
            int expectedCount)
        {
            results.Invocations.Clear();

            getMethod.Invoke(data, new object[] { "InvalidProperty" });
            results.Verify(r => r.containsProperty(
                It.IsAny<string>()),
                Times.Exactly(expectedCount));
        }

        /// <summary>
        /// Configure the mock native results to return an empty value for
        /// all getter types.
        /// </summary>
        /// <param name="results">The results to set up</param>
        private void ConfigureNativeGettersNoValue(
            Mock<IResultsSwigWrapper> results)
        {
            var stringValue = new Mock<IValueSwigWrapper<string>>();
            var boolValue = new Mock<IValueSwigWrapper<bool>>();
            var intValue = new Mock<IValueSwigWrapper<int>>();
            var doubleValue = new Mock<IValueSwigWrapper<double>>();
            var vectorValue = new Mock<IValueSwigWrapper<VectorStringSwig>>();

            stringValue.Setup(v => v.hasValue()).Returns(false);
            boolValue.Setup(v => v.hasValue()).Returns(false);
            intValue.Setup(v => v.hasValue()).Returns(false);
            doubleValue.Setup(v => v.hasValue()).Returns(false);
            vectorValue.Setup(v => v.hasValue()).Returns(false);

            results.Setup(r => r.getValueAsString(It.IsAny<string>())).Returns(stringValue.Object);
            results.Setup(r => r.getValueAsBool(It.IsAny<string>())).Returns(boolValue.Object);
            results.Setup(r => r.getValueAsInteger(It.IsAny<string>())).Returns(intValue.Object);
            results.Setup(r => r.getValueAsDouble(It.IsAny<string>())).Returns(doubleValue.Object);
            results.Setup(r => r.getValues(It.IsAny<string>())).Returns(vectorValue.Object);
        }

        /// <summary>
        /// Check that when there is only one native result, the
        /// <see cref="IResultsSwigWrapper.containsProperty(string)"/> method is
        /// not called when calling the protected get methods.
        /// </summary>
        [TestMethod]
        public void DeviceData_ContainsProperty_SingleResult()
        {
            using (var flowData = Wrapper.Pipeline.CreateFlowData())
            {
                var data = new DeviceDataHash(
                    _logger.Object,
                    flowData.Pipeline,
                    (DeviceDetectionHashEngine)Wrapper.GetEngine(),
                    _missingPropertyService.Object);

                var results = new Mock<IResultsSwigWrapper>();
                ConfigureNativeGettersNoValue(results);

                results.Setup(r => r.containsProperty(It.IsAny<string>()))
                    .Returns(false);
                data.SetResults(results.Object);

                foreach (var method in data.GetType().GetMethods())
                {
                    if (method.Name.StartsWith("getValue") &&
                        method.GetParameters().Length == 1 &&
                        method.GetParameters()[0].ParameterType.Equals(typeof(string)))
                    {
                        verifyCallsToContainsProperty(
                            data,
                            results,
                            method,
                            0);
                    }
                }
            }
        }

        /// <summary>
        /// Check that when there are multiple native results, the
        /// <see cref="IResultsSwigWrapper.containsProperty(string)"/> method is
        /// called once when calling the protected get methods.
        /// </summary>
        [TestMethod]
        public void DeviceData_ContainsProperty_MultipleResults()
        {
            using (var flowData = Wrapper.Pipeline.CreateFlowData())
            {
                var data = new DeviceDataHash(
                    _logger.Object,
                    flowData.Pipeline,
                    (DeviceDetectionHashEngine)Wrapper.GetEngine(),
                    _missingPropertyService.Object);

                var results1 = new Mock<IResultsSwigWrapper>();
                ConfigureNativeGettersNoValue(results1);
                results1.Setup(r => r.containsProperty(It.IsAny<string>()))
                    .Returns(false);
                var results2 = new Mock<IResultsSwigWrapper>();
                ConfigureNativeGettersNoValue(results2);
                results2.Setup(r => r.containsProperty(It.IsAny<string>()))
                    .Returns(false);

                data.SetResults(results1.Object);
                data.SetResults(results2.Object);

                foreach (var method in data.GetType().GetMethods())
                {
                    if (method.Name.StartsWith("getValue") &&
                        method.GetParameters().Length == 1 &&
                        method.GetParameters()[0].Equals(typeof(string)))
                    {
                        verifyCallsToContainsProperty(
                            data,
                            results1,
                            method,
                            1);
                        verifyCallsToContainsProperty(
                            data,
                            results2,
                            method,
                            1);
                    }
                }
            }
        }

        /// <summary>
        /// Check that when there is only one native results, the
        /// <see cref="IResultsSwigWrapper.containsProperty(string)"/> method
        /// is called once when calling the public get method, and a
        /// <see cref="PropertyMissingException"/> is thrown when it
        /// does not find the property.
        /// </summary>
        [TestMethod]
        public void DeviceData_MissingProperty_SingleResult()
        {
            using (var flowData = Wrapper.Pipeline.CreateFlowData())
            {
                var data = new DeviceDataHash(
                    _logger.Object,
                    flowData.Pipeline,
                    (DeviceDetectionHashEngine)Wrapper.GetEngine(),
                    _missingPropertyService.Object);

                var results = new Mock<IResultsSwigWrapper>();
                results.Setup(r => r.containsProperty(It.IsAny<string>()))
                    .Returns(false);
                data.SetResults(results.Object);

                try
                {
                    _ = data["Invalid"];
                    Assert.Fail("A missing property exception should have been thrown");
                }
                catch (PropertyMissingException e)
                {
                    Assert.AreEqual("test description", e.Message);
                }
                results.Verify(r => r.containsProperty(It.IsAny<string>()), Times.Once);
            }
        }

        /// <summary>
        /// Check that when there are multiple native results, the
        /// <see cref="IResultsSwigWrapper.containsProperty(string)"/> method is
        /// called oncewhen calling the public get method, and a
        /// <see cref="PropertyMissingException"/> is thrown when none
        /// contain the property.
        /// </summary>
        [TestMethod]
        public void DeviceData_MissingProperty_MultipleResults()
        {
            using (var flowData = Wrapper.Pipeline.CreateFlowData())
            {
                var data = new DeviceDataHash(
                    _logger.Object,
                    flowData.Pipeline,
                    (DeviceDetectionHashEngine)Wrapper.GetEngine(),
                    _missingPropertyService.Object);

                var results1 = new Mock<IResultsSwigWrapper>();
                results1.Setup(r => r.containsProperty(It.IsAny<string>()))
                    .Returns(false);
                var results2 = new Mock<IResultsSwigWrapper>();
                results2.Setup(r => r.containsProperty(It.IsAny<string>()))
                    .Returns(false);

                data.SetResults(results1.Object);
                data.SetResults(results2.Object);

                try
                {
                    _ = data["Invalid"];
                    Assert.Fail("A missing property exception should have been thrown");
                }
                catch (PropertyMissingException e)
                {
                    Assert.AreEqual("test description", e.Message);
                }
                results1.Verify(r => r.containsProperty(It.IsAny<string>()), Times.Once);
                results2.Verify(r => r.containsProperty(It.IsAny<string>()), Times.Once);

            }
        }

        /// <summary>
        /// Call a method and check that an <see cref="InvalidOperationException"/>
        /// is thrown.
        /// </summary>
        /// <param name="instance">Instance to call the method on</param>
        /// <param name="methodCall">Lambda to call on instance</param>
        private void CheckThrowsInvalidOperation(
            DeviceDataHash instance,
            Func<DeviceDataHash, object> methodCall)
        {
            try
            {
                methodCall(instance);
                Assert.Fail("An exception was not thrown by even though the instance " +
                    "was closed.");
            }
            catch (Exception e)
            {
                if (e.GetType().Equals(typeof(InvalidOperationException)) ||
                    (e.InnerException != null && e.InnerException.GetType().Equals(typeof(InvalidOperationException))))
                {
                    // This is the exception we want.
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Check that once a <see cref="DeviceDataHash"/> instance has been
        /// closed, an <see cref="InvalidOperationException"/> is throw when
        /// calling get methods.
        /// </summary>
        [TestMethod]
        public void DeviceData_Closed()
        {
            using (var flowData = Wrapper.Pipeline.CreateFlowData())
            {
                var data = new DeviceDataHash(
                    _logger.Object,
                    flowData.Pipeline,
                    (DeviceDetectionHashEngine)Wrapper.GetEngine(),
                    _missingPropertyService.Object);

                var results = new Mock<IResultsSwigWrapper>();
                data.SetResults(results.Object);

                data.Dispose();

                CheckThrowsInvalidOperation(
                    data,
                    (d) => d["ismobile"]);
                CheckThrowsInvalidOperation(
                    data,
                    (d) => d.IsMobile);
                CheckThrowsInvalidOperation(
                    data,
                    (d) => d.AsDictionary());
            }
        }
    }
}
