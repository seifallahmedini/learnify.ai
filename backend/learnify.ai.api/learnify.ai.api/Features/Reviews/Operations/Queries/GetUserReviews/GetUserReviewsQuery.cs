using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Reviews;

public record GetUserReviewsQuery(
    int UserId,
    int Page = 1,
    int PageSize = 10,
    bool? IsApproved = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IQuery<UserReviewsResponse>;

public class GetUserReviewsValidator : AbstractValidator<GetUserReviewsQuery>
{
    public GetUserReviewsValidator()
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

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be before or equal to To date");
    }
}

public class GetUserReviewsHandler : IRequestHandler<GetUserReviewsQuery, UserReviewsResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetUserReviewsHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<UserReviewsResponse> Handle(GetUserReviewsQuery request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Get user reviews
        var reviews = await _reviewRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var reviewsList = reviews.ToList();

        // Apply filters
        if (request.IsApproved.HasValue)
        {
            reviewsList = reviewsList.Where(r => r.IsApproved == request.IsApproved.Value).ToList();
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
        var reviewSummaries = new List<UserReviewSummaryResponse>();
        foreach (var review in pagedReviews)
        {
            var course = await _courseRepository.GetByIdAsync(review.CourseId, cancellationToken);
            reviewSummaries.Add(new UserReviewSummaryResponse(
                review.Id,
                review.CourseId,
                course?.Title ?? "Unknown Course",
                review.Rating,
                review.Comment,
                review.IsApproved,
                review.CreatedAt
            ));
        }

        // Calculate average rating given by user
        var averageRating = reviewsList.Any() ? reviewsList.Average(r => r.Rating) : 0;

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new UserReviewsResponse(
            user.Id,
            user.GetFullName(),
            reviewSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            Math.Round(averageRating, 2)
        );
    }
}