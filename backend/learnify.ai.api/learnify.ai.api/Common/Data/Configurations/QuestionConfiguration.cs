using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Features.Assessments;

namespace learnify.ai.api.Common.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions", t => 
        {
            t.HasCheckConstraint("CK_Question_OrderIndex", "OrderIndex >= 0");
            t.HasCheckConstraint("CK_Question_Points", "Points > 0");
        });

        builder.HasKey(q => q.Id);

        builder.Property(q => q.QuizId)
            .IsRequired();

        builder.Property(q => q.QuestionText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(q => q.QuestionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(q => q.OrderIndex)
            .IsRequired();

        builder.Property(q => q.Points)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(q => q.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.UpdatedAt)
            .IsRequired();

        // Performance indexes
        builder.HasIndex(q => q.QuizId)
            .HasDatabaseName("IX_Questions_QuizId");

        builder.HasIndex(q => q.IsActive)
            .HasDatabaseName("IX_Questions_IsActive");

        builder.HasIndex(q => q.QuestionType)
            .HasDatabaseName("IX_Questions_QuestionType");

        // Composite indexes for common queries
        builder.HasIndex(q => new { q.QuizId, q.OrderIndex })
            .HasDatabaseName("IX_Questions_QuizId_OrderIndex");

        builder.HasIndex(q => new { q.QuizId, q.IsActive })
            .HasDatabaseName("IX_Questions_QuizId_IsActive");

        // Relationships are configured in LearnifyDbContext
    }
}