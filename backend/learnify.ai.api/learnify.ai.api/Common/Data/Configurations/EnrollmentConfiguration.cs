using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Features.Enrollments;

namespace learnify.ai.api.Common.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments", t => 
        {
            t.HasCheckConstraint("CK_Enrollment_Progress", "\"Progress\" >= 0 AND \"Progress\" <= 100");
        });

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EnrollmentDate)
            .IsRequired();

        builder.Property(e => e.Progress)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // Unique constraint for user-course combination
        builder.HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique()
            .HasDatabaseName("IX_Enrollments_UserId_CourseId_Unique");

        // Indexes for performance
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.CourseId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.EnrollmentDate);
    }
}