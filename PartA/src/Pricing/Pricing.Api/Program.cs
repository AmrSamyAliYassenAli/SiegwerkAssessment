using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;
IHostBuilder host = builder.Host;

host.ConfigureSerilog();

//services.AddAntiforgery();

services.AddHealthChecksUtility(configuration);

services.AddRefitUtility(configuration);

services.AddProblemDetails();

services.AddCorsUtility();

services.AddRequestSizeUtility();

services.AddSwaggerUtility();

services.AddDatabase(configuration, environment);

services.AddInjections();

services.AddMemoryCache();

using (var app = builder.Build())
{
    //app.UseAntiforgery();

    app.UseCors("AllowAll");

    app.MapHealthChecks("/health");

    app.UseMiddleware<RequestLoggingMiddleware>();

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseExceptionHandler();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
        RequestPath = "/upload"
    }); 

    app.UseStatusCodePages();

    app.UseSwagger();

    app.UseSwaggerUI();

    await app.ApplyMigrationsAndSeed<PricingDbContext>();

    app.MapLivenessEndpoints();

    app.MapReadinessEndpoints();

    app.MapTestLogEndpoints();

    app.MapSuppliersEndpoints();

    app.MapProductsEndpoints();

    app.MapPricesEndpoints();

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Price API starting up...");
    logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    logger.LogInformation("Application Name: {ApplicationName}", builder.Environment.ApplicationName);

    try
    {
        app.Run();
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Application terminated unexpectedly");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}