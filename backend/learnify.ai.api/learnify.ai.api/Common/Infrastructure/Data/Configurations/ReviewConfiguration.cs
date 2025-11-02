using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Common.Infrastructure;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", t => 
        {
            t.HasCheckConstraint("CK_Review_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5");
        });

        builder.HasKey(r => r.Id);

        builder.Property(r => r.CourseId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.Property(r => r.IsApproved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        // Unique constraint for user-course combination
        builder.HasIndex(r => new { r.UserId, r.CourseId })
            .IsUnique()
            .HasDatabaseName("IX_Reviews_UserId_CourseId_Unique");

        // Performance indexes
        builder.HasIndex(r => r.CourseId)
            .HasDatabaseName("IX_Reviews_CourseId");

        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_Reviews_UserId");

        builder.HasIndex(r => r.Rating)
            .HasDatabaseName("IX_Reviews_Rating");

        builder.HasIndex(r => r.IsApproved)
            .HasDatabaseName("IX_Reviews_IsApproved");

        builder.HasIndex(r => r.CreatedAt)
            .HasDatabaseName("IX_Reviews_CreatedAt");

        // Composite indexes for common queries
        builder.HasIndex(r => new { r.CourseId, r.IsApproved })
            .HasDatabaseName("IX_Reviews_CourseId_IsApproved");

        builder.HasIndex(r => new { r.CourseId, r.Rating })
            .HasDatabaseName("IX_Reviews_CourseId_Rating");

        // Relationships are configured in LearnifyDbContext
    }
}