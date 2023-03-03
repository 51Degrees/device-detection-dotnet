using FiftyOne.Pipeline.Core.Data;
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Uach
{
    /// <summary>
    /// Data class for <see cref="UachJsConversionElement"/>.
    /// This element writes to the evidence collection, so the data class contains no properties.
    /// </summary>
    public class UachJsConversionData : ElementDataBase
    {
        public UachJsConversionData(ILogger<ElementDataBase> logger, IPipeline pipeline)
            : base(logger, pipeline)
        {
        }
        public UachJsConversionData(ILogger<ElementDataBase> logger, IPipeline pipeline, 
            IDictionary<string, object> dictionary)
            : base(logger, pipeline, dictionary)
        {
        }
    }
}
