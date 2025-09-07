namespace Pricing.Api.Services;

/// <summary>
/// Provides a service layer for handling supplier-related API operations.
/// This service acts as a mediator between the API endpoints and the underlying data repository,
/// ensuring a clean separation of concerns.
/// </summary>
public sealed class SupplierApiService
{
    private readonly ISupplierService _supplierService;
    /// <summary>
    /// Initializes a new instance of the <see cref="SupplierApiService"/> class.
    /// </summary>
    /// <param name="supplierService">The supplier service for handling business logic related to suppliers.</param>
    public SupplierApiService(ISupplierService supplierService) => _supplierService = supplierService;

    /// <summary>
    /// Retrieves all suppliers from the data store.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>An <see cref="Ok{TValue}"/> result containing a list of <see cref="SupplierDto"/> objects.</returns>
    public async Task<Ok<IEnumerable<SupplierDto>>> GetAllAsync(CancellationToken ct) =>
        TypedResults.Ok(await _supplierService.GetAllAsync(ct));

    /// <summary>
    /// Retrieves a supplier by its unique identifier.
    /// </summary>
    /// <param name="id">The unique ID of the supplier.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Results{TResult1, TResult2}"/> with the <see cref="SupplierDto"/> if found, or a <see cref="NotFound"/> result.</returns>
    public async Task<Results<Ok<SupplierDto>, NotFound>> GetByIdAsync(int id, CancellationToken ct)
    {
        try
        {
            var s = await _supplierService.GetAsync(id, ct);
            return s is null ? TypedResults.NotFound() : TypedResults.Ok(s);
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    /// <summary>
    /// Creates a new supplier.
    /// </summary>
    /// <param name="dto">The data transfer object containing the supplier information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Created{TValue}"/> result containing the newly created <see cref="SupplierDto"/>.</returns>
    public async Task<Created<SupplierDto>> CreateAsync(CreateSupplierDto dto, CancellationToken ct)
    {
        try
        {
            var s = await _supplierService.CreateAsync(dto, ct);
            return TypedResults.Created($"/suppliers/{s.Id}", s);
        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// Updates an existing supplier.
    /// </summary>
    /// <param name="id">The ID of the supplier to update.</param>
    /// <param name="dto">The data transfer object containing the updated supplier information.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Results{TResult1, TResult2}"/> with a <see cref="NoContent"/> result if successful, or a <see cref="NotFound"/> result if the supplier does not exist.</returns>
    public async Task<Results<NoContent, NotFound>> UpdateAsync(int id, UpdateSupplierDto dto, CancellationToken ct)
    {
        try
        {
            var ok = await _supplierService.UpdateAsync(id, dto, ct);
            return ok ? TypedResults.NoContent() : TypedResults.NotFound();
        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// Deletes a supplier by its unique identifier.
    /// </summary>
    /// <param name="id">The unique ID of the supplier to delete.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A <see cref="Results{TResult1, TResult2}"/> with a <see cref="NoContent"/> result if successful, or a <see cref="NotFound"/> result if the supplier does not exist.</returns>
    public async Task<Results<NoContent, NotFound>> DeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            var ok = await _supplierService.DeleteAsync(id, ct);
            return ok ? TypedResults.NoContent() : TypedResults.NotFound();
        }
        catch (Exception)
        {

            throw;
        }
    }
}