using Riwi.CoursesAssessment.Application.DTOs;

namespace Riwi.CoursesAssessment.Application.Interfaces;

public interface ICourseService
{
    Task<CourseDto> GetByIdAsync(Guid id);
    Task<CourseSearchResultDto> SearchAsync(string? searchTerm, string? status, int page, int pageSize);
    Task<CourseSummaryDto> GetSummaryAsync(Guid id);
    Task<CourseDto> CreateAsync(CreateCourseDto dto);
    Task<CourseDto> UpdateAsync(Guid id, UpdateCourseDto dto);
    Task DeleteAsync(Guid id);
    Task HardDeleteAsync(Guid id);
    Task PublishAsync(Guid id);
    Task UnpublishAsync(Guid id);
}


