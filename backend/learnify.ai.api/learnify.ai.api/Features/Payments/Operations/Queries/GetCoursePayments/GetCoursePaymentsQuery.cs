using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;

using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Payments;

public record GetCoursePaymentsQuery(
    int CourseId,
    PaymentStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<CoursePaymentsResponse>;

public class GetCoursePaymentsValidator : AbstractValidator<GetCoursePaymentsQuery>
{
    public GetCoursePaymentsValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
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

public class GetCoursePaymentsHandler : IRequestHandler<GetCoursePaymentsQuery, CoursePaymentsResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public GetCoursePaymentsHandler(
        IPaymentRepository paymentRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _paymentRepository = paymentRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<CoursePaymentsResponse> Handle(GetCoursePaymentsQuery request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get course payments
        var payments = await _paymentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var paymentsList = payments.ToList();

        // Apply filters
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
            var user = await _userRepository.GetByIdAsync(payment.UserId, cancellationToken);
            
            paymentSummaries.Add(new PaymentSummaryResponse(
                payment.Id,
                payment.UserId,
                user?.GetFullName() ?? "Unknown User",
                payment.CourseId,
                course.Title,
                payment.Amount,
                payment.GetFormattedAmount(),
                payment.Status,
                payment.PaymentDate
            ));
        }

        // Calculate total revenue for this course
        var totalRevenue = paymentsList.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new CoursePaymentsResponse(
            course.Id,
            course.Title,
            paymentSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            totalRevenue,
            $"{totalRevenue:C} USD"
        );
    }
}
