namespace Pricing.Application.Interfaces.IRepositories;

/// <summary>
/// Implements the repository pattern for managing <see cref="Product"/> entities.
/// This class provides methods for querying, adding, updating, and deleting product data.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves a product DTO by its SKU.
    /// </summary>
    /// <param name="sku">The SKU of the product.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A ProductDto or null if no product is found.</returns>
    Task<ProductDto?> GetBySkuAsync(string sku, CancellationToken ct);

    /// <summary>
    /// Retrieves a product DTO by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the product.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A ProductDto or null if no product is found.</returns>
    Task<ProductDto?> GetAsync(int id, CancellationToken ct);

    /// <summary>
    /// Retrieves a list of all products, mapped to DTOs.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A list of ProductDto objects, ordered by SKU.</returns>
    Task<List<ProductDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Adds a new product to the database.
    /// </summary>
    /// <param name="createProductDto">The DTO containing the product data to add.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The newly added product, mapped to a DTO.</returns>
    Task<ProductDto> AddAsync(CreateProductDto createProductDto, CancellationToken ct);

    /// <summary>
    /// Updates an existing product in the database.
    /// </summary>
    /// <param name="updateProductDto">The DTO containing the product data to update.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A boolean indicating whether the update was successful (true) or if the product was not found (false).</returns>
    Task<bool> UpdateAsync(UpdateProductDto updateProductDto, CancellationToken ct);

    /// <summary>
    /// Deletes a product from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A boolean indicating whether the deletion was successful (true) or if the product was not found (false).</returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}