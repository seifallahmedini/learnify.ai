using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Reviews;

public record GetCourseRatingQuery(
    int CourseId
) : IQuery<CourseRatingResponse>;

public class GetCourseRatingValidator : AbstractValidator<GetCourseRatingQuery>
{
    public GetCourseRatingValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class GetCourseRatingHandler : IRequestHandler<GetCourseRatingQuery, CourseRatingResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCourseRatingHandler(
        IReviewRepository reviewRepository,
        ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CourseRatingResponse> Handle(GetCourseRatingQuery request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get approved reviews for the course
        var approvedReviews = await _reviewRepository.GetApprovedReviewsAsync(request.CourseId, cancellationToken);
        var reviewsList = approvedReviews.ToList();

        var averageRating = reviewsList.Any() ? reviewsList.Average(r => r.Rating) : 0;

        // Calculate rating distribution
        var ratingDistribution = new RatingDistributionResponse(
            reviewsList.Count(r => r.Rating == 1),
            reviewsList.Count(r => r.Rating == 2),
            reviewsList.Count(r => r.Rating == 3),
            reviewsList.Count(r => r.Rating == 4),
            reviewsList.Count(r => r.Rating == 5),
            reviewsList.Count
        );

        return new CourseRatingResponse(
            course.Id,
            course.Title,
            Math.Round(averageRating, 2),
            reviewsList.Count,
            ratingDistribution
        );
    }
}
