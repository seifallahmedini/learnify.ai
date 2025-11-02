using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Reviews;

namespace learnify.ai.api.Features.Courses;

public record GetCoursesQuery(
    int? CategoryId = null,
    int? InstructorId = null,
    CourseLevel? Level = null,
    bool? IsPublished = null,
    bool? IsFeatured = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<CourseListResponse>;

public class GetCoursesValidator : AbstractValidator<GetCoursesQuery>
{
    public GetCoursesValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue)
            .WithMessage("Category ID must be greater than 0 when specified");

        RuleFor(x => x.InstructorId)
            .GreaterThan(0)
            .When(x => x.InstructorId.HasValue)
            .WithMessage("Instructor ID must be greater than 0 when specified");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price must be 0 or greater");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice)
            .When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue)
            .WithMessage("Maximum price must be greater than or equal to minimum price");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetCoursesHandler : IRequestHandler<GetCoursesQuery, CourseListResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IReviewRepository _reviewRepository;

    public GetCoursesHandler(
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IEnrollmentRepository enrollmentRepository,
        IReviewRepository reviewRepository)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _enrollmentRepository = enrollmentRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<CourseListResponse> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var allCourses = await _courseRepository.GetAllAsync(cancellationToken);
        
        // Apply filters
        var filteredCourses = allCourses.AsQueryable();

        if (request.CategoryId.HasValue)
        {
            filteredCourses = filteredCourses.Where(c => c.CategoryId == request.CategoryId.Value);
        }

        if (request.InstructorId.HasValue)
        {
            filteredCourses = filteredCourses.Where(c => c.InstructorId == request.InstructorId.Value);
        }

        if (request.Level.HasValue)
        {
            filteredCourses = filteredCourses.Where(c => c.Level == request.Level.Value);
        }

        if (request.IsPublished.HasValue)
        {
            filteredCourses = filteredCourses.Where(c => c.IsPublished == request.IsPublished.Value);
        }

        if (request.IsFeatured.HasValue)
        {
            filteredCourses = filteredCourses.Where(c => c.IsFeatured == request.IsFeatured.Value);
        }

        if (request.MinPrice.HasValue)
        {
            filteredCourses = filteredCourses.Where(c => c.GetEffectivePrice() >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            filteredCourses = filteredCourses.Where(c => c.GetEffectivePrice() <= request.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            filteredCourses = filteredCourses.Where(c => 
                c.Title.ToLower().Contains(searchTerm) ||
                c.Description.ToLower().Contains(searchTerm) ||
                c.ShortDescription.ToLower().Contains(searchTerm));
        }

        var totalCount = filteredCourses.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var paginatedCourses = filteredCourses
            .OrderBy(c => c.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var courseSummaries = new List<CourseSummaryResponse>();

        foreach (var course in paginatedCourses)
        {
            var instructor = await _userRepository.GetByIdAsync(course.InstructorId, cancellationToken);
            var category = await _categoryRepository.GetByIdAsync(course.CategoryId, cancellationToken);
            var totalStudents = await _enrollmentRepository.GetEnrollmentCountByCourseAsync(course.Id, cancellationToken);
            var averageRating = await _reviewRepository.GetAverageRatingAsync(course.Id, cancellationToken);

            courseSummaries.Add(new CourseSummaryResponse(
                course.Id,
                course.Title,
                course.ShortDescription,
                instructor?.GetFullName() ?? "Unknown Instructor",
                category?.Name ?? "Unknown Category",
                course.Price,
                course.DiscountPrice,
                course.Level,
                course.ThumbnailUrl,
                course.IsPublished,
                course.IsFeatured,
                totalStudents,
                averageRating,
                course.CreatedAt
            ));
        }

        return new CourseListResponse(
            courseSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}