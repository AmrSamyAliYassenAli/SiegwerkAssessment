namespace Pricing.Infrastructure.Mappers;

/// <summary>
/// Provides static extension methods for mapping between <see cref="Supplier"/> entities and their DTOs.
/// </summary>
public static class SupplierMapper
{
    /// <summary>
    /// Maps a <see cref="Supplier"/> entity to a <see cref="SupplierDto"/> DTO.
    /// </summary>
    /// <param name="supplier">The supplier entity to map.</param>
    /// <returns>A new <see cref="SupplierDto"/> instance.</returns>
    public static SupplierDto MapToDto(this Supplier supplier)
        => new(
            supplier.Id,
            supplier.Name,
            supplier.Country,
            supplier.Active,
            supplier.Preferred,
            supplier.LeadTimeDays);

    /// <summary>
    /// Maps a <see cref="CreateSupplierDto"/> DTO to a new <see cref="Supplier"/> entity.
    /// </summary>
    /// <param name="dto">The DTO to map.</param>
    /// <returns>A new <see cref="Supplier"/> entity instance.</returns>
    public static Supplier MapToEntity(this CreateSupplierDto dto) => new Supplier
    {
        Name = dto.Name,
        Country = dto.Country,
        Active = dto.Active,
        Preferred = dto.Preferred,
        LeadTimeDays = dto.LeadTimeDays
    };

    /// <summary>
    /// Maps an <see cref="UpdateSupplierDto"/> DTO to a new <see cref="Supplier"/> entity.
    /// This method is useful for creating an entity from update data.
    /// </summary>
    /// <param name="dto">The DTO to map.</param>
    /// <returns>A new <see cref="Supplier"/> entity instance with the updated data.</returns>
    public static Supplier MapToEntity(this UpdateSupplierDto dto) => new Supplier
    {
        Id = dto.Id,
        Name = dto.Name,
        Country = dto.Country,
        Active = dto.Active,
        Preferred = dto.Preferred,
        LeadTimeDays = dto.LeadTimeDays
    };
}