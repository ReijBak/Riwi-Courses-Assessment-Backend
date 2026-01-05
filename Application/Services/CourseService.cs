using Riwi.CoursesAssessment.Application.DTOs;
using Riwi.CoursesAssessment.Application.Interfaces;
using Riwi.CoursesAssessment.Domain.Entities;
using Riwi.CoursesAssessment.Domain.Enums;
using Riwi.CoursesAssessment.Domain.Exceptions;
using Riwi.CoursesAssessment.Domain.Interfaces;

namespace Riwi.CoursesAssessment.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public CourseService(ICourseRepository courseRepository, ILessonRepository lessonRepository)
    {
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<CourseDto> GetByIdAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null || course.IsDeleted)
        {
            throw new CourseNotFoundException(id);
        }

        return MapToDto(course);
    }

    public async Task<CourseSearchResultDto> SearchAsync(string? searchTerm, string? status, int page, int pageSize)
    {
        CourseStatus? courseStatus = null;
        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<CourseStatus>(status, true, out var parsedStatus))
            {
                courseStatus = parsedStatus;
            }
        }

        var courses = await _courseRepository.SearchAsync(searchTerm, courseStatus, page, pageSize);
        var totalCount = await _courseRepository.CountAsync(searchTerm, courseStatus);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new CourseSearchResultDto
        {
            Courses = courses.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<CourseSummaryDto> GetSummaryAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(id);
        if (course == null || course.IsDeleted)
        {
            throw new CourseNotFoundException(id);
        }

        var activeLessons = course.Lessons.Count(l => !l.IsDeleted);

        return new CourseSummaryDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status.ToString(),
            TotalLessons = activeLessons,
            LastModified = course.UpdatedAt
        };
    }

    public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
    {
        var course = new Course
        {
            Title = dto.Title
        };

        var created = await _courseRepository.AddAsync(course);
        return MapToDto(created);
    }

    public async Task<CourseDto> UpdateAsync(Guid id, UpdateCourseDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null || course.IsDeleted)
        {
            throw new CourseNotFoundException(id);
        }

        course.Title = dto.Title;
        course.UpdatedAt = DateTime.UtcNow;

        await _courseRepository.UpdateAsync(course);
        return MapToDto(course);
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null || course.IsDeleted)
        {
            throw new CourseNotFoundException(id);
        }

        course.SoftDelete();
        await _courseRepository.UpdateAsync(course);
    }

    public async Task HardDeleteAsync(Guid id)
    {
        // Para hard delete, verificamos que el curso exista (incluso si está soft deleted)
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            throw new CourseNotFoundException(id);
        }

        await _courseRepository.HardDeleteAsync(id);
    }

    public async Task PublishAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(id);
        if (course == null || course.IsDeleted)
        {
            throw new CourseNotFoundException(id);
        }

        // This will throw CourseCannotBePublishedException if business rules are not met
        course.Publish();
        
        await _courseRepository.UpdateAsync(course);
    }

    public async Task UnpublishAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null || course.IsDeleted)
        {
            throw new CourseNotFoundException(id);
        }

        course.Unpublish();
        await _courseRepository.UpdateAsync(course);
    }

    private static CourseDto MapToDto(Course course)
    {
        return new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status.ToString(),
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }
}

