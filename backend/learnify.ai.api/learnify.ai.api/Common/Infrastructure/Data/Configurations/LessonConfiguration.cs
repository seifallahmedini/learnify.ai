using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Common.Infrastructure;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons", t => 
        {
            t.HasCheckConstraint("CK_Lesson_Duration", "\"Duration\" > 0");
            t.HasCheckConstraint("CK_Lesson_OrderIndex", "\"OrderIndex\" >= 0");
        });

        builder.HasKey(l => l.Id);

        builder.Property(l => l.CourseId)
            .IsRequired();

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(l => l.Content)
            .IsRequired();

        builder.Property(l => l.VideoUrl)
            .HasMaxLength(500);

        builder.Property(l => l.Duration)
            .IsRequired();

        builder.Property(l => l.OrderIndex)
            .IsRequired();

        builder.Property(l => l.IsFree)
            .IsRequired();

        builder.Property(l => l.IsPublished)
            .IsRequired();

        builder.Property(l => l.LearningObjectives)
            .HasMaxLength(2000);

        builder.Property(l => l.Resources)
            .HasColumnType("text"); // JSON storage

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(l => l.CourseId);
        builder.HasIndex(l => l.IsPublished);
        builder.HasIndex(l => l.IsFree);
        builder.HasIndex(l => new { l.CourseId, l.OrderIndex });
        builder.HasIndex(l => new { l.CourseId, l.IsPublished });
    }
}