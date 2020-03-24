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

using FiftyOne.DeviceDetection.Shared;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.Data.Types;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FiftyOne.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiftyOne.DeviceDetection.Cloud.Data
{
    public class DeviceDataCloud : DeviceDataBase, IDeviceData
    {
        private string[] SEPERATORS = new string[] { FiftyOne.Pipeline.Core.Constants.EVIDENCE_SEPERATOR };
        private Dictionary<string, string> _noValueReasons = null;

        public DeviceDataCloud(ILogger<AspectDataBase> logger,
            IPipeline pipeline,
            IAspectEngine engine,
            IMissingPropertyService missingPropertyService)
            : base(logger, pipeline, engine, missingPropertyService)
        {
        }

        public void SetNoValueReasons(Dictionary<string,string> noValueReasons)
        {
            _noValueReasons = noValueReasons
                .Select(r => new 
                { 
                    KeySegments = r.Key.Split(SEPERATORS, StringSplitOptions.RemoveEmptyEntries),
                    Value = r.Value
                })
                .Where(r =>
                {
                    return r.KeySegments.Length >= 2 && r.KeySegments[0] == Engines.FirstOrDefault().ElementDataKey;
                })
                .ToDictionary(r => r.KeySegments[1], r => r.Value, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Try to get the value from the base dictionary and convert it to the
        /// requested type. If requesting an AspectProperty then convert to the
        /// inner type and set the no value reason if there is no value and a 
        /// no value reason has been provided from the cloud request engine.
        /// </summary>
        /// <typeparam name="T">requested type</typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool TryGetValue<T>(string key, out T value)
        {
            if (typeof(IAspectPropertyValue).IsAssignableFrom(typeof(T)))
            {
                return CloudDataHelpers.TryGetAspectPropertyValue(this, _noValueReasons, key, out value);
            }
            else
            {
                return base.TryGetValue(key, out value);
            }
        }
    }
}
