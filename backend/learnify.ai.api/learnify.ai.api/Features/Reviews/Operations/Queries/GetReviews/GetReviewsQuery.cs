using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Reviews;

public record GetReviewsQuery(
    int? CourseId = null,
    int? UserId = null,
    bool? IsApproved = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<ReviewListResponse>;

public class GetReviewsValidator : AbstractValidator<GetReviewsQuery>
{
    public GetReviewsValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .When(x => x.CourseId.HasValue)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .When(x => x.UserId.HasValue)
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

public class GetReviewsHandler : IRequestHandler<GetReviewsQuery, ReviewListResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;

    public GetReviewsHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
    }

    public async Task<ReviewListResponse> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        // Get all reviews based on filters
        IEnumerable<Review> reviews;

        if (request.CourseId.HasValue)
        {
            reviews = await _reviewRepository.GetByCourseIdAsync(request.CourseId.Value, cancellationToken);
        }
        else if (request.UserId.HasValue)
        {
            reviews = await _reviewRepository.GetByUserIdAsync(request.UserId.Value, cancellationToken);
        }
        else
        {
            reviews = await _reviewRepository.GetAllAsync(cancellationToken);
        }

        // Apply approval filter
        if (request.IsApproved.HasValue)
        {
            reviews = reviews.Where(r => r.IsApproved == request.IsApproved.Value);
        }

        var reviewsList = reviews.OrderByDescending(r => r.CreatedAt).ToList();

        // Get total count before pagination
        var totalCount = reviewsList.Count;

        // Apply pagination
        var pagedReviews = reviewsList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Build response
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

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new ReviewListResponse(
            reviewSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}