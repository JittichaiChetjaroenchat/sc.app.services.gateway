using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using SC.App.Services.Gateway.Configurations.Extensions;
using SC.App.Services.Gateway.Constant;
using SC.App.Services.Gateway.Enums;
using SC.App.Services.Gateway.Lib.Extensions;

namespace SC.App.Services.Gateway
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
            // Add CORS
            services.AddCors(Configuration);

            // Add rate limiting
            services.AddRateLimiting(Configuration);

            // Add controllers
            var mvcBuilder = services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            });

            // Add formatting
            mvcBuilder.AddJsonFormat();
            mvcBuilder.AddXmlFormat();

            // Add health checks
            services.AddHealthChecks(Configuration);

            // Add ocelot and consul
            services
                .AddOcelot()
                .AddConsul();

            // Add document
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddDocument(Configuration[AppSettings.Applications.Gateway.Name]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment(EnumEnvironment.Dev.GetDescription()))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors(Configuration);

            // Use rate limiting
            app.UseRateLimiting();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Use health checks
                endpoints.UseHealthChecks(Configuration);
            });

            // Use document
            app.UseDocument();

            // Use web socket
            app.UseWebSockets();

            // Use ocelot
            await app.UseOcelot();
        }
    }
}