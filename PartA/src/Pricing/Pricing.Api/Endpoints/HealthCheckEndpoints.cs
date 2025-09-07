namespace Pricing.Api.Endpoints;

/// <summary>
/// Provides extension methods for mapping liveness and readiness health check endpoints.
/// </summary>
internal static class HealthCheckEndpoints
{
    /// <summary>
    /// Maps a health check endpoint at "/live" to indicate the application is running.
    /// This endpoint is primarily used by container orchestrators (e.g., Kubernetes) for liveness probes.
    /// It returns a successful response if the application is up, without checking external dependencies.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to map the endpoint on.</param>
    /// <returns>The same <see cref="IEndpointRouteBuilder"/> instance for chaining.</returns>
    internal static IEndpointRouteBuilder MapLivenessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", async (HealthCheckService healthCheckService) =>
        {
            var report = await healthCheckService.CheckHealthAsync();
            return report.Status == HealthStatus.Healthy ? Results.Ok(report) : Results.Problem();
        })
        .WithTags("Health")
        .WithName("LivenessChecks");

        return app;
    }

    /// <summary>
    /// Maps a health check endpoint at "/ready" to indicate the application is ready to handle requests.
    /// This endpoint is used by container orchestrators (e.g., Kubernetes) for readiness probes.
    /// It performs a more comprehensive check, including dependencies tagged with "ready."
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to map the endpoint on.</param>
    /// <returns>The same <see cref="IEndpointRouteBuilder"/> instance for chaining.</returns>
    internal static IEndpointRouteBuilder MapReadinessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    Status = report.Status.ToString(),
                    Checks = report.Entries.Select(e => new
                    {
                        Name = e.Key,
                        Status = e.Value.Status.ToString(),
                        Description = e.Value.Description
                    })
                });
            }
        })
        .WithTags("Health")
        .WithName("ReadinessChecks");

        return app;
    }
}