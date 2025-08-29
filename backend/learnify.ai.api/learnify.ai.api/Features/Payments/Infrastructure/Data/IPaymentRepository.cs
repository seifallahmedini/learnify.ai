using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Payments.Core.Models;

namespace learnify.ai.api.Features.Payments.Infrastructure.Data;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetCompletedPaymentsAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetRefundedPaymentsAsync(int userId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetRevenueByInstructorAsync(int instructorId, CancellationToken cancellationToken = default);
    Task<decimal> GetRevenueByCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<bool> HasUserPurchasedCourseAsync(int userId, int courseId, CancellationToken cancellationToken = default);
}