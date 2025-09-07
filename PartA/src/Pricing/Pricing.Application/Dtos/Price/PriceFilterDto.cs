namespace Pricing.Application.Dtos.Price;
/// <summary>
/// Represents a set of filter criteria for querying a price list.
/// </summary>
/// <param name="Sku">The SKU of the product to filter by.</param>
/// <param name="ValidOn">The date on which the price must be valid.</param>
/// <param name="Currency">The currency to filter by.</param>
/// <param name="SupplierId">The ID of the supplier to filter by.</param>
/// <param name="Page">The page number for pagination.</param>
/// <param name="PageSize">The number of items per page.</param>
public record PriceFilter(
    string? Sku,
    DateOnly? ValidOn,
    string? Currency,
    int? SupplierId,
    int Page,
    int PageSize);