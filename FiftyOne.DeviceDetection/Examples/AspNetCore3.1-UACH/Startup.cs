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
using FiftyOne.Pipeline.Engines.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


/// @example AspNetCore3.1-UACH/Startup.cs
/// 
/// @include{doc} example-web-integration-client-hints.txt
/// 
/// The source code for this example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/tree/master/FiftyOne.DeviceDetection/Examples/AspNetCore3.1-UACH). 
/// 
/// Required NuGet Dependencies:
/// - [Microsoft.AspNetCore.App](https://www.nuget.org/packages/Microsoft.AspNetCore.App/)
/// - [FiftyOne.DeviceDetection](https://www.nuget.org/packages/FiftyOne.DeviceDetection/)
/// - [FiftyOne.Pipeline.Web](https://www.nuget.org/packages/FiftyOne.Pipeline.Web/)
///
/// 1. Add Pipeline configuration options to appsettings.json. 
/// (or a separate file if you prefer. Just don't forget to add that 
/// file to your startup.cs)
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
/// 3. Configure the server to use the Pipeline which has just been set up.
/// ```{cs}
/// public class Startup
/// {
///     ...
///     public void Configure(IApplicationBuilder app, IHostingEnvironment env)
///     {
///         app.UseFiftyOne();
///         ...
/// ```
/// 
/// 4. Inject the `IFlowDataProvider` into a controller.
/// ```{cs}
/// public class HomeController : Controller
/// {
///     private IFlowDataProvider _flow;
///     public HomeController(IFlowDataProvider flow)
///     {
///         _flow = flow;
///     }
///     ...
/// }
/// ```
/// 
/// 5. Pass the results contained in the flow data to the view.
/// ```{cs}
/// public class HomeController : Controller
/// {
///     ...
///     public IActionResult Index()
///     {
///         var data = _flow.GetFlowData().Get<IDeviceData>();
///         return View(data);
///     }
///     ...
/// ```
/// 
/// 6. Display device details in the view.
/// ```{cs}
/// @model FiftyOne.DeviceDetection.IDeviceData
/// ...
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

namespace Client_Hints_NetCore_31
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
            var pipelineConfig = new PipelineOptions();
            Configuration.Bind("PipelineOptions", pipelineConfig);
            var engineConfig = pipelineConfig.Elements.Where(e =>
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
                            $"this example. A paid-for file can be obtained " +
                            $"from http://51degrees.com/pricing.");
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
            app.UseFiftyOne();
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
