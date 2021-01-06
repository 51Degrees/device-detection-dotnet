using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;

namespace performance_tests
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

            services.AddControllers();

            services.AddSingleton<DeviceDetectionHashEngineBuilder>();

            // Call AddFiftyOne to add all the things the Pipeline will need
            // to the services collection and create it based on the supplied
            // configruation.
            services.AddFiftyOne(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Call UseFiftyOne to add the Middleware component that will send any
            // requests to 'process' through the 51Degrees pipeline. 
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/process"), appBuilder =>
            {
                appBuilder.UseFiftyOne();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
