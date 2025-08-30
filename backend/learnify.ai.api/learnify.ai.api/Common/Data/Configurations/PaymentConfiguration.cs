using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using learnify.ai.api.Features.Payments;

namespace learnify.ai.api.Common.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.CourseId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.TransactionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PaymentMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Unique constraint for TransactionId
        builder.HasIndex(p => p.TransactionId)
            .IsUnique()
            .HasDatabaseName("IX_Payments_TransactionId_Unique");

        // Performance indexes
        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_Payments_UserId");

        builder.HasIndex(p => p.CourseId)
            .HasDatabaseName("IX_Payments_CourseId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Payments_Status");

        builder.HasIndex(p => p.PaymentDate)
            .HasDatabaseName("IX_Payments_PaymentDate");

        builder.HasIndex(p => p.PaymentMethod)
            .HasDatabaseName("IX_Payments_PaymentMethod");

        // Composite indexes for common queries
        builder.HasIndex(p => new { p.UserId, p.Status })
            .HasDatabaseName("IX_Payments_UserId_Status");

        builder.HasIndex(p => new { p.CourseId, p.Status })
            .HasDatabaseName("IX_Payments_CourseId_Status");

        builder.HasIndex(p => new { p.UserId, p.CourseId })
            .HasDatabaseName("IX_Payments_UserId_CourseId");

        // Relationships are configured in LearnifyDbContext
    }
}