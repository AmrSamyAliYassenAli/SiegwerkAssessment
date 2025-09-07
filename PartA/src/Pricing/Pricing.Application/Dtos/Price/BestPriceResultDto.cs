namespace Pricing.Application.Dtos.Price;

/// <summary>
/// Represents the best-priced result for a product query, including details
/// about the winning supplier and the calculated price.
/// </summary>
/// <param name="Sku">The SKU of the product.</param>
/// <param name="Quantity">The quantity of the product being queried.</param>
/// <param name="Currency">The currency of the final price.</param>
/// <param name="Date">The date on which the price is valid.</param>
/// <param name="SupplierId">The ID of the supplier offering the best price.</param>
/// <param name="UnitPrice">The unit price of the product from the winning supplier.</param>
/// <param name="TotalPrice">The total calculated price for the specified quantity.</param>
/// <param name="Reason">A detailed explanation of why this supplier was chosen as the best option.</param>
public record BestPriceResultDto(
    string Sku,
    int Quantity,
    string Currency,
    DateOnly Date,
    int SupplierId,
    decimal UnitPrice,
    decimal TotalPrice,
    string Reason);