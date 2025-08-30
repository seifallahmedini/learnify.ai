using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Courses;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<IEnumerable<Course>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Course>> GetPublishedCoursesAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<int> GetEnrollmentCountAsync(int courseId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(int courseId, CancellationToken cancellationToken = default);
    Task<int> GetTotalStudentsAsync(int courseId, CancellationToken cancellationToken = default);
    
    // Featured courses
    Task<IEnumerable<Course>> GetFeaturedCoursesAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    
    Task<int> GetFeaturedCoursesCountAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
    
    // Popular courses (sorted by enrollment count)
    Task<IEnumerable<Course>> GetPopularCoursesAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    
    Task<int> GetPopularCoursesCountAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
    
    // Recent courses (sorted by creation date)
    Task<IEnumerable<Course>> GetRecentCoursesAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    
    Task<int> GetRecentCoursesCountAsync(
        int? categoryId = null,
        int? instructorId = null,
        CourseLevel? level = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}