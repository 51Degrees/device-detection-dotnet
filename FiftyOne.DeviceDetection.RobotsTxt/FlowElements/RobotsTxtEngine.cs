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
using FiftyOne.DeviceDetection.PropertyKeyed;
using FiftyOne.DeviceDetection.PropertyKeyed.Services;
using FiftyOne.DeviceDetection.RobotsTxt.Data;
using FiftyOne.DeviceDetection.RobotsTxt.Model;
using FiftyOne.DeviceDetection.RobotsTxt.Services;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Exceptions;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace FiftyOne.DeviceDetection.RobotsTxt.FlowElements
{
    /// <summary>
    /// Aspect engine that generates robots.txt content based on crawler
    /// usage evidence supplied via the pipeline.
    /// <para>
    /// Each evidence key follows the pattern
    /// <c>query.robotstxt.{usage}</c> with a value of <c>allow</c> or
    /// <c>disallow</c>. The engine maps these to the known crawlers in the
    /// data file and emits per-crawler User-Agent / Allow / Disallow
    /// blocks, followed by a catch-all block.
    /// </para>
    /// <para>
    /// When Terms Document Locator (TDL) evidence is provided via the
    /// <see cref="Constants.TdlEvidenceKey"/> key, the catch-all footer
    /// is emitted as an Allow-all block with one <c>TDL:</c> line per
    /// valid absolute URI. Invalid entries are silently dropped per the
    /// IETF-Robots TDL specification. If no valid TDL URIs remain, the
    /// engine falls back to the default Disallow-all catch-all.
    /// </para>
    /// </summary>
    public class RobotsTxtEngine :
        AspectEngineBase<
            IRobotsTxtData,
            IAspectPropertyMetaData>
    {
        /// <summary>
        /// Evidence key values that indicate the crawler usage is allowed.
        /// </summary>
        public static readonly string[] AllowValues = 
            ["allow", "true", "yes", "enabled", "on"];

        /// <summary>
        /// Evidence key values that indicate the crawler usage is not allowed.
        /// </summary>
        public static readonly string[] DisallowValues = 
            ["disallow", "false", "no", "disabled", "off"];

        /// <summary>
        /// Evidence key values that are valid.
        /// </summary>
        public static HashSet<string> ValidValues = 
            new (AllowValues.Concat(DisallowValues));

        /// <summary>
        /// The robots.txt generator service.
        /// </summary>
        private GeneratorService _generatorService;

        /// <summary>
        /// The service that converts a <see cref="DeviceDetectionHashEngine"/>
        /// into a data set that can then be used to create the model.
        /// </summary>
        private IPropertyValueQueryService _queryService;
        
        private readonly List<string> _properties;
        
        private readonly ILogger<RobotsTxtEngine> _logger;

        private readonly ILogger<RobotsTxtData> _loggerData;

        private readonly ITdlSourceResolver _tdlResolver;

        public override string DataSourceTier => _dataSourceTier;
        private string _dataSourceTier;

        public override string ElementDataKey => "robotstxt";

        public override IEvidenceKeyFilter EvidenceKeyFilter => 
            _evidenceKeyFilter;
        private IEvidenceKeyFilter _evidenceKeyFilter;
        private IReadOnlyDictionary<string, UsageModel> _evidenceKeys;

        /// <summary>
        /// Properties available for robots.txt.
        /// </summary>
        public override IList<IAspectPropertyMetaData> Properties => 
            _requiredProperties;

        private IList<IAspectPropertyMetaData> _requiredProperties;

        public RobotsTxtEngine(
            List<string> properties,
            ILoggerFactory loggerFactory,
            ITdlSourceResolver tdlResolver = null) : base(
                loggerFactory.CreateLogger<RobotsTxtEngine>(),
                CreateAspectData)
        {
            _properties = properties;
            _logger = loggerFactory.CreateLogger<RobotsTxtEngine>();
            _loggerData = loggerFactory.CreateLogger<RobotsTxtData>();
            _queryService = new PropertyValueQueryService(
                ["CrawlerUsage"],
                loggerFactory);
            _tdlResolver = tdlResolver;
        }

        private static IRobotsTxtData CreateAspectData(
            IPipeline pipeline, 
            FlowElementBase<IRobotsTxtData, IAspectPropertyMetaData> engine)
        {
            return new RobotsTxtData(
                ((RobotsTxtEngine)engine)._loggerData, 
                pipeline,
                (RobotsTxtEngine)engine);
        }

        /// <summary>
        /// Only processes if there are evidence keys for this engine
        /// available.
        /// </summary>
        /// <param name="data"></param>
        public override void Process(IFlowData data)
        {
            if (_evidenceKeys == null)
            {
                throw new PipelineConfigurationException(
                    "AddPipeline must be called before processing data");
            }
            if (_evidenceKeys.Any(i => data.TryGetEvidence<string>(
                    i.Key,
                    out var _))
                || data.TryGetEvidence<string>(
                    Constants.TdlEvidenceKey,
                    out var _))
            {
                base.Process(data);
            }
        }

        /// <summary>
        /// Get the allowed usages.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="aspectData"></param>
        protected override void ProcessEngine(
            IFlowData data,
            IRobotsTxtData aspectData)
        {
            var allowed = GetAllowedSet(data);
            var tdls = GetTdls(data);
            aspectData.PopulateFrom(GetValues(allowed, tdls));
        }

        /// <summary>
        /// Adds the pipeline to build the data set. Then works out all the
        /// possible evidence keys that can be used by the engine, and the
        /// robots.txt model that will be used with the generator service.
        /// </summary>
        /// <param name="pipeline"></param>
        public override void AddPipeline(IPipeline pipeline)
        {
            base.AddPipeline(pipeline);

            // Get the context pipeline ensuring it will be disposed of at the
            // end of this method. It is not needing to be active once adding
            // the pipeline is complete.
            var engine = pipeline.GetDeviceDetectionHashEngine();
            _dataSourceTier = engine.DataSourceTier;
            using var contextPipeline = _queryService.CreateContext(engine);

            // Build the robots.txt model and service from the query service.
            var usages = GetUsages(engine).ToArray();
            var crawlers = GetCrawlers(
                _queryService.Query(contextPipeline)).ToArray();
            _generatorService = new GeneratorService(new RobotsTxtModel()
            {
                Crawlers = crawlers,
                Usages = usages
            });

            // Get the valid evidence keys and set the evidence key filter.
            // TDL is a standalone evidence channel, not a usage, so include
            // it in the whitelist directly instead of mixing it into the
            // usage-keyed dictionary.
            _evidenceKeys = GetEvidenceKeys(usages).ToFrozenDictionary();
            _evidenceKeyFilter = new EvidenceKeyFilterWhitelist(
                _evidenceKeys.Keys
                    .Append(Constants.TdlEvidenceKey)
                    .ToList());

            // Set the properties.
            AspectPropertyMetaData[] allProperties = [
                new AspectPropertyMetaData(
                    this,
                    nameof(RobotsTxtData.AnnotatedText),
                    typeof(string),
                    "Bots",
                    [engine.DataSourceTier],
                    true),
                new AspectPropertyMetaData(
                    this,
                    nameof(RobotsTxtData.PlainText),
                    typeof(string),
                    "Bots",
                    [engine.DataSourceTier],
                    true),
            ];

            // If all the properties are required then use them, otherwise just
            // select the ones needed.
            if (_properties == null || _properties.Count == 0)
            {
                _requiredProperties = allProperties;
            }
            else
            {
                _requiredProperties = allProperties.Where(i =>
                    _properties.Contains(
                        i.Name,
                        StringComparer.InvariantCultureIgnoreCase)).ToArray();
            }
        }

        private HashSet<string> GetAllowedSet(IFlowData data)
        {
            var allowed = new HashSet<string>();
            var disallow = new HashSet<string>();

            // Where an evidence key exists then modify the allowed and
            // disallowed lists based on the value of the evidence.
            foreach (var key in _evidenceKeys)
            {
                if (data.TryGetEvidence<string>(key.Key, out var value))
                {
                    if (AllowValues.Contains(
                        value,
                        StringComparer.InvariantCultureIgnoreCase))
                    {
                        allowed.Add(key.Value.Name);
                    }
                    if (DisallowValues.Contains(
                        value,
                        StringComparer.InvariantCultureIgnoreCase))
                    {
                        disallow.Add(key.Value.Name);
                    }
                }
            }

            // Remove from the allow list anything that is on the disallow
            // list. This is should never be required but is added to ensure
            // the allow list always reflect the intended result.
            allowed.RemoveWhere(i => disallow.Contains(i));

            return allowed;
        }

        private static readonly char[] TdlSeparators = [',', '|'];

        /// <summary>
        /// Parses TDL URIs from evidence. Accepts a single value or a list
        /// of values joined with ',' or '|'. Each entry is treated as an
        /// absolute URI first; if that fails and an
        /// <see cref="ITdlSourceResolver"/> is registered, it gets a chance
        /// to resolve known short ids to URLs. Anything that ends up
        /// non-URI is silently dropped per the IETF-Robots TDL specification.
        /// </summary>
        private IReadOnlyList<Uri> GetTdls(IFlowData data)
        {
            if (data.TryGetEvidence<string>(
                    Constants.TdlEvidenceKey,
                    out var raw) == false
                || string.IsNullOrWhiteSpace(raw))
            {
                return [];
            }
            return raw
                .Split(
                    TdlSeparators,
                    StringSplitOptions.RemoveEmptyEntries
                        | StringSplitOptions.TrimEntries)
                .Select(ResolveTdlEntry)
                .Where(u => u != null)
                .ToArray();
        }

        private Uri ResolveTdlEntry(string entry)
        {
            if (Uri.TryCreate(entry, UriKind.Absolute, out var u))
            {
                return u;
            }
            if (_tdlResolver?.IsKnown(entry) == true)
            {
                return _tdlResolver.Resolve(entry);
            }
            return null;
        }

        private IEnumerable<KeyValuePair<string, object>>
            GetValues(HashSet<string> allowed, IReadOnlyList<Uri> tdls)
        {
            if (IsRequired(nameof(RobotsTxtData.PlainText)))
            {
                yield return new(
                    nameof(RobotsTxtData.PlainText),
                    GetValue(allowed, tdls, false));
            }
            if (IsRequired(nameof(RobotsTxtData.AnnotatedText)))
            {
                yield return new(
                    nameof(RobotsTxtData.AnnotatedText),
                    GetValue(allowed, tdls, true));
            }
        }

        private bool IsRequired(string propertyName)
        {
            return _requiredProperties.Any(i => propertyName.Equals(
                i.Name, 
                StringComparison.InvariantCultureIgnoreCase));
        }

        private IAspectPropertyValue<string> GetValue(
            HashSet<string> allowed,
            IReadOnlyList<Uri> tdls,
            bool annotations)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                _generatorService.Write(
                    writer,
                    allowed,
                    tdls,
                    annotations,
                    CancellationToken.None);
            }
            return new AspectPropertyValue<string>(sb.ToString());
        }

        private IEnumerable<UsageModel> GetUsages(
            DeviceDetectionHashEngine engine)
        {
            var order = 0;
            foreach(var property in engine.Properties.Where(i => 
                _queryService.IndexedProperties.Contains(
                    i.Name, 
                    StringComparer.InvariantCultureIgnoreCase)))
            {
                foreach(var value in property.GetValues().OrderBy(i => i.Name))
                {
                    yield return new UsageModel()
                    {
                         Name = value.Name,
                         Description = value.Description,
                         Order = order++
                    };
                }
            }
        }

        private IEnumerable<CrawlerModel> GetCrawlers(
            IEnumerable<(IProfileMetaData Profile, IDeviceData Data)> crawlers)
        {
            foreach (var crawler in crawlers)
            {
                var model = CreateCrawler(crawler);
                if (model != null)
                {
                    yield return model;
                }
            }
        }

        private CrawlerModel CreateCrawler(
            (IProfileMetaData Profile, IDeviceData Data) crawler)
        {
            CrawlerModel model = null;
            try
            {
                model = new CrawlerModel()
                {
                    Name = crawler.Data.CrawlerName.Value,
                    ProductTokens = crawler.Data.CrawlerProductTokens.Value
                        .ToArray(),
                    ReferenceUris = BuildReferenceUris(
                        crawler.Data.CrawlerUrl),
                    Usages = crawler.Data.CrawlerUsage.Value.ToArray()
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Could not include crawler for profile '{0}' in " +
                    "robots.txt model",
                    crawler.Profile);
            }
            return model;
        }

        private static Uri[] BuildReferenceUris(
            IAspectPropertyValue<string> url)
        {
            if (url == null
                || url.HasValue == false
                || string.IsNullOrWhiteSpace(url.Value))
            {
                return null;
            }
            return Uri.TryCreate(url.Value, UriKind.Absolute, out var uri)
                ? [uri]
                : null;
        }

        /// <summary>
        /// Enumerates the various crawler usage values that can be provided
        /// as evidence.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, UsageModel>> GetEvidenceKeys(
            UsageModel[] usages)
        {
            foreach (var usage in usages.Where(i => i.Name.Length > 0))
            {
                yield return new (
                    $"query.{ElementDataKey.ToLowerInvariant()}." +
                    $"{usage.Name.ToLowerInvariant()}", 
                    usage);
            }
        }

        protected override void UnmanagedResourcesCleanup()
        {
            // Do nothing.
        }
    }
}
