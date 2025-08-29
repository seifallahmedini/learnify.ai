using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses.Infrastructure.Data;
using learnify.ai.api.Features.Enrollments.Infrastructure.Data;
using learnify.ai.api.Features.Reviews.Infrastructure.Data;
using learnify.ai.api.Features.Payments.Infrastructure.Data;

namespace learnify.ai.api.Features.Courses.Operations.Queries.GetCourseAnalytics;

public record GetCourseAnalyticsQuery(int CourseId) : IQuery<CourseAnalyticsResponse>;

public record CourseAnalyticsResponse(
    int CourseId,
    string CourseTitle,
    int TotalEnrollments,
    int ActiveEnrollments,
    int CompletedEnrollments,
    decimal CompletionRate,
    double AverageRating,
    int TotalReviews,
    decimal TotalRevenue,
    decimal AverageScore,
    DateTime LastEnrollmentDate,
    DateTime AnalyticsDate
);

public class GetCourseAnalyticsValidator : AbstractValidator<GetCourseAnalyticsQuery>
{
    public GetCourseAnalyticsValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class GetCourseAnalyticsHandler : IRequestHandler<GetCourseAnalyticsQuery, CourseAnalyticsResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IPaymentRepository _paymentRepository;

    public GetCourseAnalyticsHandler(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IReviewRepository reviewRepository,
        IPaymentRepository paymentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _reviewRepository = reviewRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<CourseAnalyticsResponse> Handle(GetCourseAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get enrollment statistics
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var enrollmentsList = enrollments.ToList();
        
        var totalEnrollments = enrollmentsList.Count;
        var activeEnrollments = enrollmentsList.Count(e => e.Status == Enrollments.Core.Models.EnrollmentStatus.Active);
        var completedEnrollments = enrollmentsList.Count(e => e.Status == Enrollments.Core.Models.EnrollmentStatus.Completed);
        var completionRate = totalEnrollments > 0 ? (decimal)completedEnrollments / totalEnrollments * 100 : 0;

        // Get review statistics
        var reviews = await _reviewRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var reviewsList = reviews.Where(r => r.IsApproved).ToList();
        var averageRating = reviewsList.Any() ? reviewsList.Average(r => r.Rating) : 0;
        var totalReviews = reviewsList.Count;

        // Get revenue statistics
        var payments = await _paymentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var completedPayments = payments.Where(p => p.Status == Payments.Core.Models.PaymentStatus.Completed);
        var totalRevenue = completedPayments.Sum(p => p.Amount);

        // Get last enrollment date
        var lastEnrollmentDate = enrollmentsList.Any() 
            ? enrollmentsList.Max(e => e.EnrollmentDate) 
            : DateTime.MinValue;

        // Calculate average score (this would typically come from quiz attempts)
        var averageScore = 0m; // TODO: Implement when quiz attempts are available

        return new CourseAnalyticsResponse(
            course.Id,
            course.Title,
            totalEnrollments,
            activeEnrollments,
            completedEnrollments,
            completionRate,
            averageRating,
            totalReviews,
            totalRevenue,
            (decimal)averageScore,
            lastEnrollmentDate,
            DateTime.UtcNow
        );
    }
}