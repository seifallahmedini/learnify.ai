using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Reviews;

public record UpdateReviewCommand(
    int Id,
    int? Rating = null,
    string? Comment = null
) : ICommand<ReviewResponse?>;

public class UpdateReviewValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review ID must be greater than 0");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .When(x => x.Rating.HasValue)
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Comment))
            .WithMessage("Comment cannot exceed 1000 characters");
    }
}

public class UpdateReviewHandler : IRequestHandler<UpdateReviewCommand, ReviewResponse?>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public UpdateReviewHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<ReviewResponse?> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
            return null;

        // Update only provided fields
        if (request.Rating.HasValue)
        {
            review.Rating = request.Rating.Value;
        }

        if (request.Comment != null)
        {
            review.Comment = request.Comment;
        }

        // If review was modified, it needs re-approval
        if (request.Rating.HasValue || request.Comment != null)
        {
            review.IsApproved = false;
        }

        review.UpdatedAt = DateTime.UtcNow;

        var updatedReview = await _reviewRepository.UpdateAsync(review, cancellationToken);

        // Load related data
        var user = await _userRepository.GetByIdAsync(updatedReview.UserId, cancellationToken);
        var course = await _courseRepository.GetByIdAsync(updatedReview.CourseId, cancellationToken);

        return new ReviewResponse(
            updatedReview.Id,
            updatedReview.CourseId,
            course?.Title ?? "Unknown Course",
            updatedReview.UserId,
            user?.GetFullName() ?? "Unknown User",
            updatedReview.Rating,
            updatedReview.Comment,
            updatedReview.IsApproved,
            updatedReview.CreatedAt,
            updatedReview.UpdatedAt
        );
    }
}
