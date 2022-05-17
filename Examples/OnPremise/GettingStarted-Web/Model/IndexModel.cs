/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, Davidson House,
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
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Engines.Data;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace FiftyOne.DeviceDetection.Examples.OnPremise.GettingStartedWeb.Model
{
    public class IndexModel
    {
        public string HardwareVendor { get; private set; }
        public string HardwareName { get; private set; }
        public string DeviceType { get; private set; }
        public string PlatformVendor { get; private set; }
        public string PlatformName { get; private set; }
        public string PlatformVersion { get; private set; }
        public string BrowserVendor { get; private set; }
        public string BrowserName { get; private set; }
        public string BrowserVersion { get; private set; }
        public string ScreenWidth { get; private set; }
        public string ScreenHeight { get; private set; }

        public IFlowData FlowData { get; private set; }

        public DeviceDetectionHashEngine Engine { get; private set; }

        public IAspectEngineDataFile DataFile { get; private set; }

        public List<EvidenceModel> Evidence { get; private set; }

        public IHeaderDictionary ResponseHeaders { get; private set; }

        public IndexModel(IFlowData flowData, IHeaderDictionary responseHeaders)
        {
            FlowData = flowData;
            ResponseHeaders = responseHeaders;

            // Get the engine that performed the detection.
            Engine = FlowData.Pipeline.GetElement<DeviceDetectionHashEngine>();
            // Get meta-data about the data file.
            DataFile = Engine.DataFiles[0];
            // Get the evidence that was used when performing device detection
            Evidence = FlowData.GetEvidence().AsDictionary()
                .Select(e => new EvidenceModel()
                {
                    Key = e.Key,
                    Value = e.Value.ToString(),
                    Used = Engine.EvidenceKeyFilter.Include(e.Key)
                })
                .ToList();

            // Get the results of device detection.
            var deviceData = FlowData.Get<IDeviceData>();
            // Use helper functions to get a human-readable string representation of various
            // property values. These helpers handle situations such as the property missing
            // due to using a lite data file or the property not having a value because device
            // detection didn't find a match.
            HardwareVendor = deviceData.TryGetValue(d => d.HardwareVendor.GetHumanReadable());
            HardwareName = deviceData.TryGetValue(d => d.HardwareName.GetHumanReadable());
            DeviceType = deviceData.TryGetValue(d => d.DeviceType.GetHumanReadable());
            PlatformVendor = deviceData.TryGetValue(d => d.PlatformVendor.GetHumanReadable());
            PlatformName = deviceData.TryGetValue(d => d.PlatformName.GetHumanReadable());
            PlatformVersion = deviceData.TryGetValue(d => d.PlatformVersion.GetHumanReadable());
            BrowserVendor = deviceData.TryGetValue(d => d.BrowserVendor.GetHumanReadable());
            BrowserName = deviceData.TryGetValue(d => d.BrowserName.GetHumanReadable());
            BrowserVersion = deviceData.TryGetValue(d => d.BrowserVersion.GetHumanReadable());
            ScreenWidth = deviceData.TryGetValue(d => d.ScreenPixelsWidth.GetHumanReadable());
            ScreenHeight = deviceData.TryGetValue(d => d.ScreenPixelsHeight.GetHumanReadable());
        }
    }
}
