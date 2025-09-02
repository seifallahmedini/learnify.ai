using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Payments;

public record GetPaymentByTransactionIdQuery(
    string TransactionId
) : IQuery<PaymentResponse?>;

public class GetPaymentByTransactionIdValidator : AbstractValidator<GetPaymentByTransactionIdQuery>
{
    public GetPaymentByTransactionIdValidator()
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .WithMessage("Transaction ID is required");
    }
}

public class GetPaymentByTransactionIdHandler : IRequestHandler<GetPaymentByTransactionIdQuery, PaymentResponse?>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetPaymentByTransactionIdHandler(
        IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<PaymentResponse?> Handle(GetPaymentByTransactionIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByTransactionIdAsync(request.TransactionId, cancellationToken);
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