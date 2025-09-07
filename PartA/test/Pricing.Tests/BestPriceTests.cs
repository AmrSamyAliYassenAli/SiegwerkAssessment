namespace Pricing.Tests;
public class BestPriceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BestPriceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PricingDbContext>));
                if (descriptor is not null) services.Remove(descriptor);

                services.AddDbContext<PricingDbContext>(opt => opt.UseSqlite("Data Source=:memory:"));
            });
        });
    }

    [Fact]
    public async Task BestPrice_Returns_Winner_With_Currency_Conversion_And_Tiebreakers()
    {
        var client = _factory.CreateClient();

        // Seed CSV upload
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

        var best = await client.GetFromJsonAsync<BestPriceResult>("/pricing/best?sku=ABC123&qty=120&currency=EUR&date=2025-09-01");
        best.Should().NotBeNull();
        best!.SupplierId.Should().Be(2);
        best.UnitPrice.Should().Be(9.20m);
        best.TotalPrice.Should().Be(1104.00m);
    }

    private record BestPriceResult(
        string Sku, 
        int Quantity, 
        string Currency, 
        string Date, 
        int SupplierId, 
        decimal UnitPrice, 
        decimal TotalPrice, 
        string Reason);
}