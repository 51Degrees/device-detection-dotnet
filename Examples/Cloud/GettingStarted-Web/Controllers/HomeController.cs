/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, 5 Charlotte Close,
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

using Microsoft.AspNetCore.Mvc;
using FiftyOne.Pipeline.Web.Services;
using FiftyOne.DeviceDetection.Examples.Cloud.GettingStartedWeb.Model;
using Microsoft.Extensions.Logging;

namespace FiftyOne.DeviceDetection.Examples.Cloud.GettingStartedWeb.Controllers
{
    public class HomeController : Controller
    {
        private static bool _checkedDataFile = false;

        private IFlowDataProvider _provider;
        private ILogger<HomeController> _logger;

        // The controller has a dependency on IFlowDataProvider. This is used to access the 
        // IFlowData that contains the device detection results for the current HTTP request.
        public HomeController(IFlowDataProvider provider, ILogger<HomeController> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Log warnings if the data file is too old or the 'Lite' file is being used.
            if(_checkedDataFile == false)
            {
                ExampleUtils.CheckDataFile(_provider.GetFlowData().Pipeline, _logger);
                _checkedDataFile = true;
            }
            // Use the provider to get the flow data. This contains the results of device
            // detection that has been performed by the pipeline.
            return View(new IndexModel(_provider.GetFlowData(), Response.Headers));
        }
    }
}
