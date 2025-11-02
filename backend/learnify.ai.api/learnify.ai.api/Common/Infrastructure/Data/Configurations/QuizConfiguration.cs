using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Common.Infrastructure;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes", t =>
        {
            t.HasCheckConstraint("CK_Quiz_PassingScore", "\"PassingScore\" >= 0 AND \"PassingScore\" <= 100");
            t.HasCheckConstraint("CK_Quiz_MaxAttempts", "\"MaxAttempts\" > 0");
            t.HasCheckConstraint("CK_Quiz_TimeLimit", "\"TimeLimit\" IS NULL OR \"TimeLimit\" > 0");
        });

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(q => q.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.PassingScore)
            .IsRequired();

        builder.Property(q => q.MaxAttempts)
            .IsRequired();

        builder.Property(q => q.IsActive)
            .IsRequired();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.UpdatedAt)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(q => q.CourseId)
            .HasDatabaseName("IX_Quizzes_CourseId");

        builder.HasIndex(q => q.LessonId)
            .HasDatabaseName("IX_Quizzes_LessonId");

        builder.HasIndex(q => q.IsActive)
            .HasDatabaseName("IX_Quizzes_IsActive");

        builder.HasIndex(q => q.CreatedAt)
            .HasDatabaseName("IX_Quizzes_CreatedAt");

        // Composite indexes for common queries
        builder.HasIndex(q => new { q.CourseId, q.IsActive })
            .HasDatabaseName("IX_Quizzes_CourseId_IsActive");

        builder.HasIndex(q => new { q.LessonId, q.IsActive })
            .HasDatabaseName("IX_Quizzes_LessonId_IsActive");
    }
}