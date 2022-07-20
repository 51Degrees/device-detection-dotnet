using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.Data;
using FiftyOne.Pipeline.Engines.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
