using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FiftyOne.Pipeline.Web.Services;
using FiftyOne.DeviceDetection;

namespace AspNetCore31Cloud.Controllers
{
    public class HomeController : Controller
    {
        private IFlowDataProvider _flow;

        public HomeController(IFlowDataProvider flow)
        {
            _flow = flow;
        }

        public IActionResult Index()
        {
            var data = _flow.GetFlowData().Get<IDeviceData>();
            return View(data);
        }
    }
}
