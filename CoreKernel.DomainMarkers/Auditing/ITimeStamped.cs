namespace CoreKernel.DomainMarkers.Auditing;

/// <summary>
/// Interface for timestamped entities.
/// Provides properties to track the creation and last modification timestamps of an entity.
/// </summary>
public interface ITimeStamped
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedOn { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTimeOffset LastModifiedOn { get; set; }
}