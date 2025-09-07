namespace Pricing.Api.Services;

/// <summary>
/// Provides API-related services for managing and querying price lists, including uploading new data, listing existing entries, and finding the best price for a product.
/// </summary>
public class PriceListApiService
{
    private readonly IPriceListService _priceListService;
    private readonly IBestPriceService _bestPriceService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceListApiService"/> class.
    /// </summary>
    /// <param name="priceListService">The price list service for handling price list operations.</param>
    /// <param name="bestPriceService">The best price service for finding optimal prices.</param>
    public PriceListApiService(IPriceListService priceListService, IBestPriceService bestPriceService)
    {
        _priceListService = priceListService;
        _bestPriceService = bestPriceService;
    }

    /// <summary>
    /// Handles the upload of a price list from a multipart/form-data request.
    /// </summary>
    /// <param name="req">The HTTP request containing the file to upload.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Results{TResult1, TResult2}"/> indicating the outcome of the upload.</returns>
    public async Task<Results<Ok, ProblemHttpResult>> UploadAsync(HttpRequest req, CancellationToken ct)
    {
        if (!req.HasFormContentType) return TypedResults.Problem(title: "Invalid content type", detail: "Expected multipart/form-data", statusCode: 400);
        var form = await req.ReadFormAsync(ct);
        var file = form.Files.GetFile("file");
        if (file is null || file.Length == 0) return TypedResults.Problem(title: "Missing file", statusCode: 400);

        using var stream = file.OpenReadStream();
        var result = await _priceListService.UploadCsvAsync(stream, ct);
        return result.IsSuccess ? TypedResults.Ok() : TypedResults.Problem(title: result.Title, detail: result.Detail, statusCode: 400);
    }

    /// <summary>
    /// Retrieves a paginated list of price list entries based on provided filters.
    /// </summary>
    /// <param name="sku">(Optional) The SKU to filter by.</param>
    /// <param name="validOn">(Optional) The date to check for price validity.</param>
    /// <param name="currency">(Optional) The currency to filter by.</param>
    /// <param name="supplierId">(Optional) The supplier ID to filter by.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An <see cref="Ok{TValue}"/> result containing a <see cref="PagedResult{T}"/> of <see cref="PriceListDto"/> objects.</returns>
    public async Task<Ok<PagedResult<PriceListDto>>> ListAsync(
        [FromQuery] string? sku, [FromQuery] DateOnly? validOn, [FromQuery] string? currency,
        [FromQuery] int? supplierId, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken ct)
    {
        var filter = new PriceFilter(sku, validOn, currency, supplierId, page <= 0 ? 1 : page, pageSize <= 0 ? 20 : Math.Min(pageSize, 200));
        var res = await _priceListService.ListAsync(filter, ct);
        return TypedResults.Ok(res);
    }

    /// <summary>
    /// Queries for the best price for a specific product, quantity, and date, applying a static conversion rate and business rules.
    /// </summary>
    /// <param name="sku">The SKU of the product.</param>
    /// <param name="qty">The quantity of the product.</param>
    /// <param name="currency">The target currency for the price.</param>
    /// <param name="date">The date for which to find the price.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Results{TResult1, TResult2, TResult3}"/> with the best price result, or a <see cref="NotFound"/> result if no price is found.</returns>
    public async Task<Results<Ok<BestPriceResultDto>, NotFound, ProblemHttpResult>> BestAsync(
        [FromQuery] string sku, [FromQuery] int qty, [FromQuery] string currency, [FromQuery] DateOnly date, CancellationToken ct)
    {
        if (qty <= 0) return TypedResults.Problem(title: "Invalid qty", statusCode: 400);
        if (string.IsNullOrWhiteSpace(sku)) return TypedResults.Problem(title: "Invalid sku", statusCode: 400);
        if (string.IsNullOrWhiteSpace(currency)) return TypedResults.Problem(title: "Invalid currency", statusCode: 400);

        var best = await _bestPriceService.QueryBestPriceAsync(sku, qty, currency, date, ct);
        return best is null ? TypedResults.NotFound() : TypedResults.Ok(best);
    }
}