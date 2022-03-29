/***********************************************************************
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

using FiftyOne.DeviceDetection.Examples;
using FiftyOne.Pipeline.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace FiftyOne.DeviceDetection.Example.Tests
{
    /// <summary>
    /// This test class ensures that the cloud examples execute successfully.
    /// </summary>
    /// <remarks>
    /// Note that these test do not generally ensure the correctness 
    /// of the example, only that the example will run without 
    /// crashing or throwing any unhandled exceptions.
    /// </remarks>
    [TestClass]
    public class TestCloudExamples
    {
        private string ResourceKey;
        private string CloudEndPoint;

        private const string RESOURCE_KEY_ENV_VAR = "SUPER_RESOURCE_KEY";

        /// <summary>
        /// Init method. Specify Resource Key to run examples here or 
        /// set a Resource Key in an environment variable called 'ResourceKey'.
        /// Get cloud endpoint url from environment variables or use default.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            var resourceKey = Environment.GetEnvironmentVariable(RESOURCE_KEY_ENV_VAR);
            ResourceKey = string.IsNullOrWhiteSpace(resourceKey) == false ?
                resourceKey : "!!YOUR_RESOURCE_KEY!!";

            if (string.IsNullOrWhiteSpace(ResourceKey) == true ||
                ResourceKey.StartsWith("!!") == true)
            {
                Assert.Fail($"ResourceKey must be specified in the Init method" +
                    $" or as Environment variable '{RESOURCE_KEY_ENV_VAR}'");
            }

            var cloudEndPoint = Environment.GetEnvironmentVariable("51D_CLOUD_ENDPOINT");
            if(string.IsNullOrWhiteSpace(cloudEndPoint) == false)
            {
                CloudEndPoint = cloudEndPoint;
            } 
            else
            {
                CloudEndPoint = string.Empty;
            }
        }

        /// <summary>
        /// Test the GetAllProperties Example
        /// </summary>
        [TestMethod]
        public void Example_Cloud_GetAllProperties()
        {
            var example = new Examples.Cloud.GetAllProperties.Program.Example();
            example.Run(ResourceKey, CloudEndPoint);
        }

        /// <summary>
        /// Tests the GettingStarted Example
        /// </summary>
        [TestMethod]
        public void Example_Cloud_GettingStarted()
        {
            var example = new Examples.Cloud.GettingStartedConsole.Program.Example();
            var options = CreateConfiguration(example.GetType(), ResourceKey);
            example.Run(options, TextWriter.Null);
        }

        /// <summary>
        /// Test the NativeModelLookup Example
        /// </summary>
        [TestMethod]
        public void Example_Cloud_NativeModelLookup()
        {
            var example = new Examples.Cloud.NativeModelLookup.Program.Example();
            example.Run(ResourceKey, CloudEndPoint);
        }

        /// <summary>
        /// Test the TacLookup Example
        /// </summary>
        [TestMethod]
        public void Example_Cloud_TacLookup()
        {
            var example = new Examples.Cloud.TacLookup.Program.Example();
            example.Run(ResourceKey, CloudEndPoint);
        }

        /// <summary>
        /// Create a new PipelineOptions instance populated with the configuration that is embedded
        /// in the assembly for the specified example type.
        /// The resource key in the configuration will be overridden to use the specified value.
        /// </summary>
        /// <param name="exampleType"></param>
        /// <returns></returns>
        private static PipelineOptions CreateConfiguration(Type exampleType, string resourceKey)
        {
            // Write the contents of the configuration from the example to a temporary file.
            var jsonConfig = ReadEmbeddedConfig(exampleType);
            var configFile = Path.GetTempFileName();
            File.WriteAllText(configFile, jsonConfig);

            // Load the configuration file
            var config = new ConfigurationBuilder()
                .AddJsonFile(configFile)
                .Build();

            // Bind the configuration to a pipeline options instance
            PipelineOptions options = new PipelineOptions();
            config.Bind("PipelineOptions", options);

            // Override the resource key in the config file with the one to use for testing.
            options.SetResourceKey(resourceKey);
            return options;
        }

        /// <summary>
        /// Get the assembly that contains the specified type, check for an embedded 
        /// 'appsettings.json' resource. If present, return the contents.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string ReadEmbeddedConfig(Type exampleType)
        {
            var assembly = exampleType.Assembly;
            var resourceSuffix = "appsettings.json";

            var name = assembly.GetManifestResourceNames().Where(n => n.EndsWith(resourceSuffix));
            if(name.Count() == 0)
            {
                throw new Exception($"Failed to find an embedded resource ending with " +
                    $"'{resourceSuffix}'");
            }
            using (Stream stream = assembly.GetManifestResourceStream(name.First()))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
