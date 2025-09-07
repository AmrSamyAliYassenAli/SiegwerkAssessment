namespace Pricing.Api.Utilities;

/// <summary>
/// Provides utility methods for configuring Cross-Origin Resource Sharing (CORS) for the application.
/// </summary>
internal static class CorsUtility
{
    /// <summary>
    /// Adds a CORS policy named "AllowAll" to the service collection.
    /// This policy allows requests from any origin, using any HTTP method, and with any header.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddCorsUtility(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        return services;
    }
}