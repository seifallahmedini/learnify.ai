using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Reviews;

namespace learnify.ai.api.Features.Categories;

public record GetCategoryCoursesCountQuery(int Id) : IQuery<CategoryAnalyticsResponse?>;

public class GetCategoryCoursesCountValidator : AbstractValidator<GetCategoryCoursesCountQuery>
{
    public GetCategoryCoursesCountValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category ID must be greater than 0");
    }
}

public class GetCategoryCoursesCountHandler : IRequestHandler<GetCategoryCoursesCountQuery, CategoryAnalyticsResponse?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IReviewRepository _reviewRepository;

    public GetCategoryCoursesCountHandler(
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

    public async Task<CategoryAnalyticsResponse?> Handle(GetCategoryCoursesCountQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (category == null)
            return null;

        // Get direct course count for this category
        var directCourseCount = await _courseRepository.CountAsync(c => c.CategoryId == request.Id, cancellationToken);

        // Get all subcategories to calculate total course count
        var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);
        var subcategoryIds = GetAllSubcategoryIds(request.Id, allCategories.ToList());
        subcategoryIds.Add(request.Id); // Include the category itself

        var totalCourseCount = 0;
        foreach (var categoryId in subcategoryIds)
        {
            var count = await _courseRepository.CountAsync(c => c.CategoryId == categoryId, cancellationToken);
            totalCourseCount += count;
        }

        // Get subcategory count
        var subcategoryCount = await _categoryRepository.CountAsync(c => c.ParentCategoryId == request.Id, cancellationToken);

        // Calculate total enrollments for all courses in this category and subcategories
        var totalEnrollments = 0;
        var allCourses = await _courseRepository.GetAllAsync(cancellationToken);
        var categoryCoursesIds = allCourses.Where(c => subcategoryIds.Contains(c.CategoryId)).Select(c => c.Id);

        foreach (var courseId in categoryCoursesIds)
        {
            var enrollmentCount = await _enrollmentRepository.CountAsync(e => e.CourseId == courseId, cancellationToken);
            totalEnrollments += enrollmentCount;
        }

        // Calculate average rating for courses in this category
        var averageRating = 0m;
        if (categoryCoursesIds.Any())
        {
            var ratings = new List<double>();
            foreach (var courseId in categoryCoursesIds)
            {
                var reviews = await _reviewRepository.GetByCourseIdAsync(courseId, cancellationToken);
                var approvedReviews = reviews.Where(r => r.IsApproved).ToList();
                if (approvedReviews.Any())
                {
                    ratings.Add(approvedReviews.Average(r => r.Rating));
                }
            }
            
            if (ratings.Any())
            {
                averageRating = (decimal)ratings.Average();
            }
        }

        return new CategoryAnalyticsResponse(
            category.Id,
            category.Name,
            directCourseCount,
            totalCourseCount,
            subcategoryCount,
            totalEnrollments,
            averageRating,
            DateTime.UtcNow
        );
    }

    private List<int> GetAllSubcategoryIds(int parentId, List<Category> allCategories)
    {
        var result = new List<int>();
        var directChildren = allCategories.Where(c => c.ParentCategoryId == parentId).ToList();

        foreach (var child in directChildren)
        {
            result.Add(child.Id);
            var grandchildren = GetAllSubcategoryIds(child.Id, allCategories);
            result.AddRange(grandchildren);
        }

        return result;
    }
}