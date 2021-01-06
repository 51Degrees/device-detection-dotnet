using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace performance_tests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalibrateController : ControllerBase
    {
        [HttpGet]
        public string Get(){
            return "empty";
        }
    }
}