namespace Pricing.Tests.Unit.Repositories;

/// <summary>
/// Provides a suite of unit tests for the <see cref="IProductRepository"/> implementation.
/// These tests use an in-memory SQLite database to validate the CRUD (Create, Read, Update, Delete)
/// operations and other key behaviors of the repository.
/// </summary>
public class ProductRepositoryTests : IAsyncLifetime
{
    private readonly SqliteConnection _conn = new("Data Source=:memory:");
    private PricingDbContext _db = null!;
    private IProductRepository _repo = null!;

    /// <summary>
    /// Initializes the in-memory database and seeds it with test data before each test.
    /// This ensures a clean state for every test case.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _conn.OpenAsync();
        var options = new DbContextOptionsBuilder<PricingDbContext>()
            .UseSqlite(_conn)
            .Options;
        _db = new PricingDbContext(options);
        await _db.Database.EnsureCreatedAsync();
        _repo = new ProductRepository(_db);
    }

    /// <summary>
    /// Disposes of the in-memory database connection and context after each test.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _conn.DisposeAsync();
    }

    /// <summary>
    /// Tests that the Add, GetById, GetBySku, and GetAll methods are consistent
    /// and that the GetAll method returns products in a consistent order.
    /// </summary>
    [Fact]
    public async Task Add_GetById_GetBySku_GetAll_are_consistent_and_ordered()
    {
        var p1 = await _repo.AddAsync(new CreateProductDto("SKU-1", "A", "EA", ""), CancellationToken.None);
        var p2 = await _repo.AddAsync(new CreateProductDto("SKU-2", "B", "EA", ""), CancellationToken.None);

        var byId = await _repo.GetAsync(p1.Id, CancellationToken.None);
        byId!.Sku.Should().Be("SKU-1");

        var bySku = await _repo.GetBySkuAsync("SKU-2", CancellationToken.None);
        bySku!.Name.Should().Be("B");

        var all = await _repo.GetAllAsync(CancellationToken.None);
        all.Select(x => x.Sku).Should().BeInAscendingOrder().And.Contain(new[] { "SKU-1", "SKU-2" });
    }

    /// <summary>
    /// Tests that updating an existing product returns true and correctly persists the changes.
    /// </summary>
    [Fact]
    public async Task Update_existing_returns_true_and_persists_changes()
    {
        var p = await _repo.AddAsync(new CreateProductDto("SKU-1", "A", "EA", ""), CancellationToken.None);

        var ok = await _repo.UpdateAsync(new UpdateProductDto(p.Id, "SKU-1", "A-new", "EA", "H1"), CancellationToken.None);
        ok.Should().BeTrue();

        var fetched = await _repo.GetAsync(p.Id, CancellationToken.None);
        fetched!.Name.Should().Be("A-new");
        fetched.HazardClass.Should().Be("H1");
    }

    /// <summary>
    /// Tests that attempting to update a nonexistent product returns false.
    /// </summary>
    [Fact]
    public async Task Update_nonexistent_returns_false()
    {
        var ok = await _repo.UpdateAsync(new UpdateProductDto(999, "SKU-X", "X", "EA", ""), CancellationToken.None);
        ok.Should().BeFalse();
    }

    /// <summary>
    /// Tests that deleting an existing product returns true and successfully removes it from the database.
    /// </summary>
    [Fact]
    public async Task Delete_existing_returns_true_and_removes()
    {
        var p = await _repo.AddAsync(new CreateProductDto("SKU-1", "A", "EA", ""), CancellationToken.None);

        var ok = await _repo.DeleteAsync(p.Id, CancellationToken.None);
        ok.Should().BeTrue();

        var fetched = await _repo.GetAsync(p.Id, CancellationToken.None);
        fetched.Should().BeNull();
    }

    /// <summary>
    /// Tests that attempting to delete a nonexistent product returns false.
    /// </summary>
    [Fact]
    public async Task Delete_nonexistent_returns_false()
    {
        var ok = await _repo.DeleteAsync(424242, CancellationToken.None);
        ok.Should().BeFalse();
    }

    /// <summary>
    /// Tests that adding a product with a duplicate SKU throws a <see cref="DbUpdateException"/>.
    /// </summary>
    [Fact]
    public async Task Adding_duplicate_Sku_throws_DbUpdateException()
    {
        await _repo.AddAsync(new CreateProductDto("SKU-dup", "A", "EA", ""), CancellationToken.None);

        var act = async () => await _repo.AddAsync(new CreateProductDto("SKU-dup", "B", "EA", ""), CancellationToken.None);

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    /// <summary>
    /// Tests that updating a product with a SKU that already exists on another product throws a <see cref="DbUpdateException"/>.
    /// </summary>
    [Fact]
    public async Task Updating_to_conflicting_Sku_throws_DbUpdateException()
    {
        var p1 = await _repo.AddAsync(new CreateProductDto("SKU-1", "A", "EA", ""), CancellationToken.None);
        var p2 = await _repo.AddAsync(new CreateProductDto("SKU-2", "B", "EA", ""), CancellationToken.None);

        var act = async () => await _repo.UpdateAsync(new UpdateProductDto(p2.Id, "SKU-1", "B", "EA", ""), CancellationToken.None);

        await act.Should().ThrowAsync<DbUpdateException>();
    }
}
