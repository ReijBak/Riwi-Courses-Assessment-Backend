using Riwi.CoursesAssessment.Application.DTOs;
using Riwi.CoursesAssessment.Application.Interfaces;
using Riwi.CoursesAssessment.Domain.Entities;
using Riwi.CoursesAssessment.Domain.Exceptions;
using Riwi.CoursesAssessment.Domain.Interfaces;

namespace Riwi.CoursesAssessment.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public LessonService(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonDto> GetByIdAsync(Guid id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null || lesson.IsDeleted)
        {
            throw new LessonNotFoundException(id);
        }

        return MapToDto(lesson);
    }

    public async Task<IEnumerable<LessonDto>> GetByCourseIdAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null || course.IsDeleted)
        {
            throw new CourseNotFoundException(courseId);
        }

        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        return lessons.OrderBy(l => l.Order).Select(MapToDto);
    }

    public async Task<LessonDto> CreateAsync(CreateLessonDto dto)
    {
        // Validate course exists
        var course = await _courseRepository.GetByIdAsync(dto.CourseId);
        if (course == null || course.IsDeleted)
        {
            throw new CourseNotFoundException(dto.CourseId);
        }

        // Validate order is not negative
        if (dto.Order < 0)
        {
            throw new InvalidLessonOrderException(dto.Order);
        }

        // Check for duplicate order
        var hasDuplicate = await _lessonRepository.HasDuplicateOrderAsync(dto.CourseId, dto.Order);
        if (hasDuplicate)
        {
            throw new DuplicateLessonOrderException(dto.CourseId, dto.Order);
        }

        var lesson = new Lesson
        {
            CourseId = dto.CourseId,
            Title = dto.Title,
            Order = dto.Order
        };

        var created = await _lessonRepository.AddAsync(lesson);
        return MapToDto(created);
    }

    public async Task<LessonDto> UpdateAsync(Guid id, UpdateLessonDto dto)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null || lesson.IsDeleted)
        {
            throw new LessonNotFoundException(id);
        }

        // Validate order if changed
        if (lesson.Order != dto.Order)
        {
            if (dto.Order < 0)
            {
                throw new InvalidLessonOrderException(dto.Order);
            }

            // Check for duplicate order (excluding current lesson)
            var hasDuplicate = await _lessonRepository.HasDuplicateOrderAsync(lesson.CourseId, dto.Order, id);
            if (hasDuplicate)
            {
                throw new DuplicateLessonOrderException(lesson.CourseId, dto.Order);
            }

            lesson.UpdateOrder(dto.Order);
        }

        lesson.Title = dto.Title;
        lesson.UpdatedAt = DateTime.UtcNow;

        await _lessonRepository.UpdateAsync(lesson);
        return MapToDto(lesson);
    }

    public async Task DeleteAsync(Guid id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null || lesson.IsDeleted)
        {
            throw new LessonNotFoundException(id);
        }

        lesson.SoftDelete();
        await _lessonRepository.UpdateAsync(lesson);
    }

    public async Task HardDeleteAsync(Guid id)
    {
        // Para hard delete, verificamos que la lección exista (incluso si está soft deleted)
        var lesson = await _lessonRepository.GetByIdIncludingDeletedAsync(id);
        if (lesson == null)
        {
            throw new LessonNotFoundException(id);
        }

        await _lessonRepository.HardDeleteAsync(id);
    }

    public async Task RestoreAsync(Guid id)
    {
        var lesson = await _lessonRepository.GetByIdIncludingDeletedAsync(id);
        if (lesson == null)
        {
            throw new LessonNotFoundException(id);
        }

        if (!lesson.IsDeleted)
        {
            throw new CannotRestoreException("Lesson", id);
        }

        // Verificar que el order no esté duplicado al restaurar
        var hasDuplicate = await _lessonRepository.HasDuplicateOrderAsync(lesson.CourseId, lesson.Order, id);
        if (hasDuplicate)
        {
            // Asignar el siguiente order disponible
            var maxOrder = await _lessonRepository.GetMaxOrderAsync(lesson.CourseId);
            lesson.Order = maxOrder + 1;
        }

        lesson.Restore();
        await _lessonRepository.UpdateAsync(lesson);
    }

    public async Task<IEnumerable<LessonDto>> GetDeletedByCourseIdAsync(Guid courseId)
    {
        var lessons = await _lessonRepository.GetDeletedByCourseIdAsync(courseId);
        return lessons.Select(MapToDto);
    }

    public async Task ReorderAsync(Guid id, int newOrder)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null || lesson.IsDeleted)
        {
            throw new LessonNotFoundException(id);
        }

        if (newOrder < 0)
        {
            throw new InvalidLessonOrderException(newOrder);
        }

        if (lesson.Order == newOrder)
        {
            return; // No change needed
        }

        var targetLesson = await _lessonRepository.GetByOrderAsync(lesson.CourseId, newOrder);
        
        if (targetLesson != null && !targetLesson.IsDeleted)
        {
            // Swap orders to avoid duplicates
            var tempOrder = lesson.Order;
            lesson.UpdateOrder(newOrder);
            targetLesson.UpdateOrder(tempOrder);
            
            await _lessonRepository.UpdateAsync(lesson);
            await _lessonRepository.UpdateAsync(targetLesson);
        }
        else
        {
            // Check if order already exists
            var hasDuplicate = await _lessonRepository.HasDuplicateOrderAsync(lesson.CourseId, newOrder, id);
            if (hasDuplicate)
            {
                throw new DuplicateLessonOrderException(lesson.CourseId, newOrder);
            }

            lesson.UpdateOrder(newOrder);
            await _lessonRepository.UpdateAsync(lesson);
        }
    }

    private static LessonDto MapToDto(Lesson lesson)
    {
        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Order = lesson.Order,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }
}

