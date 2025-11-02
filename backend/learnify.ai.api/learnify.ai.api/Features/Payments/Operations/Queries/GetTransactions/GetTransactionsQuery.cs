using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;

using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Payments;

public record GetTransactionsQuery(
    int? UserId = null,
    int? CourseId = null,
    PaymentStatus? Status = null,
    PaymentMethod? PaymentMethod = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<TransactionsResponse>;

public class GetTransactionsValidator : AbstractValidator<GetTransactionsQuery>
{
    public GetTransactionsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .When(x => x.UserId.HasValue)
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

public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, TransactionsResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetTransactionsHandler(
        IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<TransactionsResponse> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        // Get all payments
        var payments = await _paymentRepository.GetAllAsync(cancellationToken);
        var paymentsList = payments.ToList();

        // Apply filters
        if (request.UserId.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.UserId == request.UserId.Value).ToList();
        }

        if (request.CourseId.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.CourseId == request.CourseId.Value).ToList();
        }

        if (request.Status.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.Status == request.Status.Value).ToList();
        }

        if (request.PaymentMethod.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.PaymentMethod == request.PaymentMethod.Value).ToList();
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

        // Build transaction summaries
        var transactionSummaries = new List<PaymentSummaryResponse>();
        foreach (var payment in pagedPayments)
        {
            var user = await _userRepository.GetByIdAsync(payment.UserId, cancellationToken);
            var course = await _courseRepository.GetByIdAsync(payment.CourseId, cancellationToken);
            
            transactionSummaries.Add(new PaymentSummaryResponse(
                payment.Id,
                payment.UserId,
                user?.GetFullName() ?? "Unknown User",
                payment.CourseId,
                course?.Title ?? "Unknown Course",
                payment.Amount,
                payment.GetFormattedAmount(),
                payment.Status,
                payment.PaymentDate
            ));
        }

        // Calculate statistics
        var totalRevenue = paymentsList.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);
        var averageTransactionValue = paymentsList.Any() ? paymentsList.Average(p => p.Amount) : 0;

        var statusBreakdown = paymentsList
            .GroupBy(p => p.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        var methodBreakdown = paymentsList
            .GroupBy(p => p.PaymentMethod)
            .ToDictionary(g => g.Key, g => g.Count());

        var stats = new TransactionStatsResponse(
            paymentsList.Count,
            totalRevenue,
            $"{totalRevenue:C} USD",
            averageTransactionValue,
            $"{averageTransactionValue:C} USD",
            statusBreakdown,
            methodBreakdown
        );

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new TransactionsResponse(
            transactionSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            stats
        );
    }
}
