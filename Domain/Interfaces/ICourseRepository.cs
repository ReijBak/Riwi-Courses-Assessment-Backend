using Riwi_Courses_Assessment_Backend.Domain.Entities;
using Riwi_Courses_Assessment_Backend.Domain.Enums;

namespace Riwi_Courses_Assessment_Backend.Domain.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> SearchAsync(string? searchTerm, CourseStatus? status, int page, int pageSize);
    Task<Course?> GetByIdWithLessonsAsync(Guid id);
    Task<int> CountAsync(string? searchTerm, CourseStatus? status);
}

