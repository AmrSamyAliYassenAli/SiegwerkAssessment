namespace Pricing.Infrastructure.Repositories;

//<!inheritdoc/>
public sealed class SupplierRepository : ISupplierRepository
{
    private readonly PricingDbContext _pricingDbContext;
    public SupplierRepository(PricingDbContext pricingDbContext) 
        => _pricingDbContext = pricingDbContext;

    //<!inheritdoc/>
    public async Task<SupplierDto?> GetAsync(int id, CancellationToken ct)
    {
        var supplier = await _pricingDbContext
            .Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return supplier?.MapToDto();
    }

    //<!inheritdoc/>
    public async Task<List<SupplierDto>> GetAllAsync(CancellationToken ct) 
        => await _pricingDbContext
        .Suppliers
        .AsNoTracking()
        .OrderBy(s => s.Id)
        .Select(x => x.MapToDto())
        .ToListAsync(ct);

    //<!inheritdoc/>
    public async Task<SupplierDto> AddAsync(CreateSupplierDto entity, CancellationToken ct)
    {
        var s = _pricingDbContext.Suppliers.Add(entity.MapToEntity());
        await _pricingDbContext.SaveChangesAsync(ct);
        return s.Entity.MapToDto();
    }

    //<!inheritdoc/>
    public async Task<bool> UpdateAsync(UpdateSupplierDto entity, CancellationToken ct)
    {
        var exists = await _pricingDbContext.Suppliers.AnyAsync(s => s.Id == entity.Id, ct);

        if (!exists) return false;

        _pricingDbContext.Suppliers.Update(entity.MapToEntity());

        await _pricingDbContext.SaveChangesAsync(ct);

        return true;
    }

    //<!inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _pricingDbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id, ct);

        if (entity == null) return false;

        _pricingDbContext.Suppliers.Remove(entity);

        await _pricingDbContext.SaveChangesAsync(ct);

        return true;
    }
}