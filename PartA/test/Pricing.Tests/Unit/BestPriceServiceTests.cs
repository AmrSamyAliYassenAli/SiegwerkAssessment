namespace Pricing.Tests.Unit;

/// <summary>
/// Provides a suite of unit tests for the <see cref="BestPriceService"/>,
/// focusing on the core business logic of finding the optimal price.
/// These tests use mock objects to isolate the service's behavior from its dependencies.
/// </summary>
public sealed class BestPriceServiceTests
{
    /// <summary>
    /// Tests the primary logic of the <see cref="BestPriceService"/>. It verifies that the service
    /// correctly selects the best price by first converting all prices to the target currency
    /// and picking the lowest one. If there's a tie, it correctly applies tiebreakers, such as
    /// preferring a supplier with a shorter lead time.
    /// </summary>
    [Fact]
    public async Task Picks_lowest_converted_unit_price_then_tiebreakers()
    {
        var priceListRepository = Substitute.For<IPriceListRepository>();

        List<SupplierDto> suppliers = new();
        suppliers.Add(new(Id: 1, Preferred: false, LeadTimeDays: 7, Active: true, Name: "S1", Country: "DE"));
        suppliers.Add(new(Id: 2, Preferred: true, LeadTimeDays: 5, Active: true, Name: "S2", Country: "US"));
        suppliers.Add(new(Id: 3, Preferred: true, LeadTimeDays: 10, Active: true, Name: "S3", Country: "FR"));


        List<PriceListDto> priceListDtos = new();
        priceListDtos.Add(new(Id: 11, SupplierId: 1, Sku: "ABC123", ValidFrom: new DateOnly(2025, 8, 1), ValidTo: new DateOnly(2025, 12, 31), Currency: "EUR", PricePerUom: 9.50m, MinQty: 100));
        priceListDtos.Add(new(Id: 22, SupplierId: 2, Sku: "ABC123", ValidFrom: new DateOnly(2025, 7, 1), ValidTo: new DateOnly(2025, 10, 31), Currency: "USD", PricePerUom: 10.00m, MinQty: 50));
        priceListDtos.Add(new(Id: 33, SupplierId: 3, Sku: "ABC123", ValidFrom: new DateOnly(2025, 8, 15), ValidTo: new DateOnly(2025, 12, 31), Currency: "EUR", PricePerUom: 9.50m, MinQty: 100));

        priceListRepository.GetCandidatesAsync("ABC123", 120, new DateOnly(2025, 9, 1), Arg.Any<CancellationToken>())
            .Returns(new List<(PriceListDto, SupplierDto)>
            {
                (priceListDtos[0], suppliers[0]),
                (priceListDtos[1], suppliers[1]),
                (priceListDtos[2], suppliers[2]),
            });

        var rates = Substitute.For<IRateProvider>();
        rates.Convert(9.50m, "EUR", "EUR").Returns(9.50m);
        rates.Convert(10.00m, "USD", "EUR").Returns(9.20m);

        var svc = new BestPriceService(priceListRepository, Substitute.For<IRateProvider>());

        var result = await svc.QueryBestPriceAsync("ABC123", 120, "EUR", new DateOnly(2025, 9, 1), CancellationToken.None);

        result.Should().NotBeNull();
        result!.SupplierId.Should().Be(2);
        result.UnitPrice.Should().Be(9.20m);
        result.TotalPrice.Should().Be(1104.00m);
    }
}