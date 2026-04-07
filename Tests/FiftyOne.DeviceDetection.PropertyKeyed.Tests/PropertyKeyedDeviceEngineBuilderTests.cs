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

using System.Linq;
using System.Reflection;
using FiftyOne.DeviceDetection.PropertyKeyed.FlowElements;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests;

/// <summary>
/// Tests for builder configuration validation.
/// </summary>
[TestClass]
public class PropertyKeyedDeviceEngineBuilderTests
{
    private ILoggerFactory _loggerFactory;

    private static TEngine InvokeBuild<TEngine>(object builder)
    {
        var method = builder.GetType()
            .GetRuntimeMethods()
            .Single(m => m.Name == "Build" && m.GetParameters().Length == 0);
        try
        {
            return (TEngine)method.Invoke(builder, null);
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo
                .Capture(ex.InnerException).Throw();
            throw;
        }
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _loggerFactory = LoggerFactory.Create(b => { });
    }

    /// <summary>
    /// Building without key property should throw.
    /// </summary>
    [TestMethod]
    public void Build_WithoutKeyProperty_Throws()
    {
        var builder = new PropertyKeyedDeviceEngineBuilder(
            _loggerFactory,
            new Mock<IDataUpdateService>().Object);

        builder.SetProperty("SomeProperty");
        Assert.ThrowsExactly<PipelineConfigurationException>(() =>
            InvokeBuild<PropertyKeyedDeviceBaseEngine>(builder));
    }

    /// <summary>
    /// TacEngineBuilder should set appropriate defaults.
    /// </summary>
    [TestMethod]
    public void TacEngineBuilder_SetsDefaults()
    {
        var builder = new TacEngineBuilder(
            _loggerFactory,
            new Mock<IDataUpdateService>().Object);

        var engine = InvokeBuild<TacEngine>(builder);

        Assert.AreEqual("tac-profiles", engine.ElementDataKey);
    }

    /// <summary>
    /// NativeEngineBuilder should set appropriate defaults.
    /// </summary>
    [TestMethod]
    public void NativeEngineBuilder_SetsDefaults()
    {
        var builder = new NativeEngineBuilder(
            _loggerFactory,
            new Mock<IDataUpdateService>().Object);

        var engine = InvokeBuild<NativeEngine>(builder);

        Assert.AreEqual("native-profiles", engine.ElementDataKey);
    }
}