namespace Pricing.Infrastructure.Providers;

//<!inheritdoc />
public sealed class StaticRateProvider : IRateProvider
{

    /// <summary>
    /// Base to EUR and USD as examples; extend as necessary.
    /// </summary>
    private readonly Dictionary<string, decimal> _toEur = new(StringComparer.OrdinalIgnoreCase)
    {
        { "EUR", 1m }, { "USD", 0.92m }, { "GBP", 1.17m }
    };

    //<!inheritdoc />
    public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
    {
        if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            return amount;

        if (!_toEur.TryGetValue(fromCurrency, out var fromToEur))
            throw new InvalidOperationException($"Unknown currency: {fromCurrency}");
        if (!_toEur.TryGetValue(toCurrency, out var toToEur))
            throw new InvalidOperationException($"Unknown currency: {toCurrency}");

        // amount_in_eur = amount * fromToEur
        var inEur = amount * fromToEur;
        // amount_in_target = amount_in_eur / toToEur
        var inTarget = inEur / toToEur;
        return Math.Round(inTarget, 4, MidpointRounding.AwayFromZero);
    }
}
