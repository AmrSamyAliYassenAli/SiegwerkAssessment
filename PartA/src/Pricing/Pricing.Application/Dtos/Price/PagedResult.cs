namespace Pricing.Application.Dtos.Price;

/// <summary>
/// Represents a paginated list of results.
/// </summary>
/// <typeparam name="T">The type of the items in the paged result.</typeparam>
/// <param name="Page">The current page number.</param>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="Total">The total number of items across all pages.</param>
/// <param name="Items">The list of items for the current page.</param>
public record PagedResult<T>(
    int Page, 
    int PageSize, 
    int Total, 
    IReadOnlyList<T> Items);