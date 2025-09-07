namespace Pricing.Application.Interfaces.IServices;

public interface IBestPriceService
{
    Task<BestPriceResultDto?> QueryBestPriceAsync(string sku, int quantity, string currency, DateOnly date, CancellationToken ct);
}