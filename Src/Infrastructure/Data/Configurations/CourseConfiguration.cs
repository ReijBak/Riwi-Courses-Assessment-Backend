using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riwi.CoursesAssessment.Domain.Entities;
using Riwi.CoursesAssessment.Domain.Enums;

namespace Riwi.CoursesAssessment.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever(); // GUID generated in the entity constructor

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (CourseStatus)Enum.Parse(typeof(CourseStatus), v))
            .HasMaxLength(50);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        // Relationship: One Course has Many Lessons
        builder.HasMany(c => c.Lessons)
            .WithOne(l => l.Course)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for better search performance
        builder.HasIndex(c => c.Title);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.IsDeleted);
    }
}

