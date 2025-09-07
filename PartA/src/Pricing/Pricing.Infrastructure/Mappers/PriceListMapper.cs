namespace Pricing.Infrastructure.Mappers;

/// <summary>
/// Provides static extension methods for mapping between <see cref="PriceListEntry"/> entities and <see cref="PriceListDto"/> DTOs.
/// </summary>
internal static class PriceListMapper
{
    /// <summary>
    /// Maps a <see cref="PriceListEntry"/> entity to a <see cref="PriceListDto"/> DTO.
    /// </summary>
    /// <param name="priceListEntryDto">The entity to map.</param>
    /// <returns>A new <see cref="PriceListDto"/> instance.</returns>
    internal static PriceListDto MapToDto(this PriceListEntry priceListEntryDto)
        => new PriceListDto
        (
            Id: priceListEntryDto.Id,
            SupplierId: priceListEntryDto.SupplierId,
            Sku: priceListEntryDto.Sku,
            ValidFrom: priceListEntryDto.ValidFrom,
            ValidTo: priceListEntryDto.ValidTo,
            Currency: priceListEntryDto.Currency,
            PricePerUom: priceListEntryDto.PricePerUom,
            MinQty: priceListEntryDto.MinQty
        );

    /// <summary>
    /// Maps a <see cref="PriceListDto"/> DTO to a <see cref="PriceListEntry"/> entity.
    /// </summary>
    /// <param name="priceListEntryDto">The DTO to map.</param>
    /// <returns>A new <see cref="PriceListEntry"/> instance.</returns>
    internal static PriceListEntry MapToEntity(this PriceListDto priceListEntryDto)
        => new PriceListEntry
        {
            Id = priceListEntryDto.Id!.Value,
            SupplierId = priceListEntryDto.SupplierId,
            Sku = priceListEntryDto.Sku,
            ValidFrom = priceListEntryDto.ValidFrom,
            ValidTo = priceListEntryDto.ValidTo,
            Currency = priceListEntryDto.Currency,
            PricePerUom = priceListEntryDto.PricePerUom,
            MinQty = priceListEntryDto.MinQty
        };

    /// <summary>
    /// Maps a collection of <see cref="PriceListDto"/> DTOs to a collection of <see cref="PriceListEntry"/> entities.
    /// </summary>
    /// <param name="priceListEntryDtos">The collection of DTOs to map.</param>
    /// <returns>An enumerable collection of <see cref="PriceListEntry"/> entities.</returns>
    internal static IEnumerable<PriceListEntry> MapToEntities(this IEnumerable<PriceListDto> priceListEntryDtos)
    {
        List<PriceListEntry> entities = new List<PriceListEntry>();

        priceListEntryDtos.ForEach(x => entities.Add(x.MapToEntity()));

        return entities.AsEnumerable();
    }
}
