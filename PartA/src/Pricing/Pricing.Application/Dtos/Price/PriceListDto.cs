namespace Pricing.Application.Dtos.Price;

/// <summary>
/// Represents a data transfer object for a price list entry.
/// </summary>
/// <param name="Id">The unique identifier of the price list entry. Optional for creation.</param>
/// <param name="SupplierId">The unique identifier of the supplier.</param>
/// <param name="Sku">The SKU (Stock Keeping Unit) of the product.</param>
/// <param name="ValidFrom">The start date of the price's validity period.</param>
/// <param name="ValidTo">The end date of the price's validity period.</param>
/// <param name="Currency">The currency of the price.</param>
/// <param name="PricePerUom">The price per unit of measure.</param>
/// <param name="MinQty">The minimum quantity required to get this price.</param>
public sealed record PriceListDto(
    int SupplierId, 
    string Sku, 
    DateOnly ValidFrom, 
    DateOnly ValidTo,
    string Currency, 
    decimal PricePerUom, 
    int MinQty,
    int? Id = null);