using Riwi.CoursesAssessment.Application.DTOs;

namespace Riwi.CoursesAssessment.Application.Interfaces;

public interface ILessonService
{
    Task<LessonDto> GetByIdAsync(Guid id);
    Task<IEnumerable<LessonDto>> GetByCourseIdAsync(Guid courseId);
    Task<LessonDto> CreateAsync(CreateLessonDto dto);
    Task<LessonDto> UpdateAsync(Guid id, UpdateLessonDto dto);
    Task DeleteAsync(Guid id);
    Task ReorderAsync(Guid id, int newOrder);
}

