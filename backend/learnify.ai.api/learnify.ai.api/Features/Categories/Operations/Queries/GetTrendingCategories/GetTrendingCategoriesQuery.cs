using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Reviews;

namespace learnify.ai.api.Features.Categories;

public record GetTrendingCategoriesQuery(int Limit = 10) : IQuery<IEnumerable<TrendingCategoryResponse>>;

public class GetTrendingCategoriesValidator : AbstractValidator<GetTrendingCategoriesQuery>
{
    public GetTrendingCategoriesValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("Limit must be between 1 and 50");
    }
}

public class GetTrendingCategoriesHandler : IRequestHandler<GetTrendingCategoriesQuery, IEnumerable<TrendingCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IReviewRepository _reviewRepository;

    public GetTrendingCategoriesHandler(
        ICategoryRepository categoryRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IReviewRepository reviewRepository)
    {
        _categoryRepository = categoryRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<IEnumerable<TrendingCategoryResponse>> Handle(GetTrendingCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var activeCategories = categories.Where(c => c.IsActive).ToList();

        var trendingCategories = new List<(Category category, int courseCount, int recentEnrollments, decimal growthRate, decimal avgRating)>();

        // Define "recent" as last 30 days for trending calculation
        var recentDate = DateTime.UtcNow.AddDays(-30);

        foreach (var category in activeCategories)
        {
            var courses = await _courseRepository.GetByCategoryIdAsync(category.Id, cancellationToken);
            var publishedCourses = courses.Where(c => c.IsPublished).ToList();
            var courseCount = publishedCourses.Count;

            if (courseCount == 0)
                continue;

            var recentEnrollments = 0;
            var totalEnrollments = 0;
            var ratings = new List<double>();

            foreach (var course in publishedCourses)
            {
                var enrollments = await _enrollmentRepository.GetByCourseIdAsync(course.Id, cancellationToken);
                var enrollmentsList = enrollments.ToList();

                totalEnrollments += enrollmentsList.Count;
                recentEnrollments += enrollmentsList.Count(e => e.EnrollmentDate >= recentDate);

                var reviews = await _reviewRepository.GetByCourseIdAsync(course.Id, cancellationToken);
                var approvedReviews = reviews.Where(r => r.IsApproved).ToList();
                if (approvedReviews.Any())
                {
                    ratings.Add(approvedReviews.Average(r => r.Rating));
                }
            }

            // Calculate growth rate (recent enrollments vs total enrollments)
            var growthRate = totalEnrollments > 0 ? (decimal)recentEnrollments / totalEnrollments * 100 : 0;

            // Calculate average rating across all courses in category
            var avgRating = ratings.Any() ? (decimal)ratings.Average() : 0;

            trendingCategories.Add((category, courseCount, recentEnrollments, growthRate, avgRating));
        }

        // Sort by recent enrollments and growth rate to determine trending
        var sortedCategories = trendingCategories
            .Where(x => x.recentEnrollments > 0) // Only categories with recent activity
            .OrderByDescending(x => x.recentEnrollments)
            .ThenByDescending(x => x.growthRate)
            .ThenByDescending(x => x.avgRating)
            .Take(request.Limit);

        var result = new List<TrendingCategoryResponse>();

        foreach (var (category, courseCount, recentEnrollments, growthRate, avgRating) in sortedCategories)
        {
            result.Add(new TrendingCategoryResponse(
                category.Id,
                category.Name,
                category.Description,
                category.IconUrl,
                courseCount,
                recentEnrollments,
                growthRate,
                avgRating
            ));
        }

        return result;
    }
}