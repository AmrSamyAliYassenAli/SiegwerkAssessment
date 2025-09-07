namespace Pricing.Tests.Unit.Repositories;
/// <summary>
/// Provides a suite of unit tests for the <see cref="ISupplierRepository"/> implementation.
/// These tests use an in-memory SQLite database to validate the CRUD (Create, Read, Update, Delete)
/// operations and other key behaviors of the repository.
/// </summary>
public class SupplierRepositoryTests : IAsyncLifetime
{
    private readonly SqliteConnection _conn = new("Data Source=:memory:");
    private PricingDbContext _db = null!;
    private ISupplierRepository _repo = null!;

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
        _repo = new SupplierRepository(_db);
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
    /// Tests that the Add, GetById, and GetAll methods are consistent
    /// and that the GetAll method returns suppliers in a consistent order.
    /// </summary>
    [Fact]
    public async Task Add_GetById_GetAll_are_consistent_and_ordered()
    {
        var s1 = await _repo.AddAsync(new CreateSupplierDto("S1", "DE", true, false, 7), CancellationToken.None);
        var s2 = await _repo.AddAsync(new CreateSupplierDto("S2", "US", true, true, 5), CancellationToken.None);

        var fetched = await _repo.GetAsync(s1.Id, CancellationToken.None);
        fetched!.Name.Should().Be("S1");
        fetched.Country.Should().Be("DE");
        fetched.Preferred.Should().BeFalse();

        var all = await _repo.GetAllAsync(CancellationToken.None);
        all.Select(x => x.Id).Should().BeInAscendingOrder().And.Contain(s1.Id).And.Contain(s2.Id);
    }

    /// <summary>
    /// Tests that updating an existing supplier returns true and correctly persists the changes.
    /// </summary>
    [Fact]
    public async Task Update_existing_returns_true_and_persists_changes()
    {
        var created = await _repo.AddAsync(new CreateSupplierDto("S1", "DE", true, false, 7), CancellationToken.None);

        var ok = await _repo.UpdateAsync(new UpdateSupplierDto(created.Id, "S1-new", "DE", true, true, 6), CancellationToken.None);
        ok.Should().BeTrue();

        var fetched = await _repo.GetAsync(created.Id, CancellationToken.None);
        fetched!.Name.Should().Be("S1-new");
        fetched.Preferred.Should().BeTrue();
        fetched.LeadTimeDays.Should().Be(6);
    }

    /// <summary>
    /// Tests that attempting to update a nonexistent supplier returns false.
    /// </summary>
    [Fact]
    public async Task Update_nonexistent_returns_false()
    {
        var ok = await _repo.UpdateAsync(new UpdateSupplierDto(999, "X", "DE", true, false, 1), CancellationToken.None);
        ok.Should().BeFalse();
    }

    /// <summary>
    /// Tests that deleting an existing supplier returns true and successfully removes it from the database.
    /// </summary>
    [Fact]
    public async Task Delete_existing_returns_true_and_removes()
    {
        var created = await _repo.AddAsync(new CreateSupplierDto("S1", "DE", true, false, 7), CancellationToken.None);

        var ok = await _repo.DeleteAsync(created.Id, CancellationToken.None);
        ok.Should().BeTrue();

        var fetched = await _repo.GetAsync(created.Id, CancellationToken.None);
        fetched.Should().BeNull();
    }

    /// <summary>
    /// Tests that attempting to delete a nonexistent supplier returns false.
    /// </summary>
    [Fact]
    public async Task Delete_nonexistent_returns_false()
    {
        var ok = await _repo.DeleteAsync(12345, CancellationToken.None);
        ok.Should().BeFalse();
    }
}