namespace Pricing.Application.Dtos.Product;

/// <summary>
/// Represents a data transfer object for updating an existing product.
/// </summary>
/// <param name="Id">The unique identifier of the product to update.</param>
/// <param name="Sku">The updated SKU of the product.</param>
/// <param name="Name">The updated name of the product.</param>
/// <param name="Uom">The updated unit of measure for the product.</param>
/// <param name="HazardClass">The updated hazard class of the product.</param>
public sealed record UpdateProductDto(
    int Id,
    string Sku, 
    string Name, 
    string Uom, 
    string HazardClass);