namespace Pricing.Tests.Integration;

/// <summary>
/// Provides a suite of integration tests for the pricing endpoints.
/// These tests run against a full, in-memory web application instance.
/// </summary>
public sealed class PricingEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PricingEndpointsTests"/> class.
    /// </summary>
    /// <param name="factory">The web application factory used to create an in-memory test server.</param>
    public PricingEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    /// <summary>
    /// Performs an end-to-end integration test of the price upload, list, and best-price calculation endpoints.
    /// This test simulates the entire workflow from uploading a CSV file to querying the best price.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous test operation.</returns>
    [Fact]
    public async Task Upload_List_BestPrice_end_to_end()
    {
        var client = _factory.CreateClient();

        var csv = string.Join('\n', new[]
        {
            "SupplierId,Sku,ValidFrom,ValidTo,Currency,PricePerUom,MinQty",
            "1,ABC123,2025-08-01,2025-12-31,EUR,9.50,100",
            "2,ABC123,2025-07-01,2025-10-31,USD,10.00,50",
            "3,ABC123,2025-08-15,2025-12-31,EUR,9.50,100"
        });
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(csv), "file", "PriceLists.csv");

        var upload = await client.PostAsync("/prices/upload", form);
        upload.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await client.GetFromJsonAsync<PricesPage>("/prices?sku=ABC123&validOn=2025-09-01&page=1&pageSize=50");
        list.Should().NotBeNull();
        list!.Total.Should().Be(3);
        list.Items.Should().HaveCount(3);

        var best = await client.GetFromJsonAsync<BestPriceResult>("/pricing/best?sku=ABC123&qty=120&currency=EUR&date=2025-09-01");
        best.Should().NotBeNull();
        best!.SupplierId.Should().Be(2);
        best.UnitPrice.Should().Be(9.20m);
        best.TotalPrice.Should().Be(1104.00m);
    }

    /// <summary>
    /// Represents a paged result of price list entries, used for deserializing API responses.
    /// </summary>
    /// <param name="Page">The current page number.</param>
    /// <param name="PageSize">The number of items per page.</param>
    /// <param name="Total">The total number of items across all pages.</param>
    /// <param name="Items">The list of price list entries for the current page.</param>
    private sealed record PricesPage(int Page, int PageSize, int Total, List<PriceListEntry> Items);

    /// <summary>
    /// Represents a single price list entry, used for deserializing API responses.
    /// </summary>
    /// <param name="Id">The unique identifier of the price list entry.</param>
    /// <param name="SupplierId">The unique identifier of the supplier.</param>
    /// <param name="Sku">The stock keeping unit.</param>
    /// <param name="Currency">The currency of the price.</param>
    /// <param name="PricePerUom">The price per unit of measure.</param>
    /// <param name="MinQty">The minimum quantity for this price.</param>
    /// <param name="ValidFrom">The start date of the price validity.</param>
    /// <param name="ValidTo">The end date of the price validity.</param>
    private sealed record PriceListEntry(int Id, int SupplierId, string Sku, string Currency, decimal PricePerUom, int MinQty, string ValidFrom, string ValidTo);

    /// <summary>
    /// Represents the result of a best-price query, used for deserializing API responses.
    /// </summary>
    /// <param name="Sku">The stock keeping unit.</param>
    /// <param name="Quantity">The quantity of the item.</param>
    /// <param name="Currency">The requested currency for the price.</param>
    /// <param name="Date">The date for which the price was queried.</param>
    /// <param name="SupplierId">The unique identifier of the supplier with the best price.</param>
    /// <param name="UnitPrice">The unit price in the requested currency.</param>
    /// <param name="TotalPrice">The total price for the requested quantity.</param>
    /// <param name="Reason">A description of how the best price was determined.</param>
    private sealed record BestPriceResult(string Sku, int Quantity, string Currency, string Date, int SupplierId, decimal UnitPrice, decimal TotalPrice, string Reason);
}
