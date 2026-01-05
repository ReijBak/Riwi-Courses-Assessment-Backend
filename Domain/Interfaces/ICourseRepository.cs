using Riwi.CoursesAssessment.Domain.Entities;
using Riwi.CoursesAssessment.Domain.Enums;

namespace Riwi.CoursesAssessment.Domain.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> SearchAsync(string? searchTerm, CourseStatus? status, int page, int pageSize);
    Task<Course?> GetByIdWithLessonsAsync(Guid id);
    Task<int> CountAsync(string? searchTerm, CourseStatus? status);
    Task HardDeleteAsync(Guid id);
}

