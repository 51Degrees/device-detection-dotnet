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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


/// @example AspNetCore3.1-UACH-manual/Startup.cs
/// 
/// @include{doc} example-web-integration-client-hints.txt
/// 
/// In this scenario, the standard Pipeline API web integration is not used.
/// This means that several jobs that the API usually takes care of 
/// automatically must be handled manually. 
/// For example, setting the HTTP response headers to request user-agent 
/// client hints.
/// 
/// The source code for this example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/tree/master/FiftyOne.DeviceDetection/Examples/AspNetCore3.1-UACH-manual).
/// 
/// This example can be configured to use the 51Degrees cloud service or a local 
/// data file. If you don't already have data file you can obtain one from the 
/// [device-detection-data](https://github.com/51Degrees/device-detection-data) 
/// GitHub repository.
/// 
/// To use the cloud service you will need to create a **resource key**. 
/// The resource key is used as short-hand to store the particular set of 
/// properties you are interested in as well as any associated license keys 
/// that entitle you to increased request limits and/or paid-for properties.
/// 
/// You can create a resource key using the 51Degrees [Configurator](https://configure.51degrees.com).
/// 
/// Required NuGet Dependencies:
/// - [Microsoft.AspNetCore.App](https://www.nuget.org/packages/Microsoft.AspNetCore.App/)
/// - [FiftyOne.DeviceDetection](https://www.nuget.org/packages/FiftyOne.DeviceDetection/)
/// - [FiftyOne.Pipeline.Web](https://www.nuget.org/packages/FiftyOne.Pipeline.Web/)
///
/// 1. Add Pipeline configuration options to appsettings.json. 
/// (or a separate file if you prefer. Just don't forget to add that 
/// file to your startup.cs)
/// Example on-premise configuration:
/// ```{json}
/// {
///   "PipelineOptions": {
///     "Elements": [
///       {
///         "BuilderName": "DeviceDetectionHashEngineBuilder",
///         "BuildParameters": {
///           "DataFile": "51Degrees-LiteV4.1.hash",
///           "CreateTempDataCopy": false,
///           "AutoUpdate": false,
///           "PerformanceProfile": "LowMemory",
///           "DataFileSystemWatcher": false,
///           "DataUpdateOnStartUp": false
///         }
///       },
///       {
///         "BuilderName": "SetHeadersElementBuilder"
///       }
///     ]
///   }
/// }
/// ```
/// 
/// Example cloud configuration:
/// ```{json}
/// {
///   "PipelineOptions": {
///     "Elements": [
///       {
///         "BuilderName": "CloudRequestEngineBuilder",
///         "BuildParameters": {
///           "ResourceKey": "YourKey" 
///         }
///       },
///       {
///         "BuilderName": "DeviceDetectionCloudEngineBuilder"
///       },
///       {
///         "BuilderName": "SetHeadersElementBuilder"
///       }
///     ]
///   }
/// }
/// ```
/// 
/// 3. Add builders and the Pipeline to the server's services.
/// ```{cs}
/// public class Startup
/// {
///     ...
///     public void ConfigureServices(IServiceCollection services)
///     {
///         ...
///         PipelineOptions options = new PipelineOptions();
///         Configuration.Bind("PipelineOptions", options);
///         services.AddSingleton(new FiftyOnePipelineBuilder()
///            .BuildFromConfiguration(options));
///         ...
/// ```
/// 
/// 5. Inject the `IPipeline` into a controller.
/// ```{cs}
/// public class HomeController : Controller
/// {
///     private IPipeline _pipeline;
///     private IWebRequestEvidenceService _evidenceService;
///     
///     public HomeController(IPipeline pipeline,
///         IWebRequestEvidenceService evidenceService)
///     {
///         _pipeline = pipeline;
///        _evidenceService = evidenceService;
///     }
///     ...
/// }
/// ```
/// 
/// 6. Process the request, set response headers and send the 
/// results to the view.
/// ```{cs}
/// public class HomeController : Controller
/// {
///     ...
///    public IActionResult Index()
///    {
///        using (var flowData = _pipeline.CreateFlowData())
///        {
///            _evidenceService.AddEvidenceFromRequest(flowData, Request);
///            flowData.Process();
///            SetHeaderService.SetHeaders(Response.HttpContext, flowData);
///            return View(flowData);
///        }
///    }
///     ...
/// ```
/// 
/// 7. Display device details in the view.
/// ```{cs}
/// @model FiftyOne.Pipeline.Core.Data.IFlowData
/// ...
/// var deviceData = Model.Get<FiftyOne.DeviceDetection.IDeviceData>();
/// var hardwareVendor = Model.HardwareVendor;
/// ...
/// Hardware Vendor: @(hardwareVendor.HasValue ? hardwareVendor.Value : $"Unknown ({hardwareVendor.NoValueMessage})")<br />
/// ...
/// ```
/// 
/// ## Controller
/// @include Controllers/HomeController.cs
/// 
/// ## View
/// @include Views/Home/Index.cshtml
/// 
/// ## Startup

namespace Client_Hints_Not_Integrated_NetCore_31
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc();

            // Bind the configuration to a pipeline options instance
            PipelineOptions options = new PipelineOptions();
            Configuration.Bind("PipelineOptions", options);
            // Create the 51Degrees pipeline from configuration
            // and add it to the service collection
            services.AddSingleton(new FiftyOnePipelineBuilder()
                 .BuildFromConfiguration(options));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
