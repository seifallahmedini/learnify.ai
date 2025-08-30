using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Features.Assessments;

namespace learnify.ai.api.Common.Data.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers", t => 
        {
            t.HasCheckConstraint("CK_Answer_OrderIndex", "OrderIndex >= 0");
        });

        builder.HasKey(a => a.Id);

        builder.Property(a => a.QuestionId)
            .IsRequired();

        builder.Property(a => a.AnswerText)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.IsCorrect)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.OrderIndex)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        // Performance indexes
        builder.HasIndex(a => a.QuestionId)
            .HasDatabaseName("IX_Answers_QuestionId");

        builder.HasIndex(a => a.IsCorrect)
            .HasDatabaseName("IX_Answers_IsCorrect");

        // Composite indexes for common queries
        builder.HasIndex(a => new { a.QuestionId, a.OrderIndex })
            .HasDatabaseName("IX_Answers_QuestionId_OrderIndex");

        builder.HasIndex(a => new { a.QuestionId, a.IsCorrect })
            .HasDatabaseName("IX_Answers_QuestionId_IsCorrect");

        // Relationships are configured in LearnifyDbContext
    }
}