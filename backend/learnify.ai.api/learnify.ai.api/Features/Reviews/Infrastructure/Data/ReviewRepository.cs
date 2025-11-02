using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Common.Infrastructure.Data.Repositories;

using learnify.ai.api.Domain.Entities;
namespace learnify.ai.api.Features.Reviews;

public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    public ReviewRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Review>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetByUserAndCourseAsync(int userId, int courseId, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetApprovedReviewsAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.CourseId == courseId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetPendingReviewsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => !r.IsApproved)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<double> GetAverageRatingAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var average = await _dbSet
            .Where(r => r.CourseId == courseId && r.IsApproved)
            .AverageAsync(r => (double?)r.Rating, cancellationToken);

        return average ?? 0;
    }

    public async Task<int> GetReviewCountAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await CountAsync(r => r.CourseId == courseId && r.IsApproved, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetReviewsByRatingAsync(int courseId, int rating, CancellationToken cancellationToken = default)
    {
        return await FindAsync(r => r.CourseId == courseId && r.Rating == rating && r.IsApproved, cancellationToken);
    }

    public async Task<bool> HasUserReviewedCourseAsync(int userId, int courseId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(r => r.UserId == userId && r.CourseId == courseId, cancellationToken);
    }
}
