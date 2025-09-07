namespace Pricing.Api.Services;

/// <summary>
/// Provides a service layer for handling product-related API operations.
/// This service acts as a mediator between the API endpoints and the underlying data repository,
/// ensuring a clean separation of concerns.
/// </summary>
public class ProductApiService
{
    private readonly IProductService _productService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductApiService"/> class.
    /// </summary>
    /// <param name="productService">The product service for handling business logic related to products.</param>
    public ProductApiService(IProductService productService) => _productService = productService;

    /// <summary>
    /// Retrieves all products from the data store.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An <see cref="Ok{TValue}"/> result containing a list of <see cref="ProductDto"/> objects.</returns>
    public async Task<Ok<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken ct) =>
        TypedResults.Ok(await _productService.GetAllAsync(ct));

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique ID of the product.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Results{TResult1, TResult2}"/> with the <see cref="ProductDto"/> if found, or a <see cref="NotFound"/> result.</returns>
    public async Task<Results<Ok<ProductDto>, NotFound>> GetByIdAsync(int id, CancellationToken ct)
    {
        var p = await _productService.GetAsync(id, ct);
        return p is null ? TypedResults.NotFound() : TypedResults.Ok(p);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="dto">The data transfer object containing the product information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Created{TValue}"/> result containing the newly created <see cref="ProductDto"/>.</returns>
    public async Task<Created<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken ct)
    {
        var p = await _productService.CreateAsync(dto, ct);
        return TypedResults.Created($"/products/{p.Id}", p);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The ID of the product to update.</param>
    /// <param name="dto">The data transfer object containing the updated product information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Results{TResult1, TResult2}"/> with a <see cref="NoContent"/> result if successful, or a <see cref="NotFound"/> result if the product does not exist.</returns>
    public async Task<Results<NoContent, NotFound>> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct)
    {
        var ok = await _productService.UpdateAsync(id, dto, ct);
        return ok ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    /// <summary>
    /// Deletes a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique ID of the product to delete.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Results{TResult1, TResult2}"/> with a <see cref="NoContent"/> result if successful, or a <see cref="NotFound"/> result if the product does not exist.</returns>
    public async Task<Results<NoContent, NotFound>> DeleteAsync(int id, CancellationToken ct)
    {
        var ok = await _productService.DeleteAsync(id, ct);
        return ok ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}