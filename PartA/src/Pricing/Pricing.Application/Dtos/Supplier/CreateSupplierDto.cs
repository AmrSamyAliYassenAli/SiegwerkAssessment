namespace Pricing.Application.Dtos.Supplier;

/// <summary>
/// Represents a data transfer object for creating a new supplier.
/// </summary>
/// <param name="Name">The name of the supplier.</param>
/// <param name="Country">The country of the supplier.</param>
/// <param name="Active">A flag indicating whether the supplier is active.</param>
/// <param name="Preferred">A flag indicating whether the supplier is preferred.</param>
/// <param name="LeadTimeDays">The typical lead time in days for the supplier's products.</param>
public sealed record CreateSupplierDto(
    string Name, 
    string Country, 
    bool Active, 
    bool Preferred, 
    int LeadTimeDays);