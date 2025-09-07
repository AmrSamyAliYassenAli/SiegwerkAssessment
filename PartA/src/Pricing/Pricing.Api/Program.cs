var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 100 * 1024 * 1024);

services.AddProblemDetails();

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(c =>
{
    c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
});

services.AddDatabase(configuration, environment);

services.AddInjections();

services.AddMemoryCache();

var app = builder.Build();

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