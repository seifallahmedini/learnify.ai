using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Courses.Core.Models;
using learnify.ai.api.Features.Enrollments.Core.Models;

namespace learnify.ai.api.Features.Courses.Infrastructure.Data;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Course>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(c => c.InstructorId == instructorId, cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(c => c.CategoryId == categoryId && c.IsPublished, cancellationToken);
    }

    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsPublished)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetEnrollmentCountAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .CountAsync(e => e.CourseId == courseId && 
                           (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed), 
                       cancellationToken);
    }

    public async Task<double> GetAverageRatingAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var average = await _context.Reviews
            .Where(r => r.CourseId == courseId && r.IsApproved)
            .AverageAsync(r => (double?)r.Rating, cancellationToken);
        
        return average ?? 0;
    }

    public async Task<int> GetTotalStudentsAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await GetEnrollmentCountAsync(courseId, cancellationToken);
    }

    public override async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}