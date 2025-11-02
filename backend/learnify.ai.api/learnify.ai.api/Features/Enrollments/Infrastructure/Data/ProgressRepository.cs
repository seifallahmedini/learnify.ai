using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Common.Infrastructure.Data.Repositories;

using learnify.ai.api.Domain.Entities;
namespace learnify.ai.api.Features.Enrollments;

public class ProgressRepository : BaseRepository<Domain.Entities.Progress>, IProgressRepository
{
    public ProgressRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Domain.Entities.Progress>> GetByEnrollmentIdAsync(int enrollmentId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.EnrollmentId == enrollmentId, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Progress>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.LessonId == lessonId, cancellationToken);
    }

    public async Task<Domain.Entities.Progress?> GetByEnrollmentAndLessonAsync(int enrollmentId, int lessonId, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId && p.LessonId == lessonId, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Progress>> GetCompletedLessonsAsync(int enrollmentId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.EnrollmentId == enrollmentId && p.IsCompleted, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Progress>> GetIncompleteLessonsAsync(int enrollmentId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(p => p.EnrollmentId == enrollmentId && !p.IsCompleted, cancellationToken);
    }

    public async Task<int> GetCompletedLessonsCountAsync(int enrollmentId, CancellationToken cancellationToken = default)
    {
        return await CountAsync(p => p.EnrollmentId == enrollmentId && p.IsCompleted, cancellationToken);
    }

    public async Task<int> GetTotalTimeSpentAsync(int enrollmentId, CancellationToken cancellationToken = default)
    {
        var progressList = await _dbSet
            .Where(p => p.EnrollmentId == enrollmentId)
            .ToListAsync(cancellationToken);
        
        return progressList.Sum(p => p.TimeSpent);
    }

    public async Task<decimal> GetProgressPercentageAsync(int enrollmentId, CancellationToken cancellationToken = default)
    {
        var totalLessons = await CountAsync(p => p.EnrollmentId == enrollmentId, cancellationToken);
        if (totalLessons == 0) return 0;

        var completedLessons = await GetCompletedLessonsCountAsync(enrollmentId, cancellationToken);
        return Math.Round((decimal)completedLessons / totalLessons * 100, 2);
    }
}
