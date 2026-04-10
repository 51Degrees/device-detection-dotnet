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

using FiftyOne.DeviceDetection.PropertyKeyed.FlowElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using static FiftyOne.DeviceDetection.TestHelpers.Utils;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    /// <summary>
    /// Verifies structural properties of property-keyed engine builders
    /// that do not require a data file.
    /// </summary>
    [TestClass]
    public class PropertyKeyedBuilderTests
    {
        /// <summary>
        /// Verifies that <see cref="TacEngineBuilder"/> exposes exactly one
        /// parameterless Build() method when inspected via
        /// <see cref="RuntimeReflectionExtensions.GetRuntimeMethods"/>.
        /// </summary>
        [TestMethod]
        public void TacEngineBuilder_SingleParameterlessBuildMethod()
        {
            AssertSingleParameterlessBuild(typeof(TacEngineBuilder));
        }

        /// <summary>
        /// Verifies that <see cref="NativeEngineBuilder"/> exposes exactly one
        /// parameterless Build() method when inspected via
        /// <see cref="RuntimeReflectionExtensions.GetRuntimeMethods"/>.
        /// </summary>
        [TestMethod]
        public void NativeEngineBuilder_SingleParameterlessBuildMethod()
        {
            AssertSingleParameterlessBuild(typeof(NativeEngineBuilder));
        }
    }
}
