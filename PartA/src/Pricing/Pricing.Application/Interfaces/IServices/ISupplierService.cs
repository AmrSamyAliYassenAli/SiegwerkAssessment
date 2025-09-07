namespace Pricing.Application.Interfaces.IServices;

/// <summary>
/// Provides a service layer for managing supplier-related business logic.
/// This class acts as an intermediary between the API controllers and the
/// supplier data repository, encapsulating business rules and workflows.
/// </summary>
public interface ISupplierService
{
    /// <summary>
    /// Retrieves all suppliers from the data store.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A list of all suppliers as <see cref="SupplierDto"/> objects.</returns>
    Task<IEnumerable<SupplierDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Retrieves a specific supplier by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The <see cref="SupplierDto"/> if found, otherwise null.</returns>
    Task<SupplierDto?> GetAsync(int id, CancellationToken ct);

    /// <summary>
    /// Creates a new supplier in the data store.
    /// </summary>
    /// <param name="dto">The data transfer object containing the new supplier information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The newly created supplier as a <see cref="SupplierDto"/>.</returns>
    Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken ct);

    /// <summary>
    /// Updates an existing supplier in the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier to update.</param>
    /// <param name="dto">The data transfer object containing the updated supplier information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>True if the supplier was updated, otherwise false.</returns>
    Task<bool> UpdateAsync(int id, UpdateSupplierDto dto, CancellationToken ct);

    /// <summary>
    /// Deletes a supplier from the data store by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier to delete.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>True if the supplier was deleted, otherwise false.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}