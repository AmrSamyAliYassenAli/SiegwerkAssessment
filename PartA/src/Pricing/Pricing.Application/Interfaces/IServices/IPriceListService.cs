namespace Pricing.Application.Interfaces.IServices;

/// <summary>
/// Provides core business logic for managing price lists, including uploading data
/// from CSV files and retrieving paged lists of entries.
/// </summary>
public interface IPriceListService
{
    /// <summary>
    /// Handles the entire process of uploading and parsing a CSV file stream to add new price list entries.
    /// This method performs row-by-row validation for data integrity and checks for
    /// overlapping entries within the uploaded file and against existing data in the database.
    /// </summary>
    /// <param name="csvStream">The stream of the CSV file to upload.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An <see cref="UploadResult"/> indicating success or failure, with a message for failures.</returns>
    Task<UploadResult> UploadCsvAsync(Stream csvStream, CancellationToken ct);

    /// <summary>
    /// Retrieves a paginated and filtered list of price list entries.
    /// </summary>
    /// <param name="filter">The filter criteria for the query.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="PagedResult{PriceListDto}"/> containing the filtered list of entries.</returns>
    Task<PagedResult<PriceListDto>> ListAsync(PriceFilter filter, CancellationToken ct);
}