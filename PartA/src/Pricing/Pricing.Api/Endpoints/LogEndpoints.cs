namespace Pricing.Api.Endpoints;

/// <summary>
/// Provides a static class for mapping logging-related minimal API endpoints.
/// </summary>
internal static class LogEndpoints
{
    /// <summary>
    /// Maps a test endpoint at "/log/test" that generates log messages at all severity levels.
    /// This is useful for verifying that logging is configured correctly across the application.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to map the endpoint on.</param>
    /// <returns>The same <see cref="IEndpointRouteBuilder"/> instance for chaining.</returns>
    internal static IEndpointRouteBuilder MapTestLogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/log/test", (ILogger<Program> logger) =>
        {
            logger.LogTrace("This is a trace log message.");
            logger.LogDebug("This is a debug log message.");
            logger.LogInformation("This is an information log message.");
            logger.LogWarning("This is a warning log message.");
            logger.LogError("This is an error log message.");
            logger.LogCritical("This is a critical log message.");
            return Results.Ok(new { Message = "Log messages have been generated. Check your logs.", timestamp = DateTime.UtcNow });
        })
          .WithTags("Logs")
          .WithName("TestLogs");

        return app;
    }
}