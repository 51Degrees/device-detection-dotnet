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

using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FiftyOne.DeviceDetection.Apple
{
    /// <summary>
    /// The data populated by <see cref="AppleProfileEngine"/>
    /// </summary>
    public class AppleProfileData : AspectDataBase, IAppleProfileData
    {
        private const string profileIdKey = "profileid";

        public uint? ProfileId 
        {
            get { return GetAs<uint?>(profileIdKey); }
            set { this[profileIdKey] = value; }
        }

        public AppleProfileData(
            ILogger<AspectDataBase> logger,
            IPipeline pipeline, 
            IAspectEngine engine)
            : base(logger, pipeline, engine)
        {
        }

        public AppleProfileData(
            ILogger<AspectDataBase> logger, 
            IPipeline pipeline, 
            IAspectEngine engine, 
            IMissingPropertyService missingPropertyService)
            : base(logger, pipeline, engine, missingPropertyService)
        {
        }

        public AppleProfileData(
            ILogger<AspectDataBase> logger, 
            IPipeline pipeline, 
            IAspectEngine engine, 
            IMissingPropertyService missingPropertyService, 
            IDictionary<string, object> dictionary)
            : base(logger, pipeline, engine, missingPropertyService, dictionary)
        {
        }
    }
}
