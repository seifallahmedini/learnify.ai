using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Courses;

public record GetFeaturedCoursesQuery(
    int? CategoryId = null,
    int? InstructorId = null,
    CourseLevel? Level = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<CourseListResponse>;

public class GetFeaturedCoursesValidator : AbstractValidator<GetFeaturedCoursesQuery>
{
    public GetFeaturedCoursesValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price must be 0 or greater");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage("Maximum price must be 0 or greater");
    }
}

public class GetFeaturedCoursesHandler : IRequestHandler<GetFeaturedCoursesQuery, CourseListResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetFeaturedCoursesHandler(
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<CourseListResponse> Handle(GetFeaturedCoursesQuery request, CancellationToken cancellationToken)
    {
        // Get featured courses with all filters
        var courses = await _courseRepository.GetFeaturedCoursesAsync(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var coursesList = courses.ToList();
        var totalCount = await _courseRepository.GetFeaturedCoursesCountAsync(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var courseSummaries = new List<CourseSummaryResponse>();
        
        foreach (var course in coursesList)
        {
            var instructor = await _userRepository.GetByIdAsync(course.InstructorId, cancellationToken);
            var category = await _categoryRepository.GetByIdAsync(course.CategoryId, cancellationToken);

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
                0,
                0.0,
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