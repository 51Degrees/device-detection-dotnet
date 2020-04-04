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
using FiftyOne.Pipeline.Core.FlowElements;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


/// @example DeviceDetectionWebDemoNetCore2_1/Startup.cs
/// This example shows how to integrate the Pipeline API with a 
/// device detection engine into an ASP.NET Core web app.
/// 
/// The source code for this example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/tree/master/FiftyOne.DeviceDetection/Examples/DeviceDetectionWebDemoNetCore2_1). 
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
///         services.AddSingleton<DeviceDetectionHashEngineBuilder>();
///         services.AddFiftyOne(Configuration);
///         ...
/// ```
/// 
/// 4. Configure the server to use the Pipeline which has just been set up.
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
/// 5. Inject the `IFlowDataProvider` into a controller.
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
/// 6. Pass the results contained in the flow data to the view.
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
/// 7. Display device details in the view.
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

namespace DeviceDetectionWebDemoNetCore2_1
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<DeviceDetectionHashEngineBuilder>();
            services.AddFiftyOne(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseFiftyOne();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
