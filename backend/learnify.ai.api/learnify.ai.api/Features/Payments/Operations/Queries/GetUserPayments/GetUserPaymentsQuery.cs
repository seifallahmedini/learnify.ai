using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Payments;

public record GetUserPaymentsQuery(
    int UserId,
    int? CourseId = null,
    PaymentStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<UserPaymentsResponse>;

public class GetUserPaymentsValidator : AbstractValidator<GetUserPaymentsQuery>
{
    public GetUserPaymentsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .When(x => x.CourseId.HasValue)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be before or equal to To date");
    }
}

public class GetUserPaymentsHandler : IRequestHandler<GetUserPaymentsQuery, UserPaymentsResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetUserPaymentsHandler(
        IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<UserPaymentsResponse> Handle(GetUserPaymentsQuery request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Get user payments
        var payments = await _paymentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var paymentsList = payments.ToList();

        // Apply filters
        if (request.CourseId.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.CourseId == request.CourseId.Value).ToList();
        }

        if (request.Status.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.Status == request.Status.Value).ToList();
        }

        if (request.FromDate.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.PaymentDate >= request.FromDate.Value).ToList();
        }

        if (request.ToDate.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.PaymentDate <= request.ToDate.Value).ToList();
        }

        // Get total count before pagination
        var totalCount = paymentsList.Count;

        // Apply pagination
        var pagedPayments = paymentsList
            .OrderByDescending(p => p.PaymentDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Build payment summaries
        var paymentSummaries = new List<PaymentSummaryResponse>();
        foreach (var payment in pagedPayments)
        {
            var course = await _courseRepository.GetByIdAsync(payment.CourseId, cancellationToken);
            
            paymentSummaries.Add(new PaymentSummaryResponse(
                payment.Id,
                payment.UserId,
                user.GetFullName(),
                payment.CourseId,
                course?.Title ?? "Unknown Course",
                payment.Amount,
                payment.GetFormattedAmount(),
                payment.Status,
                payment.PaymentDate
            ));
        }

        // Calculate statistics
        var completedPayments = paymentsList.Where(p => p.Status == PaymentStatus.Completed).ToList();
        var refundedPayments = paymentsList.Where(p => p.Status == PaymentStatus.Refunded).ToList();
        var failedPayments = paymentsList.Where(p => p.Status == PaymentStatus.Failed).ToList();

        var totalSpent = completedPayments.Sum(p => p.Amount);
        var totalRefunded = refundedPayments.Sum(p => p.RefundAmount ?? 0);

        var stats = new PaymentStatsResponse(
            paymentsList.Count,
            completedPayments.Count,
            refundedPayments.Count,
            failedPayments.Count,
            totalSpent,
            $"{totalSpent:C} USD",
            totalRefunded,
            $"{totalRefunded:C} USD"
        );

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new UserPaymentsResponse(
            user.Id,
            user.GetFullName(),
            paymentSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            stats
        );
    }
}