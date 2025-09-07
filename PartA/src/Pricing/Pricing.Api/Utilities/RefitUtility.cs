namespace Pricing.Api.Utilities;

/// <summary>
/// Provides utility methods for configuring Refit clients for API communication.
/// </summary>
internal static class RefitUtility
{
    /// <summary>
    /// Adds and configures Refit clients for various API endpoints.
    /// It reads the base URL from the application's configuration and registers
    /// clients for Suppliers, Products, Prices, and general Pricing APIs.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddRefitUtility(this IServiceCollection services, IConfiguration configuration)
    {
        var refitBaseUrl = configuration["Refit:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(refitBaseUrl))
        {
            services.AddRefitClient<ISuppliersApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(refitBaseUrl));

            services.AddRefitClient<IProductsApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(refitBaseUrl));

            services.AddRefitClient<IPricesApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(refitBaseUrl));

            services.AddRefitClient<IPricingApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(refitBaseUrl));
        }
        return services;
    }
}