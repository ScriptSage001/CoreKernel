namespace CoreKernel.DomainMarkers.Auditing;

/// <summary>
/// Base Interface for Auditable Entities.
/// Provides properties to track creation and modification metadata for an entity.
/// </summary>
public interface IAuditable : ITimeStamped
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public string LastModifiedBy { get; set; }
}