using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Riwi_Courses_Assessment_Backend.Application.Interfaces;
using Riwi_Courses_Assessment_Backend.Application.Services;
using Riwi_Courses_Assessment_Backend.Domain.Interfaces;
using Riwi_Courses_Assessment_Backend.Infrastructure.Data;
using Riwi_Courses_Assessment_Backend.Infrastructure.Repositories;

namespace Riwi_Courses_Assessment_Backend.Infrastructure;

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

