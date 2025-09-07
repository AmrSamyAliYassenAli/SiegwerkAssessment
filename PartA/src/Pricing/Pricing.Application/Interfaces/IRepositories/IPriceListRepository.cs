namespace Pricing.Application.Interfaces.IRepositories;

/// <summary>
/// Implements the repository pattern for managing <see cref="PriceListEntry"/> entities.
/// This class provides methods for querying, adding, and checking for price list entry data.
/// </summary>
public interface IPriceListRepository
{
    /// <summary>
    /// Retrieves a paginated list of price list entries based on the provided filter.
    /// </summary>
    /// <param name="filter">The filter criteria for the price list entries.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A paged result containing the filtered and paginated price list entries.</returns>
    Task<PagedResult<PriceListDto>> ListAsync(PriceFilter filter, CancellationToken ct);

    /// <summary>
    /// Adds a range of new price list entries to the database.
    /// </summary>
    /// <param name="priceListDto">A collection of DTOs representing the new price list entries.</param>
    /// <param name="ct">The cancellation token.</param>
    Task AddRangeAsync(IEnumerable<PriceListDto> priceListDto, CancellationToken ct);

    /// <summary>
    /// Checks for any overlapping price list entries based on a given supplier, SKU, and date range.
    /// </summary>
    /// <param name="supplierId">The ID of the supplier.</param>
    /// <param name="sku">The SKU of the product.</param>
    /// <param name="from">The start date of the validity period.</param>
    /// <param name="to">The end date of the validity period.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether an overlap exists.</returns>
    Task<bool> HasOverlapAsync(int supplierId, string sku, DateOnly from, DateOnly to, CancellationToken ct);

    /// <summary>
    /// Retrieves a list of candidate price list entries and their associated suppliers based on specific criteria.
    /// </summary>
    /// <param name="sku">The SKU of the product.</param>
    /// <param name="quantity">The minimum quantity required.</param>
    /// <param name="date">The date of validity.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A list of tuples, each containing a PriceListDto and a SupplierDto.</returns>
    Task<List<(PriceListDto priceListDto, SupplierDto Supplier)>> GetCandidatesAsync(
        string sku, int quantity, DateOnly date, CancellationToken ct);
}