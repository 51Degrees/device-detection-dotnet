/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2019 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
 * Caversham, Reading, Berkshire, United Kingdom RG4 7BY.
 *
 * This Original Work is licensed under the European Union Public Licence (EUPL) 
 * v.1.2 and is subject to its terms as set out below.
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
using FiftyOne.DeviceDetection.Pattern.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.TestHelpers;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using System;
using System.Collections.Generic;
using System.IO;

namespace FiftyOne.DeviceDetection.Pattern.Tests
{
    public class WrapperPattern : IWrapper
    {
        protected static readonly TestLoggerFactory _logger = new TestLoggerFactory();

        public IPipeline Pipeline { get; private set; }

        public DeviceDetectionPatternEngine Engine { get; private set; }

        public WrapperPattern(FileInfo dataFile, PerformanceProfiles profile)
        {
            Engine = new DeviceDetectionPatternEngineBuilder(_logger, null)
                .SetPerformanceProfile(profile)
                .SetUpdateMatchedUserAgent(true)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .Build(dataFile.FullName, false);
            Pipeline = new PipelineBuilder(_logger)
                .AddFlowElement(Engine)
                .Build();
        }
        public IEnumerable<IFiftyOneAspectPropertyMetaData> Properties => Engine.Properties;

        public IEnumerable<IValueMetaData> Values => Engine.Values;

        public IEnumerable<IProfileMetaData> Profiles => Engine.Profiles;

        public IEnumerable<IComponentMetaData> Components => Engine.Components;

        public void Dispose()
        {
            Pipeline.Dispose();
            Pipeline = null;
            Engine.Dispose();
            Engine = null;
        }

        public IFiftyOneAspectEngine GetEngine()
        {
            return Engine;
        }
    }
}
