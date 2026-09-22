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
        /// Built on first use by <see cref="CreateUnsuppressedFlowData"/>,
        /// because most test classes never need it and building it loads the
        /// hash data file a second time.
        /// </summary>
        private static IPipeline _unsuppressedPipeline;

        /// <summary>
        /// The arguments <see cref="CreateUnsuppressedFlowData"/> needs to
        /// build its pipeline, kept from class initialisation.
        /// </summary>
        private static string _dataFile;
        private static Func<T> _createEngine;

        private static readonly object _unsuppressedLock = new object();

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

            // Create the engine with the function provided by the derived
            // class.
            _engine = create();

            // Create the pipeline with the hash engine and the engine under
            // test.
            _pipeline = BuildPipeline(
                ddFile,
                _engine,
                suppressProcessExceptions: true);

            // Kept so that CreateUnsuppressedFlowData can build its own
            // pipeline if a test asks for one.
            _dataFile = ddFile;
            _createEngine = create;
            _unsuppressedPipeline = null;
        }

        /// <summary>
        /// Builds a pipeline containing a device detection hash engine and
        /// the engine under test.
        /// </summary>
        /// <param name="dataFile">The hash data file to load.</param>
        /// <param name="engine">The engine under test.</param>
        /// <param name="suppressProcessExceptions">
        /// Whether the pipeline should swallow errors recorded during
        /// processing rather than throwing them.
        /// </param>
        /// <returns>The pipeline, which owns and disposes its elements.</returns>
        private static IPipeline BuildPipeline(
            string dataFile,
            T engine,
            bool suppressProcessExceptions)
        {
            // The hash engine has to be built before the engine under test,
            // which reads its data set from it.
            var hashEngine = new DeviceDetectionHashEngineBuilder(
                _loggerFactory)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .Build(dataFile, false);

            return new PipelineBuilder(_loggerFactory)
                .AddFlowElement(hashEngine)
                .AddFlowElement(engine)
                .SetSuppressProcessExceptions(suppressProcessExceptions)
                .SetAutoDisposeElements(true)
                .Build();
        }

        protected static void ClassCleanupInternal()
        {
            _unsuppressedPipeline?.Dispose();
            _unsuppressedPipeline = null;
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
        /// Creates flow data on a pipeline that does not suppress process
        /// exceptions, so that a test sees the throw a cloud host would see.
        /// </summary>
        /// <remarks>
        /// Suppression hides the defect these tests guard against: any entry
        /// in IFlowData.Errors makes Pipeline.Process throw when exceptions
        /// are not suppressed, so validation of caller input has to stay off
        /// the errors channel entirely.
        ///
        /// The pipeline is built on first use and needs its own elements,
        /// because a keyed engine builds its data set when it is added to a
        /// pipeline and refuses to be added to a second one. Building it
        /// loads the hash data file again, so classes that never call this
        /// do not pay for it.
        /// </remarks>
        protected static IFlowData CreateUnsuppressedFlowData()
        {
            lock (_unsuppressedLock)
            {
                if (_unsuppressedPipeline == null)
                {
                    _unsuppressedPipeline = BuildPipeline(
                        _dataFile,
                        _createEngine(),
                        suppressProcessExceptions: false);
                }
            }
            return _unsuppressedPipeline.CreateFlowData();
        }

        /// <summary>
        /// Asserts that processing recorded no error and exactly one warning,
        /// and that the warning names the value that could not be used.
        /// </summary>
        /// <param name="data">The flow data that was processed.</param>
        /// <param name="rejectedValue">
        /// The evidence value the engine was expected to reject. The caller
        /// is batching lookups, so the warning has to say which value failed
        /// for them to find the offending row.
        /// </param>
        protected static void AssertReportedAsWarning(
            IFlowData data,
            string rejectedValue)
        {
            var errors = data.Errors;
            Assert.IsNull(errors,
                "An unusable evidence value must not reach IFlowData.Errors, " +
                "because that makes the pipeline throw and the caller then " +
                "receives no payload at all. Found: " +
                (errors == null ? "" :
                    string.Join("; ", errors.Select(
                        error => error.ExceptionData.Message))));

            var warnings = data.GetWarnings();
            Assert.HasCount(1, warnings,
                "Expected one warning about the unusable value.");
            StringAssert.Contains(warnings[0].Message, rejectedValue,
                "The warning must name the value that was rejected, so that " +
                "a caller processing a batch can find the offending row.");
        }

        /// <summary>
        /// Asserts that nothing was logged at Error (or Critical) level during
        /// the current test. Client-caused validation problems must be
        /// reported to the caller without an Error-level log, since an Error
        /// log carrying an exception can be surfaced as exception telemetry.
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
