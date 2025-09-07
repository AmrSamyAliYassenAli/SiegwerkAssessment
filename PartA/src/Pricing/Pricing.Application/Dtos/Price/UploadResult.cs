namespace Pricing.Application.Dtos.Price;

/// <summary>
/// Represents the result of a file upload operation.
/// </summary>
/// <param name="IsSuccess">A flag indicating whether the upload was successful.</param>
/// <param name="Title">A summary of the result (e.g., "Invalid file" or "Success").</param>
/// <param name="Detail">Detailed information about the result, such as a specific error message.</param>
public record UploadResult(
    bool IsSuccess, 
    string? Title = null, 
    string? Detail = null);