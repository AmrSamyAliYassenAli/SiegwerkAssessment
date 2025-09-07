namespace Pricing.Api.Utilities;

/// <summary>
/// Provides utility methods for configuring and managing the application's database.
/// </summary>
internal static class DatabaseUtility
{
    /// <summary>
    /// Adds and configures the application's database context to the service collection.
    /// It uses a connection string from configuration or defaults to an SQLite file in the 'App_Data' directory.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <param name="environment">The web hosting environment.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {

        string dataDir = InitializeDatabaseDirectory(environment);

        services.AddDbContext<PricingDbContext>(opt =>
        {
            var cs = configuration.GetConnectionString("Pricing") ?? $"Data Source={Path.Combine(dataDir, "pricing.db")};Cache=Shared";
            opt.UseSqlite(cs);
        });

        return services;
    }

    /// <summary>
    /// Initializes the directory for the database file.
    /// </summary>
    /// <param name="environment">The web hosting environment.</param>
    /// <returns>The path to the initialized data directory.</returns>
    private static string InitializeDatabaseDirectory(IWebHostEnvironment environment)
    {
        var dataDir = Path.Combine(environment.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDir);
        return dataDir;
    }
}