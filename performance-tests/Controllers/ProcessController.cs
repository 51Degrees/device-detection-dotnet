using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FiftyOne.Pipeline.Web.Services;
using FiftyOne.DeviceDetection;

namespace performance_tests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        private IFlowDataProvider _flow;

        public ProcessController(IFlowDataProvider flow)
        {
            _flow = flow;
        }

        [HttpGet]
        public string Get(){
            var device = _flow.GetFlowData()?.Get<IDeviceData>();
            if(device != null) {
                return $"{device.IsMobile}";
            }
            return "Hash engine data was null";
        }
    }
}