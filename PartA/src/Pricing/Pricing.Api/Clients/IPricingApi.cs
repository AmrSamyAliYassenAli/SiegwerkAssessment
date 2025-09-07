namespace Pricing.Api.Clients;
public interface IPricingApi
{
    [Get("/pricing/best")]
    Task<BestPriceResultDto?> GetBestAsync(string sku, int qty, string currency, DateOnly date, CancellationToken ct = default);
}