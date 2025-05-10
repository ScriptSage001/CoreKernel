namespace CoreKernel.DomainMarkers.MultiTenancy;

/// <summary>
/// Interface for entities that are scoped to a specific tenant.
/// Provides a property to track the tenant identifier.
/// </summary>
/// <typeparam name="TId">
/// The type of the tenant identifier. Must be a non-nullable type.
/// </typeparam>
public interface ITenantScoped<TId> where TId : notnull
{
    /// <summary>
    /// Gets or sets the identifier of the tenant to which the entity belongs.
    /// </summary>
    TId TenantId { get; set; }
}