using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Courses.Core.Models;

namespace learnify.ai.api.Features.Courses.Infrastructure.Data;

public class LessonRepository : BaseRepository<Lesson>, ILessonRepository
{
    public LessonRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Lesson>> GetPublishedLessonsAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.CourseId == courseId && l.IsPublished)
            .OrderBy(l => l.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Lesson>> GetFreeLessonsAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.CourseId == courseId && l.IsFree && l.IsPublished)
            .OrderBy(l => l.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<Lesson?> GetNextLessonAsync(int courseId, int currentOrderIndex, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.CourseId == courseId && l.OrderIndex > currentOrderIndex && l.IsPublished)
            .OrderBy(l => l.OrderIndex)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Lesson?> GetPreviousLessonAsync(int courseId, int currentOrderIndex, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.CourseId == courseId && l.OrderIndex < currentOrderIndex && l.IsPublished)
            .OrderByDescending(l => l.OrderIndex)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetTotalDurationAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var lessons = await _dbSet
            .Where(l => l.CourseId == courseId && l.IsPublished)
            .ToListAsync(cancellationToken);
        
        return lessons.Sum(l => l.Duration);
    }

    public async Task<int> GetLessonCountAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await CountAsync(l => l.CourseId == courseId && l.IsPublished, cancellationToken);
    }

    public async Task<int> GetMaxOrderIndexAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var maxOrder = await _dbSet
            .Where(l => l.CourseId == courseId)
            .MaxAsync(l => (int?)l.OrderIndex, cancellationToken);
        
        return maxOrder ?? 0;
    }
}