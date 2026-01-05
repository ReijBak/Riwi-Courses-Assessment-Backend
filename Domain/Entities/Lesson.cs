namespace Riwi_Courses_Assessment_Backend.Domain.Entities;

public class Lesson : BaseEntity
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    
    // Navigation property
    public virtual Course? Course { get; set; }

    public Lesson()
    {
        Order = 0;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOrder(int newOrder)
    {
        if (newOrder < 0)
        {
            throw new ArgumentException("Order cannot be negative.", nameof(newOrder));
        }
        Order = newOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}

