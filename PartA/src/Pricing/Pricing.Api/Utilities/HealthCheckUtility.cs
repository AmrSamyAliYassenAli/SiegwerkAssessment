namespace Pricing.Api.Utilities;

/// <summary>
/// Provides a utility class for configuring application health checks.
/// </summary>
internal static class HealthCheckUtility
{
    /// <summary>
    /// Adds standard health checks to the service collection.
    /// It includes a "self" check for the application's liveness and a check for the database context.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> to read settings from.</param>
    /// <returns>The same service collection for method chaining.</returns>
    internal static IServiceCollection AddHealthChecksUtility(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDbContextCheck<PricingDbContext>(); 
    
        return services;
    }
}