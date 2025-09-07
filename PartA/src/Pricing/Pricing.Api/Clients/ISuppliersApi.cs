namespace Pricing.Api.Clients;

public interface ISuppliersApi
{
    [Get("/suppliers")]
    Task<IEnumerable<SupplierDto>> GetAllAsync(CancellationToken ct = default);

    [Get("/suppliers/{id}")]
    Task<SupplierDto> GetByIdAsync(int id, CancellationToken ct = default);

    [Post("/suppliers")]
    Task<SupplierDto> CreateAsync([Body] CreateSupplierDto dto, CancellationToken ct = default);

    [Put("/suppliers/{id}")]
    Task UpdateAsync(int id, [Body] UpdateSupplierDto dto, CancellationToken ct = default);

    [Delete("/suppliers/{id}")]
    Task DeleteAsync(int id, CancellationToken ct = default);
}