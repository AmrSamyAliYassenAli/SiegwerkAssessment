namespace Pricing.Infrastructure.Services;

/// <summary>
/// A decorator for <see cref="IBestPriceService"/> that adds a caching layer.
/// This service uses <see cref="IMemoryCache"/> to store query results,
/// reducing redundant calls to the underlying service and improving performance.
/// </summary>
public class CachedBestPriceService : IBestPriceService
{
    private readonly IBestPriceService _inner;
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedBestPriceService"/> class.
    /// </summary>
    /// <param name="inner">The inner service to which caching is being applied.</param>
    /// <param name="memoryCache">The memory cache instance used for storing results.</param>
    public CachedBestPriceService(IBestPriceService inner, IMemoryCache memoryCache)
    {
        _inner = inner;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Queries for the best price, first checking the memory cache.
    /// If the result is not in the cache, it calls the inner service, stores the result, and then returns it.
    /// </summary>
    /// <param name="sku">The SKU of the product.</param>
    /// <param name="quantity">The desired quantity of the product.</param>
    /// <param name="currency">The desired currency for the final price.</param>
    /// <param name="date">The date on which the price should be valid.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="BestPriceResultDto"/> representing the best price, or null if no suitable price is found.</returns>
    public Task<BestPriceResultDto?> QueryBestPriceAsync(string sku, int quantity, string currency, DateOnly date, CancellationToken ct)
    {
        var key = $"best:{sku}:{quantity}:{currency}:{date:yyyyMMdd}";
        return _memoryCache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = Ttl;
            return await _inner.QueryBestPriceAsync(sku, quantity, currency, date, ct);
        })!;
    }
}