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
