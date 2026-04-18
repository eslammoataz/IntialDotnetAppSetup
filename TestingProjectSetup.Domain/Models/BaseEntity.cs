namespace TestingProjectSetup.Domain.Models;

/// <summary>
/// Base entity with common properties including soft delete and audit fields
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Base entity with audit fields for CreatedBy/UpdatedBy tracking
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public string? CreatedById { get; set; }
    public string? UpdatedById { get; set; }
}
