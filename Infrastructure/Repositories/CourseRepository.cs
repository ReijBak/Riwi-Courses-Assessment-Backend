using Microsoft.EntityFrameworkCore;
using Riwi.CoursesAssessment.Domain.Entities;
using Riwi.CoursesAssessment.Domain.Enums;
using Riwi.CoursesAssessment.Domain.Interfaces;
using Riwi.CoursesAssessment.Infrastructure.Data;

namespace Riwi.CoursesAssessment.Infrastructure.Repositories;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Course>> SearchAsync(string? searchTerm, CourseStatus? status, int page, int pageSize)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Title.Contains(searchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdWithLessonsAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CountAsync(string? searchTerm, CourseStatus? status)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Title.Contains(searchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        return await query.CountAsync();
    }

    public async Task HardDeleteAsync(Guid id)
    {
        // Ignorar el filtro global para poder eliminar incluso registros con IsDeleted = true
        var course = await _dbSet.IgnoreQueryFilters()
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course != null)
        {
            // Eliminar las lecciones asociadas primero
            if (course.Lessons.Any())
            {
                _context.Set<Lesson>().RemoveRange(course.Lessons);
            }
            
            // Eliminar el curso
            _dbSet.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}

