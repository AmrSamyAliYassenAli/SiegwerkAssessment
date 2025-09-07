namespace Pricing.Application.Dtos.Product;

/// <summary>
/// Represents a data transfer object for a product.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Sku">The SKU (Stock Keeping Unit) of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Uom">The unit of measure for the product (e.g., "EA" for Each).</param>
/// <param name="HazardClass">The hazard class of the product, if applicable.</param>
public sealed record ProductDto(
    int Id, 
    string Sku, 
    string Name, 
    string Uom, 
    string HazardClass);