namespace Pricing.Api.Utilities;

/// <summary>
/// Provides utility methods for configuring dependency injection for the pricing application.
/// </summary>
internal static class InjectionUtility
{
    /// <summary>
    /// Adds all application services, repositories, and providers to the service collection.
    /// This is the main entry point for configuring dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddInjections(this IServiceCollection services)
        => services
        .AddRepositories()
        .AddProviders()
        .AddServices();

    /// <summary>
    /// Registers all repository interfaces and their concrete implementations as scoped services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPriceListRepository, PriceListRepository>();

        return services;
    }

    /// <summary>
    /// Registers all provider interfaces and their concrete implementations.
    /// The rate provider is registered as a singleton because it is stateless.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    private static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.AddSingleton<IRateProvider, StaticRateProvider>();

        return services;
    }

    /// <summary>
    /// Registers all application services and API services as scoped services.
    /// It also demonstrates how to register a decorator for a service (caching).
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPriceListService, PriceListService>();

        services.AddScoped<BestPriceService>();

        services.AddScoped<SupplierApiService>();
        services.AddScoped<ProductApiService>();
        services.AddScoped<PriceListApiService>();

        services.AddScoped<IBestPriceService>(sp =>
        {
            var inner = sp.GetRequiredService<BestPriceService>();
            var cache = sp.GetRequiredService<IMemoryCache>();
            return new CachedBestPriceService(inner, cache);
        });

        return services;
    }
}