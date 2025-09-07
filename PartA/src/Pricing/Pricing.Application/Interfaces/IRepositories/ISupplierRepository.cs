namespace Pricing.Application.Interfaces.IRepositories;

/// <summary>
/// Implements the repository pattern for managing supplier data in the database.
/// This class encapsulates data access logic for the Supplier entity.
/// </summary>
public interface ISupplierRepository
{
    /// <summary>
    /// Retrieves a Supplier DTO by its ID.
    /// </summary>
    /// <param name="id">The ID of the supplier.</param>
    /// <param name="ct">The CancellationToken.</param>
    /// <returns>A SupplierDto or null if not found.</returns>
    Task<SupplierDto?> GetAsync(int id, CancellationToken ct);

    /// <summary>
    /// Retrieves a list of all suppliers.
    /// </summary>
    /// <param name="ct">The CancellationToken.</param>
    /// <returns>A list of Supplier DTOs.</returns>
    Task<List<SupplierDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Adds a new supplier to the database.
    /// </summary>
    /// <param name="entity">The DTO containing the new supplier's data.</param>
    /// <param name="ct">The CancellationToken.</param>
    /// <returns>The newly created Supplier DTO.</returns>
    Task<SupplierDto> AddAsync(CreateSupplierDto entity, CancellationToken ct);

    /// <summary>
    /// Updates an existing supplier in the database.
    /// </summary>
    /// <param name="entity">The DTO containing the updated supplier data.</param>
    /// <param name="ct">The CancellationToken.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    Task<bool> UpdateAsync(UpdateSupplierDto entity, CancellationToken ct);

    /// <summary>
    /// Deletes a supplier from the database by ID.
    /// </summary>
    /// <param name="id">The ID of the supplier to delete.</param>
    /// <param name="ct">The CancellationToken.</param>
    /// <returns>True if the deletion was successful, otherwise false.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}