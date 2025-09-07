namespace Pricing.Tests.Integration;

/// <summary>
/// Provides a suite of integration tests for the suppliers API endpoints.
/// These tests use an in-memory web application factory to perform end-to-end
/// validation of the CRUD (Create, Read, Update, Delete) operations.
/// </summary>
public class SuppliersCrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuppliersCrudTests"/> class.
    /// </summary>
    /// <param name="factory">The web application factory used to create an in-memory test server.</param>
    public SuppliersCrudTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Performs an end-to-end test to create, retrieve, update, and delete a supplier.
    /// </summary>
    [Fact]
    public async Task Create_Get_Update_Delete_supplier()
    {
        // Act: Create a new supplier
        var created = await (await _client.PostAsJsonAsync("/suppliers", new CreateSupplierDto("ACME", "DE", true, true, 5)))
            .Content.ReadFromJsonAsync<SupplierDto>();
        created.Should().NotBeNull();

        // Act: Get the created supplier by ID
        var fetched = await _client.GetFromJsonAsync<SupplierDto>($"/suppliers/{created!.Id}");

        // Assert: Verify the retrieved supplier's name
        fetched!.Name.Should().Be("ACME");

        // Act: Update the supplier
        var updateRes = await _client.PutAsJsonAsync($"/suppliers/{created.Id}", new UpdateSupplierDto(created!.Id, "ACME2", "DE", true, false, 6));

        // Assert: Verify the update was successful
        updateRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act: Delete the supplier
        var del = await _client.DeleteAsync($"/suppliers/{created.Id}");

        // Assert: Verify the deletion was successful
        del.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}