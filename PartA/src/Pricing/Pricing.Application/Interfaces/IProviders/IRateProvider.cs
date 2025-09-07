namespace Pricing.Application.Interfaces.IProviders;

/// <summary>
/// Provides static currency conversion rates for a limited set of currencies.
/// This provider performs conversions to and from a base currency (EUR) as an intermediary step.
/// </summary>
public interface IRateProvider
{
    /// <summary>
    /// Converts an amount from one currency to another using predefined static rates.
    /// </summary>
    /// <param name="amount">The amount of money to convert.</param>
    /// <param name="fromCurrency">The currency to convert from.</param>
    /// <param name="toCurrency">The currency to convert to.</param>
    /// <returns>The converted amount, rounded to 4 decimal places.</returns>
    /// <exception cref="InvalidOperationException">Thrown if either the source or target currency is not supported.</exception>
    decimal Convert(decimal amount, string fromCurrency, string toCurrency);
}