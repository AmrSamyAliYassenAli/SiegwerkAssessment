namespace Pricing.Infrastructure.Services;

//<!inheritdoc/>
public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupplierService"/> class.
    /// </summary>
    /// <param name="supplierRepository">The repository for accessing supplier data.</param>
    public SupplierService(ISupplierRepository supplierRepository) => _supplierRepository = supplierRepository;

    //<!inheritdoc/>
    public async Task<IEnumerable<SupplierDto>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            return await _supplierRepository.GetAllAsync(ct);
        }
        catch (Exception)
        {

            throw;
        }
    }

    //<!inheritdoc/>
    public async Task<SupplierDto?> GetAsync(int id, CancellationToken ct)
    {
        try
        {
            return await _supplierRepository.GetAsync(id, ct);
        }
        catch (Exception)
        {

            throw;
        }
    }

    //<!inheritdoc/>
    public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto, CancellationToken ct)
    {
        try
        {
            return await _supplierRepository.AddAsync(dto, ct);
        }
        catch (Exception)
        {

            throw;
        }
    }

    //<!inheritdoc/>
    public async Task<bool> UpdateAsync(int id, UpdateSupplierDto dto, CancellationToken ct)
    {
        try
        {
            return await _supplierRepository.UpdateAsync(dto, ct);
        }
        catch (Exception)
        {

            throw;
        }
    }

    //<!inheritdoc/>
    public Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            return _supplierRepository.DeleteAsync(id, ct);
        }
        catch (Exception)
        {

            throw;
        }
    }
}