using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Common.Infrastructure;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.ShortDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.InstructorId)
            .IsRequired();

        builder.Property(c => c.CategoryId)
            .IsRequired();

        builder.Property(c => c.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.DiscountPrice)
            .HasPrecision(18, 2);

        builder.Property(c => c.DurationHours)
            .IsRequired();

        builder.Property(c => c.Level)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Language)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(c => c.VideoPreviewUrl)
            .HasMaxLength(500);

        builder.Property(c => c.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.MaxStudents);

        builder.Property(c => c.Prerequisites)
            .HasMaxLength(1000);

        builder.Property(c => c.LearningObjectives)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        // Performance indexes
        builder.HasIndex(c => c.Title)
            .HasDatabaseName("IX_Courses_Title");

        builder.HasIndex(c => c.InstructorId)
            .HasDatabaseName("IX_Courses_InstructorId");

        builder.HasIndex(c => c.CategoryId)
            .HasDatabaseName("IX_Courses_CategoryId");

        builder.HasIndex(c => c.IsPublished)
            .HasDatabaseName("IX_Courses_IsPublished");

        builder.HasIndex(c => c.IsFeatured)
            .HasDatabaseName("IX_Courses_IsFeatured");

        builder.HasIndex(c => c.Level)
            .HasDatabaseName("IX_Courses_Level");

        builder.HasIndex(c => c.Price)
            .HasDatabaseName("IX_Courses_Price");

        builder.HasIndex(c => c.CreatedAt)
            .HasDatabaseName("IX_Courses_CreatedAt");

        // Composite indexes for common queries
        builder.HasIndex(c => new { c.CategoryId, c.IsPublished })
            .HasDatabaseName("IX_Courses_CategoryId_IsPublished");

        builder.HasIndex(c => new { c.InstructorId, c.IsPublished })
            .HasDatabaseName("IX_Courses_InstructorId_IsPublished");

        builder.HasIndex(c => new { c.Level, c.IsPublished })
            .HasDatabaseName("IX_Courses_Level_IsPublished");

        builder.HasIndex(c => new { c.Price, c.IsPublished })
            .HasDatabaseName("IX_Courses_Price_IsPublished");

        builder.HasIndex(c => new { c.IsFeatured, c.IsPublished })
            .HasDatabaseName("IX_Courses_IsFeatured_IsPublished");

        // Relationships are configured in LearnifyDbContext
    }
}