/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2023 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.DeviceDetection.Apple;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace FiftyOne.DeviceDetection.Tests.Core
{
    [TestClass]
    public class AppleProfileEngineTests
    {
        /// <summary>
        /// Check that the AppleProfileEngine is working correctly in a simple scenario
        /// </summary>
        [TestMethod]
        public void AppleProfileEngine()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(AppleDecisionTreeTests.JSON_TEST_TREE);
            using (var stream = new MemoryStream(byteArray))
            {
                var dataFile = TestHelpers.Utils.GetFilePath(
                    TestHelpers.Constants.LITE_HASH_DATA_FILE_NAME).FullName;

                var logFactory = LoggerFactory.Create(b => b.AddConsole());
                // Create the Apple profile engine
                var engine = new AppleProfileEngineBuilder(logFactory)
                    .SetAutoUpdate(false)
                    .SetDataUpdateOnStartup(false)
                    .SetDataFileSystemWatcher(false)
                    .Build(stream);
                // Create device detection engine
                var ddEngine = new DeviceDetectionHashEngineBuilder(logFactory)
                    .SetPerformanceProfile(Pipeline.Engines.PerformanceProfiles.LowMemory)
                    .SetAutoUpdate(false)
                    .SetDataUpdateOnStartup(false)
                    .SetDataFileSystemWatcher(false)
                    .Build(dataFile, false);
                // Create pipeline
                var pipeline = new PipelineBuilder(logFactory)
                    .AddFlowElement(engine)
                    .AddFlowElement(ddEngine)
                    .Build();

                using (var flowData = pipeline.CreateFlowData())
                {
                    // Supply evidence
                    flowData.AddEvidence(Constants.EVIDENCE_FAMILY_KEY, "iPad");
                    flowData.AddEvidence(Constants.EVIDENCE_SCREEN_HEIGHT_KEY, 1024);
                    flowData.Process();

                    // Check that the result has been set.
                    var engineData = flowData.GetFromElement(engine);
                    Assert.AreEqual(12345U, engineData.ProfileId);
                    // Ensure that the profile id has also been added to evidence.
                    object result;
                    Assert.IsTrue(flowData.GetEvidence().AsDictionary().TryGetValue(
                        Shared.Constants.EVIDENCE_PROFILE_IDS_KEY, out result),
                        $"Expected an evidence value to be added for '{Shared.Constants.EVIDENCE_PROFILE_IDS_KEY}'");
                    Assert.AreEqual("12345", result);
                }
            }
        }
    }
}
