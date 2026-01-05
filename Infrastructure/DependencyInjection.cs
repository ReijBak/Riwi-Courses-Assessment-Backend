using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Riwi.CoursesAssessment.Application.Interfaces;
using Riwi.CoursesAssessment.Application.Services;
using Riwi.CoursesAssessment.Domain.Interfaces;
using Riwi.CoursesAssessment.Infrastructure.Data;
using Riwi.CoursesAssessment.Infrastructure.Repositories;

namespace Riwi.CoursesAssessment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();

        // Services
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ILessonService, LessonService>();

        return services;
    }
}

