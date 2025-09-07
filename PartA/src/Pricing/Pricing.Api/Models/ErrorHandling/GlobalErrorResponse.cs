namespace Pricing.Api.Models.ErrorHandling;

/// <summary>
/// Represents a standardized format for a global error response.
/// This model provides a consistent structure for returning error details,
/// including a timestamp, the request path, the HTTP method, and a nested
/// object containing detailed error information.
/// </summary>
public sealed class GlobalErrorResponse
{
    /// <summary>
    /// Gets or sets the detailed error information, typically conforming to RFC 7807.
    /// </summary>
    public ErrorDetails Error { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp when the error occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the original request path that caused the error.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTTP method of the original request.
    /// </summary>
    public string Method { get; set; } = string.Empty;
}