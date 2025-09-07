namespace Pricing.Api.Endpoints;

/// <summary>
/// Provides a set of minimal API endpoints for managing and querying pricing data.
/// </summary>
internal static class PricesEndpoints
{
    /// <summary>
    /// Maps the pricing-related API endpoints to the application's route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to which the endpoints will be mapped.</param>
    /// <returns>The updated endpoint route builder.</returns>
    internal static IEndpointRouteBuilder MapPricesEndpoints(this IEndpointRouteBuilder app)
    {
        /// <summary>
        /// Uploads a price list from a CSV file.
        /// </summary>
        /// <remarks>
        /// This endpoint accepts a multipart form data request containing a single CSV file.
        /// The file is processed and its contents are added to the price list database.
        /// </remarks>
        /// <param name="file">The uploaded CSV file.</param>
        /// <param name="svc">The price list service for handling the upload logic.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>
        /// An <see cref="Ok"/> result if the upload is successful, otherwise a <see cref="ProblemHttpResult"/> with details about the failure.
        /// </returns>
        app.MapPost("/prices/upload", async Task<Results<Ok, ProblemHttpResult>> (
          [FromForm] IFormFile file,
          IPriceListService svc,
          ILogger<Program> logger,
          CancellationToken ct) =>
        {
            try
            {
                if (file is null || file.Length == 0)
                    return TypedResults.Problem(
                        title: "No file uploaded.",
                        detail: "A file must be provided to this endpoint.",
                        statusCode: (int)HttpStatusCode.BadRequest);

                using var stream = file.OpenReadStream();
                var result = await svc.UploadCsvAsync(stream, ct);

                // Assuming the upload is successful, return an Ok result.
                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes before returning a generic problem result.
                logger.LogError(ex, "An unhandled exception occurred during file upload.");
                return TypedResults.Problem(
                    title: "Internal Server Error",
                    detail: "An unexpected error occurred during the file upload process.",
                    statusCode: (int)HttpStatusCode.InternalServerError);
            }
        })
      .DisableAntiforgery()
      .WithMetadata(new ConsumesAttribute("multipart/form-data"))
      .WithOpenApi(op =>
      {
          op.Summary = "Upload price list CSV";
          op.RequestBody = new OpenApiRequestBody
          {
              Required = true,
              Content = new Dictionary<string, OpenApiMediaType>
              {
                  ["multipart/form-data"] = new OpenApiMediaType
                  {
                      Schema = new OpenApiSchema
                      {
                          Type = "object",
                          Properties = new Dictionary<string, OpenApiSchema>
                          {
                              ["file"] = new OpenApiSchema { Type = "string", Format = "binary" }
                          },
                          Required = new HashSet<string> { "file" }
                      }
                  }
              }
          };
          return op;
      })
      .WithTags("Prices")
      .WithName("Upload");

        /// <summary>
        /// Retrieves a paginated and filtered list of price entries.
        /// </summary>
        /// <param name="sku">The SKU to filter by.</param>
        /// <param name="validOn">The date on which the prices should be valid.</param>
        /// <param name="currency">The currency to filter by.</param>
        /// <param name="supplierId">The supplier ID to filter by.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="svc">The price list service for retrieving data.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A paged result containing the filtered price entries.</returns>
        app.MapGet("/prices", (string? sku, DateOnly? validOn, string? currency, int? supplierId, int page, int pageSize, PriceListApiService svc, CancellationToken ct)
            => svc.ListAsync(sku, validOn, currency, supplierId, page, pageSize, ct))
          .WithTags("Prices")
          .WithName("Get");

        /// <summary>
        /// Queries for the best price for a given SKU and quantity on a specific date.
        /// </summary>
        /// <param name="sku">The SKU of the product.</param>
        /// <param name="qty">The quantity of the product.</param>
        /// <param name="currency">The target currency for the price.</param>
        /// <param name="date">The date for which the price should be valid.</param>
        /// <param name="svc">The price list API service for querying the best price.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The best price result or a not found result if no price is available.</returns>
        app.MapGet("/pricing/best", (string sku, int qty, string currency, DateOnly date, PriceListApiService svc, CancellationToken ct)
            => svc.BestAsync(sku, qty, currency, date, ct))
          .WithTags("Prices")
          .WithName("Best");

        return app;
    }
}