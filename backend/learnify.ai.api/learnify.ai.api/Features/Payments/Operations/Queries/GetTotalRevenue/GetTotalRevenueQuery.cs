using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Payments;

public record GetTotalRevenueQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? Currency = null
) : IQuery<TotalRevenueResponse>;

public class GetTotalRevenueValidator : AbstractValidator<GetTotalRevenueQuery>
{
    public GetTotalRevenueValidator()
    {
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be before or equal to To date");

        RuleFor(x => x.Currency)
            .Length(3)
            .When(x => !string.IsNullOrEmpty(x.Currency))
            .WithMessage("Currency must be a valid 3-character code");
    }
}

public class GetTotalRevenueHandler : IRequestHandler<GetTotalRevenueQuery, TotalRevenueResponse>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetTotalRevenueHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<TotalRevenueResponse> Handle(GetTotalRevenueQuery request, CancellationToken cancellationToken)
    {
        // Get all completed payments
        var completedPayments = await _paymentRepository.GetByStatusAsync(PaymentStatus.Completed, cancellationToken);
        var paymentsList = completedPayments.ToList();

        // Apply filters
        var currency = request.Currency ?? "USD";
        paymentsList = paymentsList.Where(p => p.Currency == currency).ToList();

        if (request.FromDate.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.PaymentDate >= request.FromDate.Value).ToList();
        }

        if (request.ToDate.HasValue)
        {
            paymentsList = paymentsList.Where(p => p.PaymentDate <= request.ToDate.Value).ToList();
        }

        // Calculate total revenue
        var totalRevenue = paymentsList.Sum(p => p.Amount);

        // Calculate this month's revenue
        var thisMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var thisMonthEnd = thisMonthStart.AddMonths(1).AddDays(-1);
        var thisMonth = paymentsList
            .Where(p => p.PaymentDate >= thisMonthStart && p.PaymentDate <= thisMonthEnd)
            .Sum(p => p.Amount);

        // Calculate last month's revenue
        var lastMonthStart = thisMonthStart.AddMonths(-1);
        var lastMonthEnd = thisMonthStart.AddDays(-1);
        var lastMonth = paymentsList
            .Where(p => p.PaymentDate >= lastMonthStart && p.PaymentDate <= lastMonthEnd)
            .Sum(p => p.Amount);

        // Calculate growth rate
        var growthRate = lastMonth > 0 ? (double)((thisMonth - lastMonth) / lastMonth) * 100 : 0;

        return new TotalRevenueResponse(
            totalRevenue,
            $"{totalRevenue:C} {currency}",
            thisMonth,
            $"{thisMonth:C} {currency}",
            lastMonth,
            $"{lastMonth:C} {currency}",
            Math.Round(growthRate, 2),
            currency,
            DateTime.UtcNow
        );
    }
}