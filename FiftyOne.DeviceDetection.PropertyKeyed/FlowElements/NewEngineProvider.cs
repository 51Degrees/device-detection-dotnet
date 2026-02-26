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
using Microsoft.Extensions.Logging;
using System.IO;

namespace FiftyOne.DeviceDetection.PropertyKeyed.FlowElements
{
    /// <summary>
    /// Default implementation of <see cref="IEngineProvider"/> that creates 
    /// a new <see cref="DeviceDetectionHashEngine"/> using an InMemory profile.
    /// </summary>
    public class NewEngineProvider : IEngineProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="loggerFactory">Logger factory to use for building the engine.</param>
        public NewEngineProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc/>
        public DeviceDetectionHashEngine GetEngine(string dataFilePath = null, Stream data = null)
        {
            var builder = new DeviceDetectionHashEngineBuilder(_loggerFactory);
            builder.SetAutoUpdate(false);
            builder.SetDataFileSystemWatcher(false);
            builder.SetPerformanceProfile(FiftyOne.Pipeline.Engines.PerformanceProfiles.MaxPerformance);
            // Default concurrency if needed
            builder.SetConcurrency(1);

            if (data != null)
            {
                return builder.Build(data);
            }

            return builder.Build(dataFilePath, false);
        }
    }
}
