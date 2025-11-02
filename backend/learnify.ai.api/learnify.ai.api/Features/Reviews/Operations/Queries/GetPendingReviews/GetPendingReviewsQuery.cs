using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Reviews;

public record GetPendingReviewsQuery(
    int Page = 1,
    int PageSize = 10,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IQuery<PendingReviewsResponse>;

public class GetPendingReviewsValidator : AbstractValidator<GetPendingReviewsQuery>
{
    public GetPendingReviewsValidator()
    {
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

public class GetPendingReviewsHandler : IRequestHandler<GetPendingReviewsQuery, PendingReviewsResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetPendingReviewsHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<PendingReviewsResponse> Handle(GetPendingReviewsQuery request, CancellationToken cancellationToken)
    {
        // Get all pending reviews
        var pendingReviews = await _reviewRepository.GetPendingReviewsAsync(cancellationToken);
        var reviewsList = pendingReviews.ToList();

        // Apply date filters
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
            .OrderBy(r => r.CreatedAt) // Oldest first for moderation
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Build response items
        var pendingItems = new List<PendingReviewItemResponse>();
        foreach (var review in pagedReviews)
        {
            var user = await _userRepository.GetByIdAsync(review.UserId, cancellationToken);
            var course = await _courseRepository.GetByIdAsync(review.CourseId, cancellationToken);
            
            var daysPending = (DateTime.UtcNow - review.CreatedAt).Days;

            pendingItems.Add(new PendingReviewItemResponse(
                review.Id,
                review.CourseId,
                course?.Title ?? "Unknown Course",
                review.UserId,
                user?.GetFullName() ?? "Unknown User",
                review.Rating,
                review.Comment,
                review.CreatedAt,
                daysPending
            ));
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new PendingReviewsResponse(
            pendingItems,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}
