using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Common.Infrastructure;

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts", t => 
        {
            t.HasCheckConstraint("CK_QuizAttempt_Score", "\"Score\" >= 0");
            t.HasCheckConstraint("CK_QuizAttempt_MaxScore", "\"MaxScore\" > 0");
            t.HasCheckConstraint("CK_QuizAttempt_TimeSpent", "\"TimeSpent\" >= 0");
        });

        builder.HasKey(qa => qa.Id);

        builder.Property(qa => qa.QuizId)
            .IsRequired();

        builder.Property(qa => qa.UserId)
            .IsRequired();

        builder.Property(qa => qa.Score)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(qa => qa.MaxScore)
            .IsRequired();

        builder.Property(qa => qa.IsPassed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(qa => qa.StartedAt)
            .IsRequired();

        builder.Property(qa => qa.CompletedAt);

        builder.Property(qa => qa.TimeSpent)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(qa => qa.CreatedAt)
            .IsRequired();

        builder.Property(qa => qa.UpdatedAt)
            .IsRequired();

        // Performance indexes
        builder.HasIndex(qa => qa.QuizId)
            .HasDatabaseName("IX_QuizAttempts_QuizId");

        builder.HasIndex(qa => qa.UserId)
            .HasDatabaseName("IX_QuizAttempts_UserId");

        builder.HasIndex(qa => qa.IsPassed)
            .HasDatabaseName("IX_QuizAttempts_IsPassed");

        builder.HasIndex(qa => qa.StartedAt)
            .HasDatabaseName("IX_QuizAttempts_StartedAt");

        builder.HasIndex(qa => qa.CompletedAt)
            .HasDatabaseName("IX_QuizAttempts_CompletedAt");

        // Composite indexes for common queries
        builder.HasIndex(qa => new { qa.QuizId, qa.UserId })
            .HasDatabaseName("IX_QuizAttempts_QuizId_UserId");

        builder.HasIndex(qa => new { qa.UserId, qa.IsPassed })
            .HasDatabaseName("IX_QuizAttempts_UserId_IsPassed");

        builder.HasIndex(qa => new { qa.QuizId, qa.IsPassed })
            .HasDatabaseName("IX_QuizAttempts_QuizId_IsPassed");

        builder.HasIndex(qa => new { qa.UserId, qa.StartedAt })
            .HasDatabaseName("IX_QuizAttempts_UserId_StartedAt");

        // Relationships are configured in LearnifyDbContext
    }
}