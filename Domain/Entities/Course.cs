using Riwi_Courses_Assessment_Backend.Domain.Enums;

namespace Riwi_Courses_Assessment_Backend.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
    
    // Navigation property
    public virtual ICollection<Lesson> Lessons { get; set; }

    public Course()
    {
        Status = CourseStatus.Draft;
        Lessons = new List<Lesson>();
    }

    // Business logic methods
    public bool CanBePublished()
    {
        return Lessons.Any(l => !l.IsDeleted);
    }

    public void Publish()
    {
        if (!CanBePublished())
        {
            throw new InvalidOperationException("Cannot publish a course without active lessons.");
        }
        Status = CourseStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        Status = CourseStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

