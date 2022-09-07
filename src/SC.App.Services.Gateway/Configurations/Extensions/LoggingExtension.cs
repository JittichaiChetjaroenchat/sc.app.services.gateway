using System;
using Microsoft.Extensions.Hosting;
using SC.App.Services.Gateway.Constant;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace SC.App.Services.Gateway.Configurations.Extensions
{
    public static class LoggingExtension
    {
        public static IHostBuilder ConfigureLog(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, configuration) =>
            {
                var environment = context.HostingEnvironment.EnvironmentName;
                var applicationName = context.Configuration[AppSettings.Applications.Gateway.Name];
                var elasticSearchUri = context.Configuration[AppSettings.ElasticSearch.Uri];

                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchUri))
                    {
                        IndexFormat = $"{applicationName.ToLower()}-{environment.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfShards = 2,
                        NumberOfReplicas = 1
                    })
                    .Enrich.WithProperty("APP_NAME", applicationName)
                    .Enrich.WithProperty("ENV", environment.ToLower())
                    .ReadFrom.Configuration(context.Configuration);
            });

            return hostBuilder;
        }
    }
}