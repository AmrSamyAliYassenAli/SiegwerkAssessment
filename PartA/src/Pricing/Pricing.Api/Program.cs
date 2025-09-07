var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

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
	try
	{
        app.UseCors("AllowAll");

        app.UseExceptionHandler();

        app.UseStaticFiles();

        app.UseStatusCodePages();

        app.UseSwagger();

        app.UseSwaggerUI();

        await app.ApplyMigrationsAndSeed<PricingDbContext>();

        app.MapSuppliersEndpoints();

        app.MapProductsEndpoints();

        app.MapPricesEndpoints();

        app.Run();
    }
	catch (Exception ex)
	{
		throw new(ex.Message);
	}
}