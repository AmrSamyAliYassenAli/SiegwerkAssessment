namespace Pricing.Api.Clients;

public interface IProductsApi
{
    [Get("/products")]
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default);


    [Get("/products/{id}")]
    Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default);


    [Post("/products")]
    Task<ProductDto> CreateAsync([Body] CreateProductDto dto, CancellationToken ct = default);


    [Put("/products/{id}")]
    Task UpdateAsync(int id, [Body] UpdateProductDto dto, CancellationToken ct = default);


    [Delete("/products/{id}")]
    Task DeleteAsync(int id, CancellationToken ct = default);
}