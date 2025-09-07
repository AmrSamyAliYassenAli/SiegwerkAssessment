namespace Pricing.Api.Extensions;

/// <summary>
/// Provides extension methods for Entity Framework Core DbContext to handle database migrations and data seeding.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Applies any pending database migrations and seeds initial data into the database.
    /// This method is intended to be called at application startup.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext, which must inherit from <see cref="DbContext"/>.</typeparam>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task ApplyMigrationsAndSeed<TDbContext>(this WebApplication app) where TDbContext : DbContext
    {
        using (var scope = app.Services.CreateScope())
        {
            var pricingDbContext = scope.ServiceProvider.GetRequiredService<PricingDbContext>();

            var pending = await pricingDbContext.Database.GetPendingMigrationsAsync();
            if (pending.Any())
            {
                await pricingDbContext.Database.MigrateAsync();
            }
            else
            {
                await pricingDbContext.Database.EnsureCreatedAsync();
            }
            // Seed data
            await pricingDbContext.SeedDataAsync();
        }
    }

    /// <summary>
    /// Seeds initial data for Suppliers and Products if their respective tables are empty.
    /// </summary>
    /// <param name="pricingDbContext">The pricing database context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private static async Task SeedDataAsync(this PricingDbContext pricingDbContext)
    {
        // Seed suppliers if empty
        if (!await pricingDbContext.Suppliers.AnyAsync())
        {
            pricingDbContext.Suppliers.AddRange(
                new Supplier { Id = 1, Name = "Supplier 1", Country = "DE", Active = true, Preferred = false, LeadTimeDays = 7 },
                new Supplier { Id = 2, Name = "Supplier 2", Country = "US", Active = true, Preferred = true, LeadTimeDays = 5 },
                new Supplier { Id = 3, Name = "Supplier 3", Country = "FR", Active = true, Preferred = false, LeadTimeDays = 6 }
            );
        }

        // Seed products if empty
        if (!await pricingDbContext.Products.AnyAsync())
        {
            pricingDbContext.Products.AddRange(
                new Product { Id = 1, Sku = "ABC123", Name = "Product A", Uom = "EA", HazardClass = "" },
                new Product { Id = 2, Sku = "XYZ777", Name = "Product X", Uom = "EA", HazardClass = "" }
            );
        }

        await pricingDbContext.SaveChangesAsync();
    }
}
