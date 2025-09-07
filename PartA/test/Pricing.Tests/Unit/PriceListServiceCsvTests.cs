namespace Pricing.Tests.Unit;

/// <summary>
/// Provides a suite of unit tests for the CSV upload functionality of the <see cref="PriceListService"/>.
/// These tests use mock objects to ensure that the service correctly handles both valid and invalid
/// CSV data, specifically focusing on business rules such as date range overlaps.
/// </summary>
public class PriceListServiceCsvTests
{
    /// <summary>
    /// Tests that the service correctly rejects a CSV upload that contains overlapping date ranges
    /// for the same supplier and SKU. This ensures that the business logic prevents invalid data
    /// from being processed.
    /// </summary>
    [Fact]
    public async Task Rejects_overlapping_ranges_in_single_upload()
    {
        var repo = Substitute.For<IPriceListRepository>();
        repo.HasOverlapAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(false);
        var svc = new PriceListService(repo);

        var csv = string.Join('\n', new[]
        {
            "SupplierId,Sku,ValidFrom,ValidTo,Currency,PricePerUom,MinQty",
            "1,ABC123,2025-08-01,2025-12-31,EUR,9.50,100",
            "1,ABC123,2025-09-01,2025-10-31,EUR,9.40,50" // overlaps same supplier+sku
		});

        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        var res = await svc.UploadCsvAsync(stream, CancellationToken.None);

        res.IsSuccess.Should().BeFalse();
        res.Title.Should().Be("Overlapping range");
    }

    /// <summary>
    /// Tests that the service accepts a valid CSV file and calls the repository's AddRangeAsync method
    /// to persist the data. This verifies that the service correctly processes valid input.
    /// </summary>
    [Fact]
    public async Task Accepts_valid_rows_and_calls_AddRange()
    {
        var repo = Substitute.For<IPriceListRepository>();
        repo.HasOverlapAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns(false);
        var svc = new PriceListService(repo);

        var csv = string.Join('\n', new[]
        {
            "SupplierId,Sku,ValidFrom,ValidTo,Currency,PricePerUom,MinQty",
            "1,ABC123,2025-08-01,2025-12-31,EUR,9.50,100",
            "2,ABC123,2025-07-01,2025-10-31,USD,10.00,50"
        });
        await using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        var res = await svc.UploadCsvAsync(stream, CancellationToken.None);

        res.IsSuccess.Should().BeTrue();
        await repo.Received(1).AddRangeAsync(Arg.Any<IEnumerable<PriceListDto>>(), Arg.Any<CancellationToken>());
    }
}