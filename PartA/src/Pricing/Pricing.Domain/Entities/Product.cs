namespace Pricing.Domain.Entities;

/// <summary>
/// Represents a product in the system.
/// This class holds details about a product, such as its SKU, name, and unit of measure.
/// It inherits from <see cref="BaseEntitiy"/> for common properties like Id and timestamps.
/// </summary>
public sealed class Product : BaseEntitiy
{
    /// <summary>
    /// Gets or sets the Stock Keeping Unit (SKU) for the product.
    /// This is a unique identifier for the product.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Unit of Measure (UOM) for the product. Defaults to "EA" (Each).
    /// </summary>
    public string Uom { get; set; } = "EA";

    /// <summary>
    /// Gets or sets the hazard class of the product.
    /// </summary>
    public string HazardClass { get; set; } = string.Empty;
}