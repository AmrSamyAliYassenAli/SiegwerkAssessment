namespace Pricing.Infrastructure.Mappers;

/// <summary>
/// Provides static extension methods for mapping between <see cref="Product"/> entities and their DTOs.
/// </summary>
public static class ProductMapper
{
    /// <summary>
    /// Maps a <see cref="Product"/> entity to a <see cref="ProductDto"/> DTO.
    /// </summary>
    /// <param name="product">The product entity to map.</param>
    /// <returns>A new <see cref="ProductDto"/> instance.</returns>
    public static ProductDto MapToDto(this Product product)
        => new(product.Id, product.Sku, product.Name, product.Uom, product.HazardClass);

    /// <summary>
    /// Maps a <see cref="CreateProductDto"/> DTO to a new <see cref="Product"/> entity.
    /// </summary>
    /// <param name="dto">The DTO to map.</param>
    /// <returns>A new <see cref="Product"/> entity instance.</returns>
    public static Product MapToEntity(this CreateProductDto dto) => new Product
    {
        Sku = dto.Sku,
        Name = dto.Name,
        Uom = dto.Uom,
        HazardClass = dto.HazardClass
    };

    /// <summary>
    /// Maps an <see cref="UpdateProductDto"/> DTO to a new <see cref="Product"/> entity.
    /// This method is useful for creating a new entity based on update data.
    /// </summary>
    /// <param name="dto">The DTO to map.</param>
    /// <returns>A new <see cref="Product"/> entity instance with the updated data.</returns>
    public static Product MapToEntity(this UpdateProductDto dto) => new Product
    {
        Id = dto.Id,
        Sku = dto.Sku,
        Name = dto.Name,
        Uom = dto.Uom,
        HazardClass = dto.HazardClass
    };
}
