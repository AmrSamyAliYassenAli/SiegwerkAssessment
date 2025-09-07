namespace Pricing.Api.Endpoints;
/// <summary>
/// Provides a set of minimal API endpoints for managing suppliers.
/// </summary>
internal static class SuppliersEndpoints
{
    /// <summary>
    /// Maps the supplier-related API endpoints to the application's route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to which the endpoints will be mapped.</param>
    /// <returns>The updated endpoint route builder.</returns>
    internal static IEndpointRouteBuilder MapSuppliersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet()
          .WithTags("Suppliers")
          .WithName("GetSuppliers")
          .Produces<IEnumerable<SupplierDto>>(StatusCodes.Status200OK);

        app.MapGetById()
           .WithTags("Suppliers")
           .WithName("GetSupplierById")
           .Produces<SupplierDto>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound);

        app.MapPost()
           .WithTags("Suppliers")
           .WithName("CreateSupplier")
           .Produces<SupplierDto>(StatusCodes.Status201Created);

        app.MapPut()
           .WithTags("Suppliers")
           .WithName("UpdateSupplier")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status404NotFound);

        app.MapDelete()
           .WithTags("Suppliers")
           .WithName("DeleteSupplier")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Maps the GET endpoint to retrieve all suppliers.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapGet(this IEndpointRouteBuilder app)
        => app.MapGet("/suppliers", (SupplierApiService svc, CancellationToken ct) => svc.GetAllAsync(ct));

    /// <summary>
    /// Maps the GET endpoint to retrieve a supplier by its ID.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapGetById(this IEndpointRouteBuilder app)
        => app.MapGet("/suppliers/{id:int}", (int id, SupplierApiService svc, CancellationToken ct) => svc.GetByIdAsync(id, ct));

    /// <summary>
    /// Maps the POST endpoint to create a new supplier.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapPost(this IEndpointRouteBuilder app)
        => app.MapPost("/suppliers", (CreateSupplierDto dto, SupplierApiService svc, CancellationToken ct) => svc.CreateAsync(dto, ct));

    /// <summary>
    /// Maps the PUT endpoint to update an existing supplier.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapPut(this IEndpointRouteBuilder app)
        => app.MapPut("/suppliers", (UpdateSupplierDto dto, SupplierApiService svc, CancellationToken ct) => svc.UpdateAsync(dto.Id, dto, ct));

    /// <summary>
    /// Maps the DELETE endpoint to delete a supplier by its ID.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapDelete(this IEndpointRouteBuilder app)
        => app.MapDelete("/suppliers/{id:int}", (int id, SupplierApiService svc, CancellationToken ct) => svc.DeleteAsync(id, ct));
}