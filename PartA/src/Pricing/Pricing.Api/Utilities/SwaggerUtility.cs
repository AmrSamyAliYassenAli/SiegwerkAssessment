namespace Pricing.Api.Utilities;

/// <summary>
/// Provides utility methods for configuring Swagger/OpenAPI for the application.
/// </summary>
internal static class SwaggerUtility
{
    /// <summary>
    /// Adds and configures services for generating Swagger documentation.
    /// This includes adding the endpoints API explorer and configuring SwaggerGen to
    /// map the <see cref="DateOnly"/> type for proper display in the documentation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddSwaggerUtility(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
        });

        return services;
    }
}