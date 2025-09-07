namespace Pricing.Api.Utilities;

/// <summary>
/// Provides utility methods for configuring Swagger/OpenAPI for the application.
/// </summary>
internal static class SwaggerUtility
{
    /// <summary>
    /// Configures and adds Swagger generation services to the DI container.
    /// It sets up API explorers, configures the Swagger document with metadata,
    /// and includes XML comments for better API documentation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> for method chaining.</returns>
    internal static IServiceCollection AddSwaggerUtility(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });

            c.SwaggerDoc("v1", new()
            {
                Title = "Price API",
                Version = "v1",
                Description = "A comprehensive API for managing product prices with advanced logging and error handling"
            });

            // Include XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}