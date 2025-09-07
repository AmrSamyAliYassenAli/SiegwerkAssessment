namespace Pricing.Domain.Entities;

/// <summary>
/// Represents an entry in a price list, defining a specific price for a product from a supplier
/// for a given quantity and time period. Inherits from <see cref="BaseEntitiy"/> for common properties.
/// </summary>
public sealed class PriceListEntry : BaseEntitiy
{
    /// <summary>
    /// Gets or sets the ID of the supplier associated with this price list entry.
    /// This is a foreign key to the Supplier entity.
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// Gets or sets the Stock Keeping Unit (SKU) for the product.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the starting date for which this price is valid.
    /// </summary>
    public DateOnly ValidFrom { get; set; }

    /// <summary>
    /// Gets or sets the end date for which this price is valid.
    /// </summary>
    public DateOnly ValidTo { get; set; }

    /// <summary>
    /// Gets or sets the currency of the price. Defaults to "USD".
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets the price per unit of measure.
    /// </summary>
    public decimal PricePerUom { get; set; }

    /// <summary>
    /// Gets or sets the minimum quantity required to obtain this price.
    /// </summary>
    public int MinQty { get; set; }
}