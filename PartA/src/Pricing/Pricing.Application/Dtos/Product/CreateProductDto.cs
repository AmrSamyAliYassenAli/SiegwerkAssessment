namespace Pricing.Application.Dtos.Product;

/// <summary>
/// Represents a data transfer object for creating a new product.
/// </summary>
/// <param name="Sku">The SKU (Stock Keeping Unit) of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Uom">The unit of measure for the product (e.g., "EA" for Each).</param>
/// <param name="HazardClass">The hazard class of the product, if applicable.</param>
public record CreateProductDto(
    string Sku, 
    string Name, 
    string Uom, 
    string HazardClass);