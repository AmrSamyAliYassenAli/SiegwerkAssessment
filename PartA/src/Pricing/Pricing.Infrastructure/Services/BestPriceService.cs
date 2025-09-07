namespace Pricing.Infrastructure.Services;

/// <summary>
/// Provides a service for querying the best price for a given product and quantity.
/// The service determines the "best" price based on a set of rules, including lowest unit price,
/// supplier preference, and lead time.
/// </summary>
public sealed class BestPriceService : IBestPriceService
{
    private readonly IPriceListRepository _priceListRepository;
    private readonly IRateProvider _rateProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BestPriceService"/> class.
    /// </summary>
    /// <param name="priceListRepository">The repository for accessing price list data.</param>
    /// <param name="rateProvider">The provider for currency conversion rates.</param>
    public BestPriceService(IPriceListRepository priceListRepository, IRateProvider rateProvider)
    {
        _priceListRepository = priceListRepository;
        _rateProvider = rateProvider;
    }

    /// <summary>
    /// Queries for the best available price for a product based on its SKU, quantity, desired currency, and a specific date.
    /// </summary>
    /// <param name="sku">The SKU of the product.</param>
    /// <param name="quantity">The desired quantity of the product.</param>
    /// <param name="currency">The desired currency for the final price.</param>
    /// <param name="date">The date on which the price should be valid.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="BestPriceResultDto"/> representing the best price, or null if no suitable price is found.</returns>
    public async Task<BestPriceResultDto?> QueryBestPriceAsync(string sku, int quantity, string currency, DateOnly date, CancellationToken ct)
    {
        var candidates = await _priceListRepository.GetCandidatesAsync(sku, quantity, date, ct);
        if (candidates.Count == 0) return null;

        var scored = candidates.Select(c =>
        {
            var converted = _rateProvider.Convert(c.priceListDto.PricePerUom, c.priceListDto.Currency, currency);
            return new
            {
                Entry = c.priceListDto,
                Supplier = c.Supplier,
                ConvertedUnitPrice = converted
            };
        }).ToList();

        var best = scored
            .OrderBy(x => x.ConvertedUnitPrice)
            .ThenByDescending(x => x.Supplier.Preferred)
            .ThenBy(x => x.Supplier.LeadTimeDays)
            .ThenBy(x => x.Supplier.Id)
            .First();

        var total = Math.Round(best.ConvertedUnitPrice * quantity, 2, MidpointRounding.AwayFromZero);
        var reason = $"Chosen by lowest unit price; tie-breakers applied: Preferred={best.Supplier.Preferred}, LeadTimeDays={best.Supplier.LeadTimeDays}, SupplierId={best.Supplier.Id}";

        return new BestPriceResultDto(
            Sku: sku,
            Quantity: quantity,
            Currency: currency,
            Date: date,
            SupplierId: best.Supplier.Id,
            UnitPrice: Math.Round(best.ConvertedUnitPrice, 2, MidpointRounding.AwayFromZero),
            TotalPrice: total,
            Reason: reason
        );
    }
}