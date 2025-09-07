namespace Pricing.Api.Utilities;

/// <summary>
/// Provides extension methods for setting up Serilog with host and application builders.
/// </summary>
internal static class SerilogUtility
{
    /// <summary>
    /// Configures and enables Serilog as the primary logging provider for the application host.
    /// This method sets up Serilog to read configuration from appsettings and automatically
    /// enriches logs with context from services.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to configure.</param>
    /// <returns>The same <see cref="IHostBuilder"/> instance for chaining.</returns>
    internal static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/price-api-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .CreateLogger();

        return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration)
                               .ReadFrom.Services(services)
                               .Enrich.FromLogContext();
        });
    }
}