using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Reviews;

public record GetCourseReviewsQuery(
    int CourseId,
    int Page = 1,
    int PageSize = 10,
    int? Rating = null,
    bool? IsApproved = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IQuery<CourseReviewsResponse>;

public class GetCourseReviewsValidator : AbstractValidator<GetCourseReviewsQuery>
{
    public GetCourseReviewsValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be before or equal to To date");
    }
}

public class GetCourseReviewsHandler : IRequestHandler<GetCourseReviewsQuery, CourseReviewsResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCourseReviewsHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CourseReviewsResponse> Handle(GetCourseReviewsQuery request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get course reviews
        var reviews = await _reviewRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var reviewsList = reviews.ToList();

        // Apply filters
        if (request.IsApproved.HasValue)
        {
            reviewsList = reviewsList.Where(r => r.IsApproved == request.IsApproved.Value).ToList();
        }

        if (request.Rating.HasValue)
        {
            reviewsList = reviewsList.Where(r => r.Rating == request.Rating.Value).ToList();
        }

        if (request.FromDate.HasValue)
        {
            reviewsList = reviewsList.Where(r => r.CreatedAt >= request.FromDate.Value).ToList();
        }

        if (request.ToDate.HasValue)
        {
            reviewsList = reviewsList.Where(r => r.CreatedAt <= request.ToDate.Value).ToList();
        }

        // Get total count before pagination
        var totalCount = reviewsList.Count;

        // Apply pagination
        var pagedReviews = reviewsList
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Build review summaries
        var reviewSummaries = new List<ReviewSummaryResponse>();
        foreach (var review in pagedReviews)
        {
            var user = await _userRepository.GetByIdAsync(review.UserId, cancellationToken);
            reviewSummaries.Add(new ReviewSummaryResponse(
                review.Id,
                review.CourseId,
                review.UserId,
                user?.GetFullName() ?? "Unknown User",
                review.Rating,
                review.Comment,
                review.IsApproved,
                review.CreatedAt
            ));
        }

        // Calculate statistics
        var approvedReviews = reviews.Where(r => r.IsApproved).ToList();
        var averageRating = approvedReviews.Any() ? approvedReviews.Average(r => r.Rating) : 0;

        // Calculate rating distribution
        var ratingDistribution = new RatingDistributionResponse(
            approvedReviews.Count(r => r.Rating == 1),
            approvedReviews.Count(r => r.Rating == 2),
            approvedReviews.Count(r => r.Rating == 3),
            approvedReviews.Count(r => r.Rating == 4),
            approvedReviews.Count(r => r.Rating == 5),
            approvedReviews.Count
        );

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new CourseReviewsResponse(
            course.Id,
            course.Title,
            reviewSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            Math.Round(averageRating, 2),
            ratingDistribution
        );
    }
}