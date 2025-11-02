using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Reviews;

public record GetReviewByIdQuery(
    int Id
) : IQuery<ReviewResponse?>;

public class GetReviewByIdValidator : AbstractValidator<GetReviewByIdQuery>
{
    public GetReviewByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review ID must be greater than 0");
    }
}

public class GetReviewByIdHandler : IRequestHandler<GetReviewByIdQuery, ReviewResponse?>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetReviewByIdHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<ReviewResponse?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
            return null;

        // Load related data
        var user = await _userRepository.GetByIdAsync(review.UserId, cancellationToken);
        var course = await _courseRepository.GetByIdAsync(review.CourseId, cancellationToken);

        return new ReviewResponse(
            review.Id,
            review.CourseId,
            course?.Title ?? "Unknown Course",
            review.UserId,
            user?.GetFullName() ?? "Unknown User",
            review.Rating,
            review.Comment,
            review.IsApproved,
            review.CreatedAt,
            review.UpdatedAt
        );
    }
}
