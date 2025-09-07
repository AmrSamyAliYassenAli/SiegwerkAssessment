namespace Pricing.Tests.Unit.Repositories;

/// <summary>
/// Provides a suite of unit tests for the <see cref="PriceListRepository"/>'s query methods.
/// These tests use an in-memory SQLite database to ensure the repository correctly
/// filters, paginates, and projects data into DTOs.
/// </summary>
public class PriceListRepositoryQueryTests : IAsyncLifetime
{
    private readonly SqliteConnection _conn = new("Data Source=:memory:");
    private PricingDbContext _db = null!;
    private IPriceListRepository _repo = null!;

    /// <summary>
    /// Initializes the in-memory database and seeds it with test data.
    /// This method is called before each test.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _conn.OpenAsync();
        var options = new DbContextOptionsBuilder<PricingDbContext>().UseSqlite(_conn).Options;
        _db = new PricingDbContext(options);
        await _db.Database.EnsureCreatedAsync();

        _db.Suppliers.Add(new Supplier { Id = 1, Name = "S1", Country = "DE", Active = true, Preferred = false, LeadTimeDays = 7 });
        _db.PriceListEntries.AddRange(
            new PriceListEntry { SupplierId = 1, Sku = "ABC123", ValidFrom = new DateOnly(2025, 8, 1), ValidTo = new DateOnly(2025, 12, 31), Currency = "EUR", PricePerUom = 9.50m, MinQty = 100 },
            new PriceListEntry { SupplierId = 1, Sku = "XYZ777", ValidFrom = new DateOnly(2025, 9, 1), ValidTo = new DateOnly(2025, 11, 30), Currency = "USD", PricePerUom = 5.40m, MinQty = 10 }
        );
        await _db.SaveChangesAsync();

        _repo = new PriceListRepository(_db);
    }

    /// <summary>
    /// Disposes of the in-memory database connection and context after each test.
    /// This method is called after each test.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _conn.DisposeAsync();
    }

    /// <summary>
    /// Tests that the <see cref="IPriceListRepository.ListAsync"/> method correctly filters,
    /// paginates, and projects data based on the provided filter criteria.
    /// </summary>
    [Fact]
    public async Task List_filters_and_paginates_and_projects_DTOs()
    {
        var (page, size) = (1, 10);
        var filter = new PriceFilter("ABC123", new DateOnly(2025, 9, 1), null, null, page, size);

        var result = await _repo.ListAsync(filter, CancellationToken.None);

        result.Page.Should().Be(page);
        result.PageSize.Should().Be(size);
        result.Total.Should().Be(1);
        result.Items.Should().HaveCount(1);
        var dto = result.Items[0];
        dto.Sku.Should().Be("ABC123");
        dto.Currency.Should().Be("EUR");
    }
}