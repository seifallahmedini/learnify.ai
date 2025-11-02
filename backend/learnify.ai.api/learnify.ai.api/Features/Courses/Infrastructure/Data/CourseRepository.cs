using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Common.Infrastructure.Data.Repositories;

namespace learnify.ai.api.Features.Courses;

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

    // Featured courses implementation
    public async Task<IEnumerable<Course>> GetFeaturedCoursesAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = BuildCourseQuery(categoryId, instructorId, level, minPrice, maxPrice, searchTerm, true)
            .Where(c => c.IsFeatured);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetFeaturedCoursesCountAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildCourseQuery(categoryId, instructorId, level, minPrice, maxPrice, searchTerm, true)
            .Where(c => c.IsFeatured);

        return await query.CountAsync(cancellationToken);
    }

    // Popular courses implementation (sorted by enrollment count)
    public async Task<IEnumerable<Course>> GetPopularCoursesAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = BuildCourseQuery(categoryId, instructorId, level, minPrice, maxPrice, searchTerm, true);

        return await query
            .GroupJoin(_context.Enrollments,
                course => course.Id,
                enrollment => enrollment.CourseId,
                (course, enrollments) => new { course, enrollmentCount = enrollments.Count() })
            .OrderByDescending(x => x.enrollmentCount)
            .ThenByDescending(x => x.course.CreatedAt)
            .Select(x => x.course)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetPopularCoursesCountAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildCourseQuery(categoryId, instructorId, level, minPrice, maxPrice, searchTerm, true);
        return await query.CountAsync(cancellationToken);
    }

    // Recent courses implementation (sorted by creation date)
    public async Task<IEnumerable<Course>> GetRecentCoursesAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = BuildCourseQuery(categoryId, instructorId, level, minPrice, maxPrice, searchTerm, true);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetRecentCoursesCountAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildCourseQuery(categoryId, instructorId, level, minPrice, maxPrice, searchTerm, true);
        return await query.CountAsync(cancellationToken);
    }

    // Helper method to build common query filters
    private IQueryable<Course> BuildCourseQuery(
        int? categoryId,
        int? instructorId,
        CourseLevel? level,
        decimal? minPrice,
        decimal? maxPrice,
        string? searchTerm,
        bool? isPublished = null)
    {
        var query = _dbSet.AsQueryable();

        if (isPublished.HasValue)
            query = query.Where(c => c.IsPublished == isPublished.Value);

        if (categoryId.HasValue)
            query = query.Where(c => c.CategoryId == categoryId.Value);

        if (instructorId.HasValue)
            query = query.Where(c => c.InstructorId == instructorId.Value);

        if (level.HasValue)
            query = query.Where(c => c.Level == level.Value);

        if (minPrice.HasValue)
            query = query.Where(c => c.GetEffectivePrice() >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(c => c.GetEffectivePrice() <= maxPrice.Value);

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(c => c.Title.Contains(searchTerm) || c.Description.Contains(searchTerm));

        return query;
    }

    public override async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
