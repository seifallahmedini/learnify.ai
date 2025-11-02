using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Common.Infrastructure;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.IconUrl)
            .HasMaxLength(255);

        builder.Property(c => c.ParentCategoryId);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        // Performance indexes
        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Categories_Name");

        builder.HasIndex(c => c.ParentCategoryId)
            .HasDatabaseName("IX_Categories_ParentCategoryId");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_Categories_IsActive");

        // Composite indexes for common queries
        builder.HasIndex(c => new { c.ParentCategoryId, c.IsActive })
            .HasDatabaseName("IX_Categories_ParentCategoryId_IsActive");

        // Self-referencing relationship is configured in LearnifyDbContext
    }
}