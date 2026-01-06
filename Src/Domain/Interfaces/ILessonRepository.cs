using Riwi.CoursesAssessment.Domain.Entities;

namespace Riwi.CoursesAssessment.Domain.Interfaces;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId);
    Task<IEnumerable<Lesson>> GetDeletedByCourseIdAsync(Guid courseId);
    Task<Lesson?> GetByIdIncludingDeletedAsync(Guid id);
    Task<bool> HasDuplicateOrderAsync(Guid courseId, int order, Guid? excludeLessonId = null);
    Task<Lesson?> GetByOrderAsync(Guid courseId, int order);
    Task<int> GetMaxOrderAsync(Guid courseId);
    Task HardDeleteAsync(Guid id);
}

