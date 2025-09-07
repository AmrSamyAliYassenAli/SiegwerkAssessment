namespace Pricing.Infrastructure.Repositories;

//<!inheritdoc />
public sealed class ProductRepository : IProductRepository
{
    private readonly PricingDbContext _pricingDbContext;

    //<!inheritdoc />
    public ProductRepository(PricingDbContext pricingDbContext) => _pricingDbContext = pricingDbContext;

    //<!inheritdoc />
    public async Task<ProductDto?> GetBySkuAsync(string sku, CancellationToken ct)
        => await _pricingDbContext
        .Products
        .AsNoTracking()
        .Select(p => p.MapToDto())
        .FirstOrDefaultAsync(p => p.Sku == sku, ct);

    //<!inheritdoc />
    public async Task<ProductDto?> GetAsync(int id, CancellationToken ct)
    {
        try
        {
            var p = await _pricingDbContext
            .Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

            return p?.MapToDto();
        }
        catch (Exception)
        {

            throw;
        }
    }

    //<!inheritdoc />
    public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct)
        => await _pricingDbContext
        .Products
        .AsNoTracking()
        .OrderBy(p => p.Sku)
        .Select(p => p.MapToDto())
        .ToListAsync(ct);

    //<!inheritdoc />
    public async Task<ProductDto> AddAsync(CreateProductDto createProductDto, CancellationToken ct)
    {
        try
        {
            var p = _pricingDbContext.Products.Add(createProductDto.MapToEntity());

            await _pricingDbContext.SaveChangesAsync(ct);

            return p.Entity.MapToDto();
        }
        catch (Exception)
        {

            throw;
        }
    }

    //<!inheritdoc />
    public async Task<bool> UpdateAsync(UpdateProductDto updateProductDto, CancellationToken ct)
    {
        var exists = await _pricingDbContext.Products.AnyAsync(p => p.Id == updateProductDto.Id, ct);

        if (!exists) return false;

        _pricingDbContext.Products.Update(updateProductDto.MapToEntity());

        await _pricingDbContext.SaveChangesAsync(ct);

        return true;
    }

    //<!inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _pricingDbContext.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        if (entity == null) return false;

        _pricingDbContext.Products.Remove(entity);

        await _pricingDbContext.SaveChangesAsync(ct);

        return true;
    }
}