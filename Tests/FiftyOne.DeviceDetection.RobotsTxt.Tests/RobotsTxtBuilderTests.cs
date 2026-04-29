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

using FiftyOne.DeviceDetection.RobotsTxt.FlowElements;
using FiftyOne.DeviceDetection.RobotsTxt.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using static FiftyOne.DeviceDetection.TestHelpers.Utils;


namespace FiftyOne.DeviceDetection.RobotsTxt.Tests
{
    /// <summary>
    /// Tests for <see cref="RobotsTxtEngineBuilder"/> and structural
    /// properties of <see cref="RobotsTxtEngine"/> that do not require
    /// a data file.
    /// </summary>
    [TestClass]
    public class RobotsTxtBuilderTests
    {
        /// <summary>
        /// Verifies that <see cref="RobotsTxtEngineBuilder"/> exposes
        /// exactly one parameterless Build() method when inspected via
        /// <see cref="RuntimeReflectionExtensions.GetRuntimeMethods"/>.
        /// <para>
        /// <see cref="FiftyOne.Pipeline.Core.FlowElements.PipelineBuilder"/>
        /// uses GetRuntimeMethods() to locate the Build() method when
        /// constructing a pipeline from configuration. If multiple
        /// parameterless Build() methods are visible (e.g. the public
        /// override and an inherited protected one from
        /// SingleFileAspectEngineBuilderBase) an ambiguity exception is
        /// thrown at startup. Using 'protected override' instead of
        /// 'public new' ensures only one is visible.
        /// </para>
        /// </summary>
        [TestMethod]
        public void RobotsTxtEngineBuilder_SingleParameterlessBuildMethod()
        {
            AssertSingleParameterlessBuild(typeof(RobotsTxtEngineBuilder));
        }

        /// <summary>
        /// Out of the box the builder ships with a GitHub-backed
        /// resolver attached, so a consumer that lists
        /// "RobotsTxtEngineBuilder" in pipeline config gets short-id
        /// resolution without any extra wiring.
        /// </summary>
        [TestMethod]
        public void Constructor_AttachesDefaultResolverThatKnowsMowSocw()
        {
            var builder = new RobotsTxtEngineBuilder(
                NullLoggerFactory.Instance);

            var field = typeof(RobotsTxtEngineBuilder).GetField(
                "_tdlResolver",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field);
            var resolver = field.GetValue(builder) as ITdlSourceResolver;
            Assert.IsNotNull(resolver,
                "constructor should attach a default resolver");
            Assert.IsTrue(resolver.IsKnown("MOW-SOCW"));
        }

        /// <summary>
        /// Passing null to SetTdlSourceResolver disables short-id
        /// resolution — the documented opt-out for air-gapped
        /// deployments that cannot reach GitHub.
        /// </summary>
        [TestMethod]
        public void SetTdlSourceResolver_Null_DisablesResolver()
        {
            var builder = new RobotsTxtEngineBuilder(
                    NullLoggerFactory.Instance)
                .SetTdlSourceResolver(null);

            var field = typeof(RobotsTxtEngineBuilder).GetField(
                "_tdlResolver",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNull(field.GetValue(builder));
        }

        /// <summary>
        /// SetUserAgent is the BuildParameters-friendly entry point
        /// for the same logic; pipeline configuration drives it via
        /// "BuildParameters": { "UserAgent": "..." } in appsettings.json.
        /// </summary>
        [TestMethod]
        public void SetUserAgent_AttachesResolverWithGivenUserAgent()
        {
            var builder = new RobotsTxtEngineBuilder(NullLoggerFactory.Instance)
                .SetUserAgent("TestApp/1.0");

            var field = typeof(RobotsTxtEngineBuilder).GetField(
                "_tdlResolver",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var resolver = field.GetValue(builder) as ITdlSourceResolver;
            Assert.IsNotNull(resolver);
            Assert.IsTrue(resolver.IsKnown("MOW-SOCW"));
        }

        /// <summary>
        /// UseDefaultTdlSourceResolver is fluent — it returns the same
        /// builder instance so callers can chain further configuration.
        /// </summary>
        [TestMethod]
        public void UseDefaultTdlSourceResolver_ReturnsSameBuilderForChaining()
        {
            var builder = new RobotsTxtEngineBuilder(NullLoggerFactory.Instance);

            var returned = builder.UseDefaultTdlSourceResolver("TestApp/1.0");

            Assert.AreSame(builder, returned);
        }

    }
}
