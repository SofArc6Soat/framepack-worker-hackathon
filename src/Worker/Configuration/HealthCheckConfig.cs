using System.Diagnostics.CodeAnalysis;

namespace Worker.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddHealthCheckConfig(this IServiceCollection services)
        {
            services.AddHealthChecks();

            return services;
        }
    }
}