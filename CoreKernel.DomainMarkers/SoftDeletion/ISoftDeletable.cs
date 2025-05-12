using System;

namespace CoreKernel.DomainMarkers.SoftDeletion;

/// <summary>
/// Interface for entities that support soft deletion.
/// Provides properties to track the deletion state, timestamp, and the user responsible for deletion.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was deleted.
    /// This value is null if the entity has not been deleted.
    /// </summary>
    DateTimeOffset? DeletedOn { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// This value is null if the entity has not been deleted or the user is unknown.
    /// </summary>
    string? DeletedBy { get; set; }
}