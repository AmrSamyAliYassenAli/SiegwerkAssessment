namespace Pricing.Domain.Entities;

/// <summary>
/// An abstract base class for database entities.
/// This class provides common properties for all entities,
/// such as an identifier and creation/modification timestamps.
/// </summary>
public abstract class BaseEntitiy
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// This property is typically the primary key in the database table.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for when the entity was created.
    /// It is automatically set to the current UTC time.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp for the last time the entity was updated.
    /// It is automatically set to the current UTC time upon creation.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}