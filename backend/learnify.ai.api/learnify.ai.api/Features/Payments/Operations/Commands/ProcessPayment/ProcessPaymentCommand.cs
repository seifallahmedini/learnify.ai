using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;

namespace learnify.ai.api.Features.Payments;

public record ProcessPaymentCommand(
    int UserId,
    int CourseId,
    decimal Amount,
    string Currency = "USD",
    PaymentMethod PaymentMethod = PaymentMethod.CreditCard,
    string? PaymentMethodDetails = null
) : ICommand<ProcessPaymentResponse>;

public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a valid 3-character code");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("Payment method must be valid");
    }
}

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, ProcessPaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public ProcessPaymentHandler(
        IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<ProcessPaymentResponse> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Check if user has already purchased this course
        var hasPurchased = await _paymentRepository.HasUserPurchasedCourseAsync(request.UserId, request.CourseId, cancellationToken);
        if (hasPurchased)
            throw new InvalidOperationException("User has already purchased this course");

        // Validate payment amount matches course price
        var coursePrice = course.GetEffectivePrice();
        if (Math.Abs(request.Amount - coursePrice) > 0.01m)
            throw new ArgumentException($"Payment amount {request.Amount} does not match course price {coursePrice}");

        // Generate transaction ID
        var transactionId = GenerateTransactionId();

        // Create payment record
        var payment = new Payment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            Amount = request.Amount,
            Currency = request.Currency,
            PaymentMethod = request.PaymentMethod,
            TransactionId = transactionId,
            Status = PaymentStatus.Pending,
            PaymentDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Process payment (this would integrate with actual payment gateway)
        var paymentResult = await ProcessWithPaymentGateway(payment, request.PaymentMethodDetails);
        
        payment.Status = paymentResult.Success ? PaymentStatus.Completed : PaymentStatus.Failed;
        payment.UpdatedAt = DateTime.UtcNow;

        var createdPayment = await _paymentRepository.CreateAsync(payment, cancellationToken);

        // If payment successful, create enrollment
        if (paymentResult.Success)
        {
            var enrollment = new Enrollment
            {
                UserId = request.UserId,
                CourseId = request.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active,
                Progress = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _enrollmentRepository.CreateAsync(enrollment, cancellationToken);
        }

        var message = paymentResult.Success 
            ? "Payment processed successfully and enrollment created"
            : $"Payment failed: {paymentResult.ErrorMessage}";

        return new ProcessPaymentResponse(
            createdPayment.Id,
            createdPayment.TransactionId,
            createdPayment.Status,
            createdPayment.Amount,
            createdPayment.GetFormattedAmount(),
            createdPayment.PaymentDate,
            message
        );
    }

    private string GenerateTransactionId()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    private async Task<PaymentGatewayResult> ProcessWithPaymentGateway(Payment payment, string? paymentMethodDetails)
    {
        // This is a mock implementation. In reality, you would integrate with actual payment gateways
        // like Stripe, PayPal, Square, etc.
        
        await Task.Delay(100, CancellationToken.None); // Simulate API call
        
        // Mock success rate (95% success for demonstration)
        var isSuccess = Random.Shared.NextDouble() < 0.95;
        
        return new PaymentGatewayResult
        {
            Success = isSuccess,
            ErrorMessage = isSuccess ? null : "Payment declined by card issuer"
        };
    }
}

public class PaymentGatewayResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}