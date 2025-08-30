using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Payments;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CourseId == courseId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.Status == status, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetCompletedPaymentsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.UserId == userId && p.Status == PaymentStatus.Completed, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetRefundedPaymentsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.UserId == userId && p.Status == PaymentStatus.Refunded, cancellationToken);
    }

    public async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
    {
        var completedPayments = await _dbSet
            .Where(p => p.Status == PaymentStatus.Completed)
            .ToListAsync(cancellationToken);

        return completedPayments.Sum(p => p.Amount - (p.RefundAmount ?? 0));
    }

    public async Task<decimal> GetRevenueByInstructorAsync(int instructorId, CancellationToken cancellationToken = default)
    {
        var revenue = await _dbSet
            .Join(_context.Courses,
                payment => payment.CourseId,
                course => course.Id,
                (payment, course) => new { Payment = payment, Course = course })
            .Where(joined => joined.Course.InstructorId == instructorId && 
                           joined.Payment.Status == PaymentStatus.Completed)
            .SumAsync(joined => joined.Payment.Amount - (joined.Payment.RefundAmount ?? 0), 
                     cancellationToken);

        return revenue;
    }

    public async Task<decimal> GetRevenueByCourseAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var completedPayments = await _dbSet
            .Where(p => p.CourseId == courseId && p.Status == PaymentStatus.Completed)
            .ToListAsync(cancellationToken);

        return completedPayments.Sum(p => p.Amount - (p.RefundAmount ?? 0));
    }

    public async Task<bool> HasUserPurchasedCourseAsync(int userId, int courseId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(p => p.UserId == userId && 
                                     p.CourseId == courseId && 
                                     p.Status == PaymentStatus.Completed, 
                                cancellationToken);
    }
}