using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Reviews;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Categories;

public record GetPopularCoursesInCategoryQuery(int CategoryId, int Limit = 10) : IQuery<IEnumerable<PopularCourseResponse>>;

public record PopularCourseResponse(
    int Id,
    string Title,
    string ShortDescription,
    string InstructorName,
    decimal Price,
    decimal? DiscountPrice,
    string? ThumbnailUrl,
    int EnrollmentCount,
    double AverageRating,
    int ReviewCount
);

public class GetPopularCoursesInCategoryValidator : AbstractValidator<GetPopularCoursesInCategoryQuery>
{
    public GetPopularCoursesInCategoryValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than 0");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("Limit must be between 1 and 50");
    }
}

public class GetPopularCoursesInCategoryHandler : IRequestHandler<GetPopularCoursesInCategoryQuery, IEnumerable<PopularCourseResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public GetPopularCoursesInCategoryHandler(
        ICategoryRepository categoryRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IReviewRepository reviewRepository,
        IUserRepository userRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<PopularCourseResponse>> Handle(GetPopularCoursesInCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        
        if (category == null)
            throw new ArgumentException($"Category with ID {request.CategoryId} not found");

        // Get all courses in this category that are published
        var courses = await _courseRepository.GetByCategoryIdAsync(request.CategoryId, cancellationToken);
        var publishedCourses = courses.Where(c => c.IsPublished).ToList();

        var popularCourses = new List<(Course course, int enrollmentCount, double avgRating, int reviewCount)>();

        foreach (var course in publishedCourses)
        {
            var enrollmentCount = await _enrollmentRepository.CountAsync(e => e.CourseId == course.Id, cancellationToken);
            var reviews = await _reviewRepository.GetByCourseIdAsync(course.Id, cancellationToken);
            var approvedReviews = reviews.Where(r => r.IsApproved).ToList();
            var avgRating = approvedReviews.Any() ? approvedReviews.Average(r => r.Rating) : 0;
            var reviewCount = approvedReviews.Count;

            popularCourses.Add((course, enrollmentCount, avgRating, reviewCount));
        }

        // Sort by enrollment count (popularity) and then by rating
        var sortedCourses = popularCourses
            .OrderByDescending(x => x.enrollmentCount)
            .ThenByDescending(x => x.avgRating)
            .Take(request.Limit);

        var result = new List<PopularCourseResponse>();

        foreach (var (course, enrollmentCount, avgRating, reviewCount) in sortedCourses)
        {
            var instructor = await _userRepository.GetByIdAsync(course.InstructorId, cancellationToken);
            var instructorName = instructor?.GetFullName() ?? "Unknown Instructor";

            result.Add(new PopularCourseResponse(
                course.Id,
                course.Title,
                course.ShortDescription,
                instructorName,
                course.Price,
                course.DiscountPrice,
                course.ThumbnailUrl,
                enrollmentCount,
                avgRating,
                reviewCount
            ));
        }

        return result;
    }
}
