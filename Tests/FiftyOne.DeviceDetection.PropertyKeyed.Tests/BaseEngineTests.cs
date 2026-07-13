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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace FiftyOne.DeviceDetection.PropertyKeyed.Tests
{
    public class BaseEngineTests<T> where T : IFlowElement
    {
        protected static ILoggerFactory _loggerFactory;
        protected static CapturingLoggerProvider _capturedLogs;
        protected static T _engine;
        protected static IPipeline _pipeline;
        protected IFlowData _data;

        /// <summary>
        /// Creates the fields and structures used for the tests.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="create">
        /// Function to create the engine used in the tests.
        /// </param>
        protected static void ClassInitializeInternal(
            TestContext context, 
            Func<T> create)
        {
            var ddFile = Utils.GetFilePath(Constants.TAC_HASH_DATA_FILE_NAME).FullName;

            _capturedLogs = new CapturingLoggerProvider();
            _loggerFactory = LoggerFactory.Create(b => b
                .AddProvider(_capturedLogs)
                .SetMinimumLevel(LogLevel.Warning));

            // Build DeviceDetectionHashEngine first
            var hashEngine = new DeviceDetectionHashEngineBuilder(
                _loggerFactory)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .Build(ddFile, false);

            // Create the engine with the function provided by the derived
            // class.
            _engine = create();

            // Create the pipeline with the hash engine and the engine under
            // test.
            _pipeline = new PipelineBuilder(_loggerFactory)
                .AddFlowElement(hashEngine)
                .AddFlowElement(_engine)
                .SetSuppressProcessExceptions(true)
                .SetAutoDisposeElements(true)
                .Build();
        }

        protected static void ClassCleanupInternal()
        {
            _pipeline?.Dispose();
        }

        public virtual void TestInitialize()
        {
            _capturedLogs?.Clear();
            _data = _pipeline.CreateFlowData();
        }

        public virtual void TestCleanup()
        {
            _data?.Dispose();
        }

        /// <summary>
        /// Asserts that nothing was logged at Error (or Critical) level during
        /// the current test. Client-caused validation errors must be surfaced
        /// via <see cref="IFlowData.Errors"/> without an Error-level log, since
        /// an Error log carrying the exception is recorded as an AppInsights
        /// exception (see cloud #201).
        /// </summary>
        protected static void AssertNoErrorLevelLog()
        {
            var offending = _capturedLogs.Entries
                .Where(e => e.Level >= LogLevel.Error)
                .ToList();
            Assert.IsEmpty(offending,
                "Expected no Error-level log, but found: " +
                string.Join("; ", offending.Select(e =>
                    $"{e.Level} {e.Category}: {e.Exception?.Message}")));
        }
    }
}
