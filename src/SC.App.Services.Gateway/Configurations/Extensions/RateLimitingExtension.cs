using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SC.App.Services.Gateway.Constant;

namespace SC.App.Services.Gateway.Configurations.Extensions
{
    public static class RateLimitingExtension
    {
        public static void AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            // In-Memory caching
            services.AddMemoryCache();

            // Configuration
            var config = configuration.GetSection(AppSettings.RateLimiting.Default);
            services.Configure<IpRateLimitOptions>(config);

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            services.AddMemoryCache();
        }

        public static void UseRateLimiting(this IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
        }
    }
}