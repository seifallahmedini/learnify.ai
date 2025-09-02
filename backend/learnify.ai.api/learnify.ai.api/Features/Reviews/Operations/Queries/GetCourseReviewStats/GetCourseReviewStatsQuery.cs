using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Reviews;

public record GetCourseReviewStatsQuery(
    int CourseId
) : IQuery<CourseReviewStatsResponse>;

public class GetCourseReviewStatsValidator : AbstractValidator<GetCourseReviewStatsQuery>
{
    public GetCourseReviewStatsValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class GetCourseReviewStatsHandler : IRequestHandler<GetCourseReviewStatsQuery, CourseReviewStatsResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCourseReviewStatsHandler(
        IReviewRepository reviewRepository,
        ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CourseReviewStatsResponse> Handle(GetCourseReviewStatsQuery request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get all reviews for the course
        var allReviews = await _reviewRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var reviewsList = allReviews.ToList();

        var approvedReviews = reviewsList.Where(r => r.IsApproved).ToList();
        var pendingReviews = reviewsList.Where(r => !r.IsApproved).ToList();

        var totalReviews = reviewsList.Count;
        var approvedCount = approvedReviews.Count;
        var pendingCount = pendingReviews.Count;

        var averageRating = approvedReviews.Any() ? approvedReviews.Average(r => r.Rating) : 0;
        var approvalRate = totalReviews > 0 ? (double)approvedCount / totalReviews * 100 : 0;

        // Calculate rating distribution (approved reviews only)
        var ratingDistribution = new RatingDistributionResponse(
            approvedReviews.Count(r => r.Rating == 1),
            approvedReviews.Count(r => r.Rating == 2),
            approvedReviews.Count(r => r.Rating == 3),
            approvedReviews.Count(r => r.Rating == 4),
            approvedReviews.Count(r => r.Rating == 5),
            approvedCount
        );

        var lastReviewDate = reviewsList.Any() ? reviewsList.Max(r => r.CreatedAt) : (DateTime?)null;

        return new CourseReviewStatsResponse(
            course.Id,
            course.Title,
            Math.Round(averageRating, 2),
            totalReviews,
            approvedCount,
            pendingCount,
            Math.Round(approvalRate, 2),
            ratingDistribution,
            lastReviewDate,
            DateTime.UtcNow
        );
    }
}