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

using System;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using Microsoft.Extensions.Logging.Abstractions;

namespace FiftyOne.DeviceDetection.PackageConsumption
{
    // Drives the FiftyOne.DeviceDetection.Hash.Engine.OnPremise NuGet package
    // end-to-end: builds the engine, wires it into a pipeline, processes one
    // user-agent. Any failure (DllNotFoundException, BadImageFormatException,
    // engine build error, etc.) surfaces as a non-zero exit code.
    //
    // Invoked from the CI pipeline against published output, so the native
    // DLL resolution path under test is the publish folder, not the NuGet
    // cache — this is what makes it sensitive to bugs like the wrong-platform
    // DLL overwrite that a build-time test would miss.
    public static class Program
    {
        private const string SampleUserAgent =
            "Mozilla/5.0 (Linux; Android 11; Pixel 5) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/90.0.4430.91 Mobile Safari/537.36";

        public static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: PackageConsumption <path-to-hash-data-file>");
                return 2;
            }

            var dataFile = args[0];
            Console.WriteLine("Data file: " + dataFile);

            var logger = NullLoggerFactory.Instance;

            using (var engine = new DeviceDetectionHashEngineBuilder(logger)
                .SetPerformanceProfile(PerformanceProfiles.LowMemory)
                .SetAutoUpdate(false)
                .SetDataFileSystemWatcher(false)
                .Build(dataFile, false))
            using (var pipeline = new PipelineBuilder(logger)
                .AddFlowElement(engine)
                .Build())
            using (var flowData = pipeline.CreateFlowData())
            {
                flowData.AddEvidence("header.user-agent", SampleUserAgent);
                flowData.Process();
            }

            Console.WriteLine("OK: engine loaded, pipeline built, flow processed.");
            return 0;
        }
    }
}
