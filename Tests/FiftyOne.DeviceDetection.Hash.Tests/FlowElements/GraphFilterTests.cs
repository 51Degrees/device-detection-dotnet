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

using FiftyOne.Common.TestHelpers;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Data;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Shared.Data;
using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Caching;
using FiftyOne.Pipeline.Engines.Configuration;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Constants = FiftyOne.DeviceDetection.TestHelpers.Constants;

namespace FiftyOne.DeviceDetection.Hash.Tests.FlowElements
{
    /// <summary>
    /// An engine that forwards a fixed set of required property indexes to
    /// the filtered ProcessEngine, standing in for a caller that knows which
    /// properties it will read.
    /// </summary>
    internal class FilteredHashEngine : DeviceDetectionHashEngine
    {
        /// <summary>
        /// Indexes passed to the filtered overload on every request. Null
        /// walks every graph, an empty array walks none.
        /// </summary>
        public int[] Indexes { get; set; }

        internal FilteredHashEngine(
            ILoggerFactory loggerFactory,
            Func<IPipeline, FlowElementBase<IDeviceDataHash, IFiftyOneAspectPropertyMetaData>, IDeviceDataHash> deviceDataFactory,
            string tempDataFilePath)
            : base(loggerFactory, deviceDataFactory, tempDataFilePath)
        {
        }

        protected override void ProcessEngine(IFlowData data, IDeviceDataHash deviceData)
        {
            ProcessEngine(data, deviceData, Indexes);
        }
    }

    /// <summary>
    /// Builder for <see cref="FilteredHashEngine"/>, identical to the
    /// standard builder except for the engine type it creates.
    /// </summary>
    internal class FilteredHashEngineBuilder : DeviceDetectionHashEngineBuilderBase<FilteredHashEngine>
    {
        public FilteredHashEngineBuilder(ILoggerFactory loggerFactory)
            : base(loggerFactory, null)
        {
        }

        protected override FilteredHashEngine CreateEngine(
            ILoggerFactory loggerFactory,
            Func<IPipeline, FlowElementBase<IDeviceDataHash, IFiftyOneAspectPropertyMetaData>, IDeviceDataHash> deviceDataFactory,
            string tempDataFilePath)
        {
            return new FilteredHashEngine(loggerFactory, deviceDataFactory, tempDataFilePath);
        }
    }

    /// <summary>
    /// Tests for the filtered ProcessEngine overload and the
    /// RequiredPropertyIndexes map that feeds it.
    /// </summary>
    [TestClass]
    [TestCategory("Core")]
    [TestCategory("GraphFilter")]
    public class GraphFilterTests
    {
        private static readonly TestLoggerFactory _logger = new TestLoggerFactory();

        // User-Agent string of an iPhone mobile device.
        private const string UserAgent =
            "Mozilla/5.0 (iPhone; CPU iPhone OS 7_1 like Mac OS X) " +
            "AppleWebKit/537.51.2 (KHTML, like Gecko) Version/7.0 Mobile/11D167 " +
            "Safari/9537.53";

        private FilteredHashEngine _engine;
        private IPipeline _pipeline;

        [TestInitialize]
        public void Init()
        {
            var dataFile = Utils.GetFilePath(Constants.LITE_HASH_DATA_FILE_NAME);
            _engine = new FilteredHashEngineBuilder(_logger)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .SetProperties(new List<string> { "IsMobile", "BrowserName", "PlatformName" })
                .Build(dataFile.FullName, false);
            _pipeline = new PipelineBuilder(_logger).AddFlowElement(_engine).Build();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _pipeline?.Dispose();
            _engine?.Dispose();
        }

        private IDeviceDataHash Detect()
        {
            return Detect(_pipeline);
        }

        private static IDeviceDataHash Detect(IPipeline pipeline)
        {
            var data = pipeline.CreateFlowData();
            data.AddEvidence("header.user-agent", UserAgent);
            data.Process();
            return data.Get<IDeviceDataHash>();
        }

        /// <summary>
        /// Builds a second engine and sets a results cache on it directly,
        /// which is the only way to give this engine a cache because the
        /// builder refuses one, and runs the test against it. Cache hits are
        /// flagged on the results.
        /// </summary>
        private static void WithEngineCache(
            Action<FilteredHashEngine, IPipeline> test,
            LazyLoadingConfiguration lazyLoading = null)
        {
            var dataFile = Utils.GetFilePath(Constants.LITE_HASH_DATA_FILE_NAME);
            using (var engine = new FilteredHashEngineBuilder(_logger)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .SetProperties(new List<string> { "IsMobile", "BrowserName", "PlatformName" })
                .Build(dataFile.FullName, false))
            {
                engine.SetCache(new DefaultFlowCache(new CacheConfiguration() { Size = 10 }));
                engine.SetCacheHitOrMiss(true);
                if (lazyLoading != null)
                {
                    engine.SetLazyLoading(lazyLoading);
                }
                using (var pipeline = new PipelineBuilder(_logger).AddFlowElement(engine).Build())
                {
                    test(engine, pipeline);
                }
            }
        }

        /// <summary>
        /// Processes the User-Agent and checks the engine refused the
        /// filtered request because a cache is set.
        /// </summary>
        private static void AssertRefused(IPipeline pipeline)
        {
            var data = pipeline.CreateFlowData();
            data.AddEvidence("header.user-agent", UserAgent);
            // The pipeline collects element exceptions and rethrows them
            // together, so look inside the aggregate for the refusal.
            var aggregate = Assert.ThrowsExactly<AggregateException>(() => data.Process());
            Assert.IsTrue(IsRefusal(aggregate), "Expected the refusal, got: " + aggregate);
        }

        /// <summary>
        /// True if the exception, or one it wraps, is the engine refusing to
        /// filter because a cache is set. Matched on the message so another
        /// InvalidOperationException, such as ObjectDisposedException, does
        /// not count.
        /// </summary>
        private static bool IsRefusal(Exception e)
        {
            if (e == null)
            {
                return false;
            }
            if (e is InvalidOperationException && e.Message ==
                global::FiftyOne.DeviceDetection.Hash.Engine.OnPremise.Messages.ExceptionGraphFilterWithCache)
            {
                return true;
            }
            if (e is AggregateException aggregate &&
                aggregate.InnerExceptions.Any(IsRefusal))
            {
                return true;
            }
            return IsRefusal(e.InnerException);
        }

        [TestMethod]
        public void GraphFilter_RequiredPropertyIndexes_OnePerBuiltProperty()
        {
            var map = _engine.RequiredPropertyIndexes;
            Assert.HasCount(3, map);
            Assert.IsTrue(map.ContainsKey("IsMobile"));
            Assert.IsTrue(map.ContainsKey("ismobile"), "Lookup must ignore case.");
            Assert.HasCount(map.Count, map.Values.Distinct().ToList(), "Indexes must be distinct.");
            Assert.IsTrue(map.Values.All(i => i >= 0 && i < map.Count));
        }

        [TestMethod]
        public void GraphFilter_Null_GivesEveryValue()
        {
            _engine.Indexes = null;
            var device = Detect();
            Assert.IsTrue(device.IsMobile.HasValue);
            Assert.IsTrue(device.BrowserName.HasValue);
            Assert.IsTrue(device.PlatformName.HasValue);
        }

        [TestMethod]
        public void GraphFilter_OneProperty_GivesOnlyItsComponent()
        {
            _engine.Indexes = new[] { _engine.RequiredPropertyIndexes["IsMobile"] };
            var device = Detect();
            Assert.IsTrue(device.IsMobile.HasValue);
            Assert.IsFalse(device.BrowserName.HasValue);
            Assert.IsFalse(device.PlatformName.HasValue);
            Assert.IsFalse(string.IsNullOrEmpty(device.BrowserName.NoValueMessage),
                "A skipped property must explain why it has no value.");
        }

        [TestMethod]
        public void GraphFilter_Empty_GivesNoValue()
        {
            _engine.Indexes = new int[0];
            var device = Detect();
            Assert.IsFalse(device.IsMobile.HasValue);
            Assert.IsFalse(device.BrowserName.HasValue);
            Assert.IsFalse(device.PlatformName.HasValue);
        }

        [TestMethod]
        public void GraphFilter_OneProperty_DeviceIdHasZeroForSkippedComponents()
        {
            _engine.Indexes = null;
            var all = Detect().DeviceId.Value.Split('-');
            _engine.Indexes = new[] { _engine.RequiredPropertyIndexes["IsMobile"] };
            var some = Detect().DeviceId.Value.Split('-');
            Assert.HasCount(all.Length, some);
            for (int i = 0; i < all.Length; i++)
            {
                Assert.IsTrue(some[i] == all[i] || some[i] == "0",
                    "Component " + i + " must keep its profile id or read 0.");
            }
            Assert.AreEqual(1, some.Count(id => id != "0"),
                "Only the component IsMobile belongs to should have a profile.");
        }

        [TestMethod]
        public void GraphFilter_Unfiltered_MatchesFiltered()
        {
            _engine.Indexes = null;
            var all = Detect();
            _engine.Indexes = new[] { _engine.RequiredPropertyIndexes["BrowserName"] };
            var some = Detect();
            Assert.AreEqual(all.BrowserName.Value, some.BrowserName.Value);
        }

        [TestMethod]
        public void GraphFilter_ThrowsWhenCacheSet()
        {
            _engine.SetCache(new DefaultFlowCache(new CacheConfiguration() { Size = 10 }));
            _engine.Indexes = new[] { _engine.RequiredPropertyIndexes["IsMobile"] };
            AssertRefused(_pipeline);
        }

        [TestMethod]
        public void GraphFilter_Unfiltered_StillWorksWhenCacheSet()
        {
            _engine.SetCache(new DefaultFlowCache(new CacheConfiguration() { Size = 10 }));
            _engine.Indexes = null;
            var device = Detect();
            Assert.IsTrue(device.IsMobile.HasValue);
        }

        [TestMethod]
        public void GraphFilter_BuilderRefusesCache()
        {
            // The builder cannot give this engine a cache, so SetCache on the
            // engine is the only route and the only one the guard must cover.
            Assert.ThrowsExactly<NotSupportedException>(() =>
                new FilteredHashEngineBuilder(_logger).SetCache(
                    new CacheConfiguration() { Size = 10 }));
        }

        [TestMethod]
        public void GraphFilter_RefusalLeavesNothingInCache()
        {
            WithEngineCache((engine, pipeline) =>
            {
                engine.Indexes = new[] { engine.RequiredPropertyIndexes["IsMobile"] };
                AssertRefused(pipeline);
                // The same evidence unfiltered must be a miss with every
                // value, not a filtered result left behind by the refusal.
                engine.Indexes = null;
                var device = Detect(pipeline);
                Assert.IsFalse(device.CacheHit,
                    "The refused request must not have stored a result.");
                Assert.IsTrue(device.IsMobile.HasValue);
                Assert.IsTrue(device.BrowserName.HasValue);
                Assert.IsTrue(device.PlatformName.HasValue);
            });
        }

        [TestMethod]
        public void GraphFilter_Unfiltered_ServedFromCache()
        {
            WithEngineCache((engine, pipeline) =>
            {
                engine.Indexes = null;
                var first = Detect(pipeline);
                // The cache hands back the same instance and flags it, so
                // read the first answer before the second request.
                Assert.IsFalse(first.CacheHit);
                var isMobile = first.IsMobile.Value;
                var browser = first.BrowserName.Value;
                var platform = first.PlatformName.Value;
                var second = Detect(pipeline);
                Assert.IsTrue(second.CacheHit,
                    "The second request should be served from the cache.");
                Assert.AreEqual(isMobile, second.IsMobile.Value);
                Assert.AreEqual(browser, second.BrowserName.Value);
                Assert.AreEqual(platform, second.PlatformName.Value);
            });
        }

        [TestMethod]
        public void GraphFilter_LazyLoading_RefusalSurfacesOnReadAndIsCached()
        {
            WithEngineCache((engine, pipeline) =>
            {
                engine.Indexes = new[] { engine.RequiredPropertyIndexes["IsMobile"] };
                // Processing runs on a task, so the refusal is raised when a
                // value is read rather than by Process.
                var device = Detect(pipeline);
                var error = Assert.Throws<Exception>(() => device.IsMobile.HasValue);
                Assert.IsTrue(IsRefusal(error), "Expected the refusal, got: " + error);
                // The pipeline cached the result before the task failed, so
                // the same evidence fails the same way even unfiltered until
                // the entry is evicted. The documentation says so.
                engine.Indexes = null;
                var after = Detect(pipeline);
                Assert.IsTrue(after.CacheHit);
                var cached = Assert.Throws<Exception>(() => after.IsMobile.HasValue);
                Assert.IsTrue(IsRefusal(cached), "Expected the refusal, got: " + cached);
            },
            // A long wait so the task always finishes first. A wait that times
            // out without a cancellation token fails inside the pipeline with
            // "Nullable object must have a value" rather than a timeout.
            new LazyLoadingConfiguration(60000));
        }
    }
}
