namespace Pricing.Api.Models.ErrorHandling;

/// <summary>
/// Represents a single validation error, typically used within a larger error response
/// to provide details about why a request was invalid.
/// </summary>
public sealed class ValidationError
{
    /// <summary>
    /// Gets or sets the name of the field or property that failed validation.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a human-readable message describing the validation failure.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value that was attempted to be assigned to the field,
    /// which may be null.
    /// </summary>
    public object? AttemptedValue { get; set; }
}