using Microsoft.EntityFrameworkCore;
using Riwi_Courses_Assessment_Backend.Domain.Entities;
using Riwi_Courses_Assessment_Backend.Domain.Enums;
using Riwi_Courses_Assessment_Backend.Domain.Interfaces;
using Riwi_Courses_Assessment_Backend.Infrastructure.Data;

namespace Riwi_Courses_Assessment_Backend.Infrastructure.Repositories;

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
}

