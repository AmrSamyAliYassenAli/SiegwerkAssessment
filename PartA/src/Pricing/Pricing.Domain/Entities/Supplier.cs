namespace Pricing.Domain.Entities;

/// <summary>
/// Represents a supplier in the system.
/// This class holds details about a supplier, such as its name, country, and status.
/// It inherits from <see cref="BaseEntitiy"/> for common properties like Id and timestamps.
/// </summary>
public sealed class Supplier : BaseEntitiy
{
    /// <summary>
    /// Gets or sets the name of the supplier.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country where the supplier is located.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the supplier is active.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the supplier is preferred.
    /// </summary>
    public bool Preferred { get; set; }

    /// <summary>
    /// Gets or sets the lead time in days for delivery from this supplier.
    /// </summary>
    public int LeadTimeDays { get; set; }
}
