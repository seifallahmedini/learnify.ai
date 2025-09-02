using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;

namespace learnify.ai.api.Features.Payments;

public record ProcessRefundCommand(
    int PaymentId,
    decimal? RefundAmount = null,
    string? Reason = null
) : ICommand<RefundResponse>;

public class ProcessRefundValidator : AbstractValidator<ProcessRefundCommand>
{
    public ProcessRefundValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithMessage("Payment ID must be greater than 0");

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0)
            .When(x => x.RefundAmount.HasValue)
            .WithMessage("Refund amount must be greater than 0");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Refund reason cannot exceed 500 characters");
    }
}

public class ProcessRefundHandler : IRequestHandler<ProcessRefundCommand, RefundResponse>
{
    private readonly IPaymentRepository _paymentRepository;

    public ProcessRefundHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<RefundResponse> Handle(ProcessRefundCommand request, CancellationToken cancellationToken)
    {
        // Get the payment
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
            throw new ArgumentException($"Payment with ID {request.PaymentId} not found");

        // Validate payment can be refunded
        if (!payment.CanBeRefunded())
            throw new InvalidOperationException("Payment cannot be refunded. It may already be refunded or not completed.");

        // Determine refund amount (full refund if not specified)
        var refundAmount = request.RefundAmount ?? payment.Amount;
        
        // Validate refund amount doesn't exceed original payment
        if (refundAmount > payment.Amount)
            throw new ArgumentException("Refund amount cannot exceed original payment amount");

        // Process refund with payment gateway
        var refundResult = await ProcessRefundWithGateway(payment, refundAmount, request.Reason);
        
        if (!refundResult.Success)
            throw new InvalidOperationException($"Refund processing failed: {refundResult.ErrorMessage}");

        // Update payment record
        payment.ProcessRefund(refundAmount);
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        return new RefundResponse(
            payment.Id,
            payment.TransactionId,
            refundAmount,
            $"{refundAmount:C} {payment.Currency}",
            payment.RefundDate!.Value,
            payment.Status,
            "Refund processed successfully"
        );
    }

    private async Task<RefundGatewayResult> ProcessRefundWithGateway(Payment payment, decimal refundAmount, string? reason)
    {
        // Mock implementation - in reality, you would call the payment gateway's refund API
        await Task.Delay(100, CancellationToken.None);
        
        // Mock high success rate for refunds
        var isSuccess = Random.Shared.NextDouble() < 0.98;
        
        return new RefundGatewayResult
        {
            Success = isSuccess,
            ErrorMessage = isSuccess ? null : "Refund declined by payment processor"
        };
    }
}

public class RefundGatewayResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}