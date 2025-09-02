using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Payments;

public record GetInstructorRevenueQuery(
    int InstructorId,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? Currency = null
) : IQuery<InstructorRevenueResponse>;

public class GetInstructorRevenueValidator : AbstractValidator<GetInstructorRevenueQuery>
{
    public GetInstructorRevenueValidator()
    {
        RuleFor(x => x.InstructorId)
            .GreaterThan(0)
            .WithMessage("Instructor ID must be greater than 0");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be before or equal to To date");
    }
}

public class GetInstructorRevenueHandler : IRequestHandler<GetInstructorRevenueQuery, InstructorRevenueResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetInstructorRevenueHandler(
        IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<InstructorRevenueResponse> Handle(GetInstructorRevenueQuery request, CancellationToken cancellationToken)
    {
        // Verify instructor exists
        var instructor = await _userRepository.GetByIdAsync(request.InstructorId, cancellationToken);
        if (instructor == null)
            throw new ArgumentException($"Instructor with ID {request.InstructorId} not found");

        var currency = request.Currency ?? "USD";

        // Get instructor's courses
        var instructorCourses = await _courseRepository.GetByInstructorIdAsync(request.InstructorId, cancellationToken);
        var courseIds = instructorCourses.Select(c => c.Id).ToHashSet();

        // Get all payments for instructor's courses
        var allPayments = await _paymentRepository.GetAllAsync(cancellationToken);
        var instructorPayments = allPayments
            .Where(p => courseIds.Contains(p.CourseId) && 
                       p.Status == PaymentStatus.Completed && 
                       p.Currency == currency)
            .ToList();

        // Apply date filters
        if (request.FromDate.HasValue)
        {
            instructorPayments = instructorPayments.Where(p => p.PaymentDate >= request.FromDate.Value).ToList();
        }

        if (request.ToDate.HasValue)
        {
            instructorPayments = instructorPayments.Where(p => p.PaymentDate <= request.ToDate.Value).ToList();
        }

        // Calculate revenue metrics
        var totalRevenue = instructorPayments.Sum(p => p.Amount);

        var thisMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var thisMonthEnd = thisMonthStart.AddMonths(1).AddDays(-1);
        var thisMonth = instructorPayments
            .Where(p => p.PaymentDate >= thisMonthStart && p.PaymentDate <= thisMonthEnd)
            .Sum(p => p.Amount);

        var lastMonthStart = thisMonthStart.AddMonths(-1);
        var lastMonthEnd = thisMonthStart.AddDays(-1);
        var lastMonth = instructorPayments
            .Where(p => p.PaymentDate >= lastMonthStart && p.PaymentDate <= lastMonthEnd)
            .Sum(p => p.Amount);

        var growthRate = lastMonth > 0 ? (double)((thisMonth - lastMonth) / lastMonth) * 100 : 0;

        // Course breakdown
        var courseBreakdown = instructorPayments
            .GroupBy(p => p.CourseId)
            .Select(g => new CourseRevenueBreakdownResponse(
                g.Key,
                instructorCourses.First(c => c.Id == g.Key).Title,
                g.Sum(p => p.Amount),
                $"{g.Sum(p => p.Amount):C} {currency}",
                g.Count()
            ))
            .OrderByDescending(c => c.Revenue)
            .ToList();

        return new InstructorRevenueResponse(
            instructor.Id,
            instructor.GetFullName(),
            totalRevenue,
            $"{totalRevenue:C} {currency}",
            thisMonth,
            $"{thisMonth:C} {currency}",
            lastMonth,
            $"{lastMonth:C} {currency}",
            Math.Round(growthRate, 2),
            instructorPayments.Count,
            courseBreakdown
        );
    }
}