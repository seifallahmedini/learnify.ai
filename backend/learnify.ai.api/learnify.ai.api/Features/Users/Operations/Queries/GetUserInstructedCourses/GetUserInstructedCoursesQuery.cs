using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Reviews;

namespace learnify.ai.api.Features.Users;

public record GetUserInstructedCoursesQuery(
    int UserId,
    bool? IsPublished = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<GetUserInstructedCoursesResponse>;

public record GetUserInstructedCoursesResponse(
    IEnumerable<InstructedCourseDto> Courses,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public record InstructedCourseDto(
    int Id,
    string Title,
    string ShortDescription,
    string ThumbnailUrl,
    decimal Price,
    decimal? DiscountPrice,
    bool IsPublished,
    int TotalStudents,
    double AverageRating,
    int TotalReviews,
    DateTime CreatedAt
)
{
    public decimal EffectivePrice => DiscountPrice ?? Price;
    public bool IsDiscounted => DiscountPrice.HasValue && DiscountPrice < Price;
}

public class GetUserInstructedCoursesValidator : AbstractValidator<GetUserInstructedCoursesQuery>
{
    public GetUserInstructedCoursesValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetUserInstructedCoursesHandler : IRequestHandler<GetUserInstructedCoursesQuery, GetUserInstructedCoursesResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IReviewRepository _reviewRepository;

    public GetUserInstructedCoursesHandler(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IReviewRepository reviewRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<GetUserInstructedCoursesResponse> Handle(GetUserInstructedCoursesQuery request, CancellationToken cancellationToken)
    {
        var allCourses = await _courseRepository.GetByInstructorIdAsync(request.UserId, cancellationToken);
        
        // Apply published filter if specified
        if (request.IsPublished.HasValue)
        {
            allCourses = allCourses.Where(c => c.IsPublished == request.IsPublished.Value);
        }

        var totalCount = allCourses.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var paginatedCourses = allCourses
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var courseDtos = new List<InstructedCourseDto>();

        foreach (var course in paginatedCourses)
        {
            var totalStudents = await _enrollmentRepository.GetEnrollmentCountByCourseAsync(course.Id, cancellationToken);
            var averageRating = await _reviewRepository.GetAverageRatingAsync(course.Id, cancellationToken);
            var totalReviews = await _reviewRepository.GetReviewCountAsync(course.Id, cancellationToken);

            courseDtos.Add(new InstructedCourseDto(
                course.Id,
                course.Title,
                course.ShortDescription,
                course.ThumbnailUrl ?? "",
                course.Price,
                course.DiscountPrice,
                course.IsPublished,
                totalStudents,
                averageRating,
                totalReviews,
                course.CreatedAt
            ));
        }

        return new GetUserInstructedCoursesResponse(
            courseDtos,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}
