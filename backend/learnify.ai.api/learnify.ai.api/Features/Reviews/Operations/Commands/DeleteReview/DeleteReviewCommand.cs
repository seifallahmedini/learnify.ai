using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

namespace learnify.ai.api.Features.Reviews;

public record DeleteReviewCommand(
    int Id
) : ICommand<bool>;

public class DeleteReviewValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review ID must be greater than 0");
    }
}

public class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;

    public DeleteReviewHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review == null)
            return false;

        return await _reviewRepository.DeleteAsync(review, cancellationToken);
    }
}
