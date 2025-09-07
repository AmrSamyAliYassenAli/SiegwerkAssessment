namespace Pricing.Infrastructure.Services;

//<!inheritdoc />
public class PriceListService : IPriceListService
{
    private readonly IPriceListRepository _priceListRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceListService"/> class.
    /// </summary>
    /// <param name="priceListRepository">The repository for accessing price list data.</param>
    public PriceListService(IPriceListRepository priceListRepository) => _priceListRepository = priceListRepository;

    //<!inheritdoc />
    public async Task<UploadResult> UploadCsvAsync(Stream csvStream, CancellationToken ct)
    {
        using var reader = new StreamReader(csvStream);
        string? line;
        int row = 0;
        string[]? header = null;
        var entries = new List<PriceListDto>();

        while ((line = await reader.ReadLineAsync()) is not null)
        {
            row++;
            if (row == 1) { header = line.Split(','); continue; }
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = line.Split(',');
            if (cols.Length < 7) return new UploadResult(false, "Invalid CSV", $"Row {row}: expected 7 columns");

            if (!int.TryParse(cols[0], out var supplierId)) return new UploadResult(false, "Invalid SupplierId", $"Row {row}");
            var sku = cols[1];
            if (!DateOnly.TryParse(cols[2], out var validFrom)) return new UploadResult(false, "Invalid ValidFrom", $"Row {row}");
            if (!DateOnly.TryParse(cols[3], out var validTo)) return new UploadResult(false, "Invalid ValidTo", $"Row {row}");
            var currency = cols[4];
            if (!decimal.TryParse(cols[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var pricePerUom)) return new UploadResult(false, "Invalid PricePerUom", $"Row {row}");
            if (!int.TryParse(cols[6], out var minQty)) return new UploadResult(false, "Invalid MinQty", $"Row {row}");
            if (validTo < validFrom) return new UploadResult(false, "Invalid range", $"Row {row}: ValidTo < ValidFrom");

            if (entries.Any(e => e.SupplierId == supplierId && e.Sku == sku && e.ValidFrom <= validTo && validFrom <= e.ValidTo))
                return new UploadResult(false, "Overlapping range", $"Row {row}: overlaps upload for same SupplierId+Sku");

            if (await _priceListRepository.HasOverlapAsync(supplierId, sku, validFrom, validTo, ct))
                return new UploadResult(false, "Overlapping range", $"Row {row}: overlaps existing entries for same SupplierId+Sku");

            entries.Add(new(
                Id: null,
                SupplierId: supplierId,
                Sku: sku,
                ValidFrom: validFrom,
                ValidTo: validTo,
                Currency: currency,
                PricePerUom: pricePerUom,
                MinQty: minQty));
        }

        await _priceListRepository.AddRangeAsync(entries, ct);
        return new UploadResult(true);
    }

    //<!inheritdoc />
    public Task<PagedResult<PriceListDto>> ListAsync(PriceFilter filter, CancellationToken ct) =>
        _priceListRepository.ListAsync(filter, ct);
}