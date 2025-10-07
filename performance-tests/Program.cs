/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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
using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Web.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System;
using System.IO;

namespace performance_tests
{
    public class Program
    {
		static string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
        
		public static void Main(string[] args)
        {
            var configOverrides = CreateConfigOverrides();
            CreateHostBuilder(configOverrides, args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(
            IDictionary<string, string> overrides, string[] args) =>
            Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(builder =>
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json")
                    .AddInMemoryCollection(overrides);
            })
            .UseContentRoot(strWorkPath)
            .UseStartup<Startup>();
        });

        /// <summary>
        /// Typically, something like this will not be necessary.
        /// The device detection API will accept an absolute or relative path for the data file.
        /// However, if a relative path is specified, it will only look in the current working 
        /// directory.
        /// In our examples, we have many different projects and we don't want to have a copy of 
        /// the data file for every single one.
        /// In order to handle this, we dynamically search the project directories for the data 
        /// file location and then override the configured setting with the absolute path if 
        /// necessary.
        /// In a real-world scenario, you can just put the data file in your working directory
        /// or use an absolute path in the configuration file.
        /// </summary>
        private static Dictionary<string, string> CreateConfigOverrides()
        {
            var overrides = new Dictionary<string, string>();

            // Load the configuration file
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            // Bind the configuration to a pipeline options instance
            PipelineOptions options = new PipelineWebIntegrationOptions();
            var section = config.GetRequiredSection("PipelineOptions");
            // Use the 'ErrorOnUnknownConfiguration' option to warn us if we've got any
            // misnamed configuration keys.
            section.Bind(options, (o) => { o.ErrorOnUnknownConfiguration = true; });

            // Get the index of the device detection engine element in the config file so that
            // we can create an override key for it.
            var hashEngineOptions = options.GetElementConfig(nameof(DeviceDetectionHashEngine));
            var hashEngineIndex = options.Elements.IndexOf(hashEngineOptions);
            var dataFileConfigKey = $"PipelineOptions:Elements:{hashEngineIndex}" +
                $":BuildParameters:DataFile";

            var dataFile = options.GetHashDataFile();
            var foundDataFile = false;
            if (string.IsNullOrEmpty(dataFile))
            {
                throw new Exception($"A data file must be specified in the appsettings.json file.");
            }
            // The data file location provided in the configuration may be using an absolute or
            // relative path. If it is relative then search for a matching file using the 
            // ExampleUtils.FindFile function.
            else if (Path.IsPathRooted(dataFile) == false)
            {
                var newPath = FiftyOne.DeviceDetection.TestHelpers.Utils.GetFilePath(dataFile);
                if (newPath != null)
                {
                    // Add an override for the absolute path to the data file.
                    overrides.Add(dataFileConfigKey, newPath.FullName);
                    foundDataFile = true;
                }
            }
            else
            {
                foundDataFile = File.Exists(dataFile);
            }

            if (foundDataFile == false)
            {
                throw new Exception($"Failed to find a device detection data file matching " +
                    $"'{dataFile}'. If using the lite file, then make sure the " +
                    $"device-detection-data submodule has been updated by running " +
                    "`git submodule update --recursive`. Otherwise, ensure that the filename " +
                    "is correct in appsettings.json.");
            }

            return overrides;
        }
    }
}
