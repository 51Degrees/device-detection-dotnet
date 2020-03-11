using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FiftyOne.DeviceDetection.Cloud.FlowElements;
using FiftyOne.Pipeline.CloudRequestEngine.FlowElements;
using FiftyOne.Pipeline.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore31Cloud
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
            // This section is not really necessary. We're just checking
            // if the resource key has been set to a new value.
            var pipelineConfig = new PipelineOptions();
            Configuration.Bind("PipelineOptions", pipelineConfig);
            var cloudConfig = pipelineConfig.Elements.Where(e => 
                e.BuilderName.Contains(nameof(CloudRequestEngine), 
                    StringComparison.OrdinalIgnoreCase));
            if (cloudConfig.Count() > 0)
            {
                if(cloudConfig.Any(c => c.BuildParameters
                        .TryGetValue("ResourceKey", out var resourceKey) == true && 
                    resourceKey.ToString().StartsWith("!!")))
                {
                    throw new Exception("You need to create a resource key at " +
                        "https://configure.51degrees.com and paste it into the " +
                        "appsettings.json file in this example.");
                }
            }


            services.AddControllersWithViews();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<CloudRequestEngineBuilder>();
            services.AddSingleton<DeviceDetectionCloudEngineBuilder>();
            services.AddFiftyOne(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseFiftyOne();
            app.UseHttpsRedirection();
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
    }
}
