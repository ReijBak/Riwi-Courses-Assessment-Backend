using Microsoft.EntityFrameworkCore;
using Riwi.CoursesAssessment.Domain.Entities;
using Riwi.CoursesAssessment.Domain.Interfaces;
using Riwi.CoursesAssessment.Infrastructure.Data;

namespace Riwi.CoursesAssessment.Infrastructure.Repositories;

public class LessonRepository : Repository<Lesson>, ILessonRepository
{
    public LessonRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId)
    {
        return await _dbSet
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task<bool> HasDuplicateOrderAsync(Guid courseId, int order, Guid? excludeLessonId = null)
    {
        var query = _dbSet.Where(l => l.CourseId == courseId && l.Order == order);

        if (excludeLessonId.HasValue)
        {
            query = query.Where(l => l.Id != excludeLessonId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<Lesson?> GetByOrderAsync(Guid courseId, int order)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.CourseId == courseId && l.Order == order);
    }

    public async Task<int> GetMaxOrderAsync(Guid courseId)
    {
        var maxOrder = await _dbSet
            .Where(l => l.CourseId == courseId)
            .MaxAsync(l => (int?)l.Order);

        return maxOrder ?? 0;
    }

    public async Task HardDeleteAsync(Guid id)
    {
        // Ignorar el filtro global para poder eliminar incluso registros con IsDeleted = true
        var lesson = await _dbSet.IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson != null)
        {
            _dbSet.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }
}

