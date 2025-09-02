using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;

namespace learnify.ai.api.Features.Payments;

public record HasUserPurchasedCourseQuery(
    int UserId,
    int CourseId
) : IQuery<bool>;

public class HasUserPurchasedCourseValidator : AbstractValidator<HasUserPurchasedCourseQuery>
{
    public HasUserPurchasedCourseValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class HasUserPurchasedCourseHandler : IRequestHandler<HasUserPurchasedCourseQuery, bool>
{
    private readonly IPaymentRepository _paymentRepository;

    public HasUserPurchasedCourseHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<bool> Handle(HasUserPurchasedCourseQuery request, CancellationToken cancellationToken)
    {
        return await _paymentRepository.HasUserPurchasedCourseAsync(request.UserId, request.CourseId, cancellationToken);
    }
}