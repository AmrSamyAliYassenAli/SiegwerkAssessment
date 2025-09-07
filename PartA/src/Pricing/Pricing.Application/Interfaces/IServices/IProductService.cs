namespace Pricing.Application.Interfaces.IServices;

/// <summary>
/// Provides a service layer for managing product-related business logic.
/// This class acts as an intermediary between the API controllers and the
/// product data repository, encapsulating business rules and workflows.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves all products from the data store.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A list of all products as <see cref="ProductDto"/> objects.</returns>
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The <see cref="ProductDto"/> if found, otherwise null.</returns>
    Task<ProductDto?> GetAsync(int id, CancellationToken ct);

    /// <summary>
    /// Creates a new product in the data store.
    /// </summary>
    /// <param name="dto">The data transfer object containing the new product information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The newly created product as a <see cref="ProductDto"/>.</returns>
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct);

    /// <summary>
    /// Updates an existing product in the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="dto">The data transfer object containing the updated product information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>True if the product was updated, otherwise false.</returns>
    Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct);

    /// <summary>
    /// Deletes a product from the data store by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>True if the product was deleted, otherwise false.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}