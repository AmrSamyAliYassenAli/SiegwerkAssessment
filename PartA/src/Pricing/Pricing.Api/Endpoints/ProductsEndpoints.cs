namespace Pricing.Api.Endpoints;
/// <summary>
/// Provides a set of minimal API endpoints for managing products.
/// </summary>
public static class ProductsEndpoints
{
    /// <summary>
    /// Maps the product-related API endpoints to the application's route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to which the endpoints will be mapped.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet()
            .WithTags("Products")
            .WithName("GetProducts")
            .Produces<IEnumerable<ProductDto>>(StatusCodes.Status200OK);

        app.MapGetById()
           .WithTags("Products")
           .WithName("GetProductById")
           .Produces<ProductDto>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound);

        app.MapPost()
           .WithTags("Products")
           .WithName("CreateProduct")
           .Produces<ProductDto>(StatusCodes.Status201Created);

        app.MapPut()
           .WithTags("Products")
           .WithName("UpdateProducts")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status404NotFound);

        app.MapDelete()
           .WithTags("Products")
           .WithName("DeleteProducts")
           .Produces(StatusCodes.Status204NoContent)
           .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Maps the GET endpoint to retrieve all products.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapGet(this IEndpointRouteBuilder app)
        => app.MapGet("/products", (ProductApiService svc, CancellationToken ct) => svc.GetAllAsync(ct));

    /// <summary>
    /// Maps the GET endpoint to retrieve a product by its ID.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapGetById(this IEndpointRouteBuilder app)
        => app.MapGet("/products/{id:int}", (int id, ProductApiService svc, CancellationToken ct) => svc.GetByIdAsync(id, ct));

    /// <summary>
    /// Maps the POST endpoint to create a new product.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapPost(this IEndpointRouteBuilder app)
        => app.MapPost("/products", (CreateProductDto dto, ProductApiService svc, CancellationToken ct) => svc.CreateAsync(dto, ct));

    /// <summary>
    /// Maps the PUT endpoint to update an existing product.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapPut(this IEndpointRouteBuilder app)
        => app.MapPut("/products/{id:int}", (int id, UpdateProductDto dto, ProductApiService svc, CancellationToken ct) => svc.UpdateAsync(id, dto, ct));

    /// <summary>
    /// Maps the DELETE endpoint to delete a product by its ID.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> for the endpoint.</returns>
    private static RouteHandlerBuilder MapDelete(this IEndpointRouteBuilder app)
        => app.MapDelete("/products/{id:int}", (int id, ProductApiService svc, CancellationToken ct) => svc.DeleteAsync(id, ct));
}