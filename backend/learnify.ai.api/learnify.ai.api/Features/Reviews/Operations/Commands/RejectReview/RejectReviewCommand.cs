using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Reviews;

public record RejectReviewCommand(
    int Id
) : ICommand<ReviewModerationResponse?>;

public class RejectReviewValidator : AbstractValidator<RejectReviewCommand>
{
    public RejectReviewValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review ID must be greater than 0");
    }
}

public class RejectReviewHandler : IRequestHandler<RejectReviewCommand, ReviewModerationResponse?>
{
    private readonly IReviewRepository _reviewRepository;

    public RejectReviewHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewModerationResponse?> Handle(RejectReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
            return null;

        review.IsApproved = false;
        review.UpdatedAt = DateTime.UtcNow;
        await _reviewRepository.UpdateAsync(review, cancellationToken);

        return new ReviewModerationResponse(
            review.Id,
            review.IsApproved,
            review.UpdatedAt,
            "Review rejected successfully"
        );
    }
}
