namespace Pricing.Application.Dtos.Supplier;

/// <summary>
/// Represents a data transfer object for updating an existing supplier.
/// </summary>
/// <param name="Id">The unique identifier of the supplier to update.</param>
/// <param name="Name">The updated name of the supplier.</param>
/// <param name="Country">The updated country of the supplier.</param>
/// <param name="Active">The updated active status of the supplier.</param>
/// <param name="Preferred">The updated preferred status of the supplier.</param>
/// <param name="LeadTimeDays">The updated lead time in days for the supplier.</param>
public sealed record UpdateSupplierDto(
    int Id,
    string Name, 
    string Country, 
    bool Active, 
    bool Preferred, 
    int LeadTimeDays);