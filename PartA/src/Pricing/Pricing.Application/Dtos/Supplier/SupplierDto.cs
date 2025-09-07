namespace Pricing.Application.Dtos.Supplier;

/// <summary>
/// Represents a data transfer object for a supplier.
/// </summary>
/// <param name="Id">The unique identifier of the supplier.</param>
/// <param name="Name">The name of the supplier.</param>
/// <param name="Country">The country of the supplier.</param>
/// <param name="Active">A flag indicating whether the supplier is active.</param>
/// <param name="Preferred">A flag indicating whether the supplier is preferred.</param>
/// <param name="LeadTimeDays">The typical lead time in days for the supplier's products.</param>
public sealed record SupplierDto(
    int Id, 
    string Name, 
    string Country, 
    bool Active, 
    bool Preferred, 
    int LeadTimeDays);