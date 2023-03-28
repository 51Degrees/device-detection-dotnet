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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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


/// @example UACH-manual/Startup.cs
/// 
/// @include{doc} example-web-integration-client-hints.txt
/// 
/// In this scenario, the standard Pipeline API web integration is not used.
/// This means that several jobs that the API usually takes care of 
/// automatically must be handled manually. 
/// For example, setting the HTTP response headers to request user-agent 
/// client hints.
/// 
/// The source code for this example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/tree/master/FiftyOne.DeviceDetection/Examples/UACH-manual).
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
/// 2. Add builders and the Pipeline to the server's services.
/// ```{cs}
/// public class Startup
/// {
///     ...
///     public void ConfigureServices(IServiceCollection services)
///     {
///         ...
///         services.AddSingleton<DeviceDetectionHashEngineBuilder>();
///         services.AddFiftyOne(Configuration);
///         ...
/// ```
/// 
/// 3. Inject the `IPipeline` into a controller.
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
/// 4. Process the request, set response headers and send the 
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
/// 5. Display device details in the view.
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

namespace Client_Hints_Not_Integrated
{
    public class Startup
    {
        /// <summary>
        /// The ASP.NET TestServer infrastructure seems to cause the 
        /// User-Agent header to be split into multiple values using 
        /// spaces as a delimiter.
        /// These are them combined using commas as a delimiter.
        /// Essentially, replacing spaces with commas in the User-Agent.
        /// This causes the device detection to fail so we deal with
        /// it via a custom middleware.
        /// </summary>
        private class UserAgentCorrectionMiddleware
        {
            private readonly RequestDelegate next;

            public UserAgentCorrectionMiddleware(RequestDelegate next)
            {
                this.next = next;
            }

            public async Task Invoke(HttpContext httpContext)
            {
                var val = httpContext.Request.Headers["User-Agent"];
                httpContext.Request.Headers.Remove("User-Agent");
                httpContext.Request.Headers["User-Agent"] = new Microsoft.Extensions.Primitives.StringValues(string.Join(" ", val));
                await this.next(httpContext);
            }
        }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // This section is not generally necessary. We're just checking
            // if the resource key has been set to a new value so we can
            // warn the user if it has not.
            // --------------------------------------------------------------
            var options = new PipelineOptions();
            var section = Configuration.GetRequiredSection("PipelineOptions");
            // Use the 'ErrorOnUnknownConfiguration' option to warn us if we've got any
            // misnamed configuration keys.
            section.Bind(options, (o) => { o.ErrorOnUnknownConfiguration = true; });

            var engineConfig = options.Elements.Where(e =>
                e.BuilderName.Contains(nameof(DeviceDetectionHashEngine),
                    StringComparison.OrdinalIgnoreCase));
            if (engineConfig.Count() > 0)
            {
                object dataFile = null;
                if (engineConfig.Any(c => c.BuildParameters
                         .TryGetValue("DataFile", out dataFile) == true))
                {
                    var dataFileStr = dataFile.ToString();
                    string dataFilePath =
                        Path.IsPathRooted(dataFileStr) ?
                        dataFileStr :
                        Path.Combine(Environment.CurrentDirectory, dataFileStr);
                    if (File.Exists(dataFilePath) == false)
                    {
                        throw new Exception($"No data file found at " +
                            $"'{dataFilePath}'. This location can be set " +
                            $"using the 'DataFile' entry in the " +
                            $"appsettings.json file. Also, note that the " +
                            $"free 'lite' data file is insufficient to run " +
                            $"this example.");
                    }
                }
            }
            // --------------------------------------------------------------

            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc();

            services.AddSingleton<DeviceDetectionHashEngineBuilder>();
            services.AddFiftyOne(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();

            app.UseMiddleware<UserAgentCorrectionMiddleware>();
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
