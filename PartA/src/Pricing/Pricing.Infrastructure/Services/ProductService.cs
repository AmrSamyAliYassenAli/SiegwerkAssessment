namespace Pricing.Infrastructure.Services;

//<!inheritdoc/>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService"/> class.
    /// </summary>
    /// <param name="productRepository">The repository for accessing product data.</param>
    public ProductService(IProductRepository productRepository) 
        => _productRepository = productRepository;

    //<!inheritdoc/>
    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct)
        => await _productRepository.GetAllAsync(ct);

    //<!inheritdoc/>
    public async Task<ProductDto?> GetAsync(int id, CancellationToken ct)
        => await _productRepository.GetAsync(id, ct);

    //<!inheritdoc/>
    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct)
        => await _productRepository.AddAsync(dto, ct);

    //<!inheritdoc/>
    public Task<bool> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct) =>
        _productRepository.UpdateAsync(dto, ct);

    //<!inheritdoc/>
    public Task<bool> DeleteAsync(int id, CancellationToken ct) => _productRepository.DeleteAsync(id, ct);
}