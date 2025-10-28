using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Features.Enrollments;

namespace learnify.ai.api.Common.Data.Configurations;

public class ProgressConfiguration : IEntityTypeConfiguration<Progress>
{
    public void Configure(EntityTypeBuilder<Progress> builder)
    {
        builder.ToTable("Progress", t => 
        {
            t.HasCheckConstraint("CK_Progress_TimeSpent", "\"TimeSpent\" >= 0");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IsCompleted)
            .IsRequired();

        builder.Property(p => p.TimeSpent)
            .IsRequired();

        builder.Property(p => p.LastAccessDate)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Unique constraint for enrollment-lesson combination
        builder.HasIndex(p => new { p.EnrollmentId, p.LessonId })
            .IsUnique()
            .HasDatabaseName("IX_Progress_EnrollmentId_LessonId_Unique");

        // Indexes for performance
        builder.HasIndex(p => p.EnrollmentId);
        builder.HasIndex(p => p.LessonId);
        builder.HasIndex(p => p.IsCompleted);
        builder.HasIndex(p => p.LastAccessDate);
    }
}