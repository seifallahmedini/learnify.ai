using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Payments;

public record GetPaymentByIdQuery(
    int Id
) : IQuery<PaymentResponse?>;

public class GetPaymentByIdValidator : AbstractValidator<GetPaymentByIdQuery>
{
    public GetPaymentByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Payment ID must be greater than 0");
    }
}

public class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQuery, PaymentResponse?>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetPaymentByIdHandler(
        IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<PaymentResponse?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment == null)
            return null;

        // Load related data
        var user = await _userRepository.GetByIdAsync(payment.UserId, cancellationToken);
        var course = await _courseRepository.GetByIdAsync(payment.CourseId, cancellationToken);

        var formattedRefundAmount = payment.RefundAmount.HasValue 
            ? $"{payment.RefundAmount:C} {payment.Currency}" 
            : null;

        return new PaymentResponse(
            payment.Id,
            payment.UserId,
            user?.GetFullName() ?? "Unknown User",
            payment.CourseId,
            course?.Title ?? "Unknown Course",
            payment.Amount,
            payment.Currency,
            payment.GetFormattedAmount(),
            payment.PaymentMethod,
            payment.TransactionId,
            payment.Status,
            payment.PaymentDate,
            payment.RefundDate,
            payment.RefundAmount,
            formattedRefundAmount,
            payment.CreatedAt,
            payment.UpdatedAt
        );
    }
}