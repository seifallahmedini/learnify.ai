using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Reviews.Core.Models;

namespace learnify.ai.api.Features.Reviews.Infrastructure.Data;

public interface IReviewRepository : IBaseRepository<Review>
{
    Task<IEnumerable<Review>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Review?> GetByUserAndCourseAsync(int userId, int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetApprovedReviewsAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(int courseId, CancellationToken cancellationToken = default);
    Task<int> GetReviewCountAsync(int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Review>> GetReviewsByRatingAsync(int courseId, int rating, CancellationToken cancellationToken = default);
    Task<bool> HasUserReviewedCourseAsync(int userId, int courseId, CancellationToken cancellationToken = default);
}