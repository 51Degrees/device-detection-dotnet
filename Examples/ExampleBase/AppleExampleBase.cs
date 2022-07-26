using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Examples
{
    public class AppleExampleBase : ExampleBase
    {

        protected static bool CheckDataFiles(ILogger logger, DeviceDetectionHashEngine engine, 
            string hashDataFile, string appleDataFile)
        {
            bool runExample = true;
            // Show a useful message if no data file was supplied
            if (string.IsNullOrWhiteSpace(hashDataFile))
            {
                logger.LogError(
                    $"Failed to find a device detection data file. This example requires " +
                    $"a paid-for device detection data file. See our pricing page for " +
                    $"details: https://51degrees.com/pricing");
                runExample = false;
            }
            else
            {
                // Verify that the supplied data file was not lite
                var dataFileInfo = ExampleUtils.GetDataFileInfo(engine);

                if (dataFileInfo.Tier == "Lite")
                {
                    logger.LogError(
                        $"Lite data file supplied. This example requires a paid-for " +
                        $"device detection data file. See our pricing page for " +
                        $"details: https://51degrees.com/pricing");
                    runExample = false;
                }
            }
            // Verify that we have found an Apple data file
            if (string.IsNullOrWhiteSpace(appleDataFile))
            {
                logger.LogError(
                    $"Failed to find an Apple data file. This example requires an " +
                    $"Apple data file. This is available from " +
                    $"https://cloud.51degrees.com/cdn/macintosh.data.json");
                runExample = false;
            }

            return runExample;
        }
    }
}
