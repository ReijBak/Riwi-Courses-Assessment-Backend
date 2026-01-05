namespace Riwi_Courses_Assessment_Backend.Domain.Entities;
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
