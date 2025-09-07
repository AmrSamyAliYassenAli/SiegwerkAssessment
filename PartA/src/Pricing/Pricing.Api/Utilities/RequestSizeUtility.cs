namespace Pricing.Api.Utilities;

/// <summary>
/// Provides a utility class for configuring request body size limits in the application.
/// </summary>
internal static class RequestSizeUtility
{
    /// <summary>
    /// Configures the maximum request body size for multipart form data and IIS server options.
    /// This is essential for handling large file uploads without exceeding default size limits.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddRequestSizeUtility(this IServiceCollection services)
    {
        services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 100 * 1024 * 1024);

        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = int.MaxValue;
        });

        return services;
    }
}