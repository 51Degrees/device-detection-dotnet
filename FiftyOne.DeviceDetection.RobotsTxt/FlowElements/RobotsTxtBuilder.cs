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

using FiftyOne.DeviceDetection.RobotsTxt.Data;
using FiftyOne.DeviceDetection.RobotsTxt.Services;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.RobotsTxt.FlowElements
{
    /// <summary>
    /// Builder for <see cref="RobotsTxtEngine"/>. The constructor
    /// attaches a <see cref="GitHubTdlSourceResolver"/> by default so
    /// short TDL ids in <c>query.robotstxt.tdl</c> evidence are
    /// resolved against the embedded sources out of the box. Pass a
    /// different resolver via <see cref="SetTdlSourceResolver"/>, or
    /// <c>null</c> to disable id resolution entirely (e.g. air-gapped
    /// deployments).
    /// </summary>
    public class RobotsTxtEngineBuilder : PropertyKeyedEngineBuilderBase<
        RobotsTxtEngineBuilder,
        RobotsTxtEngine>
    {
        private const string DefaultUserAgent =
            "FiftyOne.DeviceDetection.RobotsTxt";

        private ITdlSourceResolver _tdlResolver;

        public RobotsTxtEngineBuilder(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            _tdlResolver = GitHubTdlSourceResolver.CreateDefault(
                DefaultUserAgent, loggerFactory);
        }

        public RobotsTxtEngineBuilder SetPerformanceProfile(
            PerformanceProfiles profile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the resolver attached by the constructor. Pass
        /// your own <see cref="ITdlSourceResolver"/> implementation
        /// to use a different source list, a static map, or a
        /// non-GitHub backend; pass <c>null</c> to disable id
        /// resolution and accept only absolute URIs as before.
        /// </summary>
        public RobotsTxtEngineBuilder SetTdlSourceResolver(
            ITdlSourceResolver resolver)
        {
            _tdlResolver = resolver;
            return this;
        }

        /// <summary>
        /// Replaces the constructor's default resolver with another
        /// <see cref="GitHubTdlSourceResolver"/> that uses the given
        /// User-Agent. Use this when you want the same default
        /// behaviour but with an identifying UA (recommended) instead
        /// of the engine's generic one.
        /// </summary>
        /// <param name="userAgent">
        /// Identifying string for the HTTP User-Agent header. GitHub
        /// rejects requests without one.
        /// </param>
        public RobotsTxtEngineBuilder UseDefaultTdlSourceResolver(
            string userAgent)
        {
            return SetTdlSourceResolver(
                GitHubTdlSourceResolver.CreateDefault(
                    userAgent, _loggerFactory));
        }

        /// <summary>
        /// Sets the User-Agent used by the default GitHub-backed
        /// resolver. Equivalent to
        /// <see cref="UseDefaultTdlSourceResolver"/> — exposed under
        /// this name so pipeline configuration can drive it from
        /// <c>"BuildParameters": { "UserAgent": "..." }</c> in
        /// appsettings.json. Has no effect if you have already
        /// installed a non-GitHub resolver via
        /// <see cref="SetTdlSourceResolver"/>.
        /// </summary>
        public RobotsTxtEngineBuilder SetUserAgent(string userAgent)
        {
            return UseDefaultTdlSourceResolver(userAgent);
        }

        /// <inheritdoc/>
        protected override RobotsTxtEngine CreateEngine(
            List<string> properties)
        {
            return new RobotsTxtEngine(
                properties,
                _loggerFactory,
                _tdlResolver);
        }
    }
}