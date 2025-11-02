using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Reviews;

public record ApproveReviewCommand(
    int Id
) : ICommand<ReviewModerationResponse?>;

public class ApproveReviewValidator : AbstractValidator<ApproveReviewCommand>
{
    public ApproveReviewValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review ID must be greater than 0");
    }
}

public class ApproveReviewHandler : IRequestHandler<ApproveReviewCommand, ReviewModerationResponse?>
{
    private readonly IReviewRepository _reviewRepository;

    public ApproveReviewHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewModerationResponse?> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
            return null;

        review.Approve();
        await _reviewRepository.UpdateAsync(review, cancellationToken);

        return new ReviewModerationResponse(
            review.Id,
            review.IsApproved,
            review.UpdatedAt,
            "Review approved successfully"
        );
    }
}
