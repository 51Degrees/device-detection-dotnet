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

using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

/// @example OnPremise/GettingStarted-Web/Startup.cs
/// 
/// @include{doc} example-getting-started-web.txt
/// 
/// The source code for this example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet/tree/master/Examples/OnPremise/GettingStarted-Web). 
/// 
/// @include{doc} example-require-datafile.txt
/// 
/// Required NuGet Dependencies:
/// - [Microsoft.AspNetCore.App](https://www.nuget.org/packages/Microsoft.AspNetCore.App/)
/// - [FiftyOne.DeviceDetection](https://www.nuget.org/packages/FiftyOne.DeviceDetection/)
/// - [FiftyOne.Pipeline.Web](https://www.nuget.org/packages/FiftyOne.Pipeline.Web/)
/// 
/// ## Overview
/// 
/// The `UseFiftyOne` extension method is used to create a Pipeline instance from the configuration 
/// that is supplied. The `AddFiftyOne` extension method adds a 
/// [middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware) 
/// component that will intercept requests and perform device detection. The results will be 
/// stored in the [HttpContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext.items).
/// The middleware will also handle setting response headers (e.g. Accept-CH for User-Agent 
/// Client Hints) and serving requests for client-side JavaScript and JSON resources.
/// ```{cs}
/// services.AddFiftyOne(Configuration);
/// 
/// app.UseFiftyOne();
/// ```
/// 
/// The results of detection can be accessed by adding a dependency on IFlowDataProvider. This
/// can then be used to interrogate the data.
/// ```{cs}
/// var flowData = _provider.GetFlowData();
/// var deviceData = flowData.Get<IDeviceData>();
/// var hardwareVendor = deviceData.HardwareVendor;
/// ```
/// 
/// Results can also be accessed in client-side code by using the `fod` object. See the 
/// [JavaScriptBuilderElementBuilder](https://51degrees.com/pipeline-dotnet/class_fifty_one_1_1_pipeline_1_1_java_script_builder_1_1_flow_element_1_1_java_script_builder_element_builder.html)
/// for details on available settings such as changing the `fod` name.
/// ```{js}
/// window.onload = function () {
///     fod.complete(function(data) {
///         var hardwareName = data.device.hardwarename;
///         alert(hardwareName.join(", "));
///     }
/// }
/// ```
///
/// ## Configuration
/// @include OnPremise/GettingStarted-Web/appsettings.json
/// 
/// ## Controller
/// @include Controllers/HomeController.cs
/// 
/// ## View
/// @include Views/Home/Index.cshtml
/// 
/// ## Startup
namespace FiftyOne.DeviceDetection.Examples.OnPremise.GettingStartedWeb
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
                // This lambda determines whether user consent for non-essential cookies is needed
                // for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc();

            // Add the hash engine builder to services so that the system can find the builder
            // when it needs to.
            services.AddSingleton<DeviceDetectionHashEngineBuilder>();
            // Configure the services needed by device detection and create the 51Degrees Pipeline
            // instance that will be used to process requests.
            services.AddFiftyOne(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request
        // pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            // This is only needed when running under an ASP.NET test server.
            app.UseMiddleware<UserAgentCorrectionMiddleware>();

            // Add the 51Degrees middleware component.
            // This will pass any incoming requests through the pipeline API, performing device
            // detection. The IFlowData that is used will be stored in the data associated with 
            // the current HTTP session.
            // The IFlowDataAccessor provides an easy way to retrieve this data. See HomeController
            // for an example of this.
            app.UseFiftyOne();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// The ASP.NET TestServer infrastructure causes the User-Agent header to be split into 
        /// multiple values using spaces as a delimiter. These are then re-combined using commas 
        /// as a delimiter. Essentially, replacing spaces with commas in the User-Agent.
        /// See https://github.com/dotnet/aspnetcore/issues/18198
        /// This causes the device detection to fail, so we need to deal with it via a custom 
        /// middleware.
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
                httpContext.Request.Headers["User-Agent"] =
                    new Microsoft.Extensions.Primitives.StringValues(string.Join(" ", val));
                await this.next(httpContext);
            }
        }
    }
}
