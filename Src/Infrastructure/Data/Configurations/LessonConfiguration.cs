using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Riwi.CoursesAssessment.Domain.Entities;

namespace Riwi.CoursesAssessment.Infrastructure.Data.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever(); // GUID generated in the entity constructor

        builder.Property(l => l.CourseId)
            .IsRequired();

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Order)
            .IsRequired();

        builder.Property(l => l.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .IsRequired();

        // Unique constraint: Order must be unique per course
        builder.HasIndex(l => new { l.CourseId, l.Order })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false"); // Only enforce for active lessons

        // Index for better query performance
        builder.HasIndex(l => l.CourseId);
        builder.HasIndex(l => l.IsDeleted);
    }
}

