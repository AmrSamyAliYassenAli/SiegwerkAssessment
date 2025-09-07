using Microsoft.Extensions.Logging;

namespace Pricing.Infrastructure.Repositories;

//</inheritdoc />
public sealed class PriceListRepository : IPriceListRepository
{
    private readonly PricingDbContext _pricingDbContext;
    private readonly ILogger<PriceListRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriceListRepository"/> class.
    /// </summary>
    /// <param name="pricingDbContext">The database context.</param>
    public PriceListRepository(PricingDbContext pricingDbContext, ILogger<PriceListRepository> logger) => (_pricingDbContext, _logger) = (pricingDbContext, logger);

    //</inheritdoc />
    public async Task<PagedResult<PriceListDto>> ListAsync(PriceFilter filter, CancellationToken ct)
    {
        var q = _pricingDbContext.PriceListEntries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Sku))
            q = q.Where(x => x.Sku == filter.Sku);
        if (filter.ValidOn.HasValue)
        {
            var d = filter.ValidOn.Value;
            q = q.Where(x => x.ValidFrom <= d && d <= x.ValidTo);
        }
        if (!string.IsNullOrWhiteSpace(filter.Currency))
            q = q.Where(x => x.Currency == filter.Currency);
        if (filter.SupplierId.HasValue)
            q = q.Where(x => x.SupplierId == filter.SupplierId.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderBy(x => x.Sku).ThenBy(x => x.SupplierId).ThenBy(x => x.ValidFrom)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => x.MapToDto())
            .ToListAsync(ct);

        return new PagedResult<PriceListDto>(filter.Page, filter.PageSize, total, items);
    }

    //</inheritdoc />
    public async Task AddRangeAsync(IEnumerable<PriceListDto> priceListDto, CancellationToken ct)
    {
        try
        {
            var entities = priceListDto.MapToEntities();
            _pricingDbContext.PriceListEntries.AddRange(entities);
            await _pricingDbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error adding price list entries");
            throw;
        }
    }

    //</inheritdoc />
    public Task<bool> HasOverlapAsync(int supplierId, string sku, DateOnly from, DateOnly to, CancellationToken ct) =>
        _pricingDbContext.PriceListEntries.AsNoTracking().AnyAsync(x =>
            x.SupplierId == supplierId && x.Sku == sku && x.ValidFrom <= to && from <= x.ValidTo, ct);

    //</inheritdoc />
    public async Task<List<(PriceListDto priceListDto, SupplierDto Supplier)>> GetCandidatesAsync(string sku, int quantity, DateOnly date, CancellationToken ct)
    {
        var res = await (from p in _pricingDbContext.PriceListEntries.AsNoTracking()
                         join s in _pricingDbContext.Suppliers.AsNoTracking() on p.SupplierId equals s.Id
                         where p.Sku == sku && p.ValidFrom <= date && date <= p.ValidTo && p.MinQty <= quantity && s.Active
                         select new
                         {
                             Entry = p.MapToDto(),
                             Supplier = s.MapToDto()
                         }).ToListAsync(ct);

        return res.Select(x => (x.Entry, x.Supplier)).ToList();
    }
}