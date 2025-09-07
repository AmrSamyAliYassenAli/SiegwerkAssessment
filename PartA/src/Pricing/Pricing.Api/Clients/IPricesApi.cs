namespace Pricing.Api.Clients;
public interface IPricesApi
{
    [Multipart]
    [Post("/prices/upload")]
    Task UploadAsync([AliasAs("file")] StreamPart file, CancellationToken ct = default);

    [Get("/prices")]
    Task<PagedResult<PriceListDto>> ListAsync(string? sku = null, DateOnly? validOn = null, string? currency = null,
        int? supplierId = null, int page = 1, int pageSize = 20, CancellationToken ct = default);
}