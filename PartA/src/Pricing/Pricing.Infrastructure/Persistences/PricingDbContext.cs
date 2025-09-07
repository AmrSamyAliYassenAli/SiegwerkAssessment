namespace Pricing.Infrastructure.Persistences;

/// <summary>
/// Represents the database context for the pricing application.
/// This context manages the database connection and the sets of entities.
/// </summary>
public sealed class PricingDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PricingDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options) { }

    /// <summary>
    /// Gets or sets the database set for <see cref="Supplier"/> entities.
    /// </summary>
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    /// <summary>
    /// Gets or sets the database set for <see cref="Product"/> entities.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Gets or sets the database set for <see cref="PriceListEntry"/> entities.
    /// </summary>
    public DbSet<PriceListEntry> PriceListEntries => Set<PriceListEntry>();

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// in the context. This method is used to configure indexes and property conversions.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasIndex(p => p.Sku).IsUnique();

        modelBuilder.Entity<PriceListEntry>().HasIndex(p => new { p.SupplierId, p.Sku, p.ValidFrom, p.ValidTo });

        modelBuilder.Entity<PriceListEntry>()
            .Property(p => p.ValidFrom)
            .HasConversion(v => v.ToDateTime(TimeOnly.MinValue), v => DateOnly.FromDateTime(v));

        modelBuilder.Entity<PriceListEntry>()
            .Property(p => p.ValidTo)
            .HasConversion(v => v.ToDateTime(TimeOnly.MinValue), v => DateOnly.FromDateTime(v));
    }
}