using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
namespace learnify.ai.api.Features.Enrollments;

public record GetEnrollmentStatisticsQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? UserId = null,
    int? CourseId = null
) : IQuery<EnrollmentStatisticsResponse>;

public class GetEnrollmentStatisticsValidator : AbstractValidator<GetEnrollmentStatisticsQuery>
{
    public GetEnrollmentStatisticsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .When(x => x.UserId.HasValue)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .When(x => x.CourseId.HasValue)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date");
    }
}

public class GetEnrollmentStatisticsHandler : IRequestHandler<GetEnrollmentStatisticsQuery, EnrollmentStatisticsResponse>
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetEnrollmentStatisticsHandler(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<EnrollmentStatisticsResponse> Handle(GetEnrollmentStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Get all enrollments based on filters
        IEnumerable<Enrollment> enrollments;

        if (request.UserId.HasValue)
        {
            enrollments = await _enrollmentRepository.GetByUserIdAsync(request.UserId.Value, cancellationToken);
        }
        else if (request.CourseId.HasValue)
        {
            enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId.Value, cancellationToken);
        }
        else
        {
            enrollments = await _enrollmentRepository.GetAllAsync(cancellationToken);
        }

        // Apply date filters
        if (request.StartDate.HasValue)
        {
            enrollments = enrollments.Where(e => e.EnrollmentDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            enrollments = enrollments.Where(e => e.EnrollmentDate <= request.EndDate.Value);
        }

        var enrollmentsList = enrollments.ToList();

        // Calculate statistics
        var totalEnrollments = enrollmentsList.Count;
        var activeEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Active);
        var completedEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Completed);
        var droppedEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Dropped);
        var suspendedEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Suspended);

        // Calculate rates
        var completionRate = totalEnrollments > 0 ? (decimal)completedEnrollments / totalEnrollments * 100 : 0;
        var dropoutRate = totalEnrollments > 0 ? (decimal)droppedEnrollments / totalEnrollments * 100 : 0;

        // Calculate average time to completion (in days)
        var completedEnrollmentsWithDates = enrollmentsList
            .Where(e => e.Status == EnrollmentStatus.Completed && e.CompletionDate.HasValue)
            .ToList();

        var averageTimeToCompletion = 0;
        if (completedEnrollmentsWithDates.Any())
        {
            var totalDays = completedEnrollmentsWithDates
                .Sum(e => (e.CompletionDate!.Value - e.EnrollmentDate).TotalDays);
            averageTimeToCompletion = (int)(totalDays / completedEnrollmentsWithDates.Count);
        }

        return new EnrollmentStatisticsResponse(
            totalEnrollments,
            activeEnrollments,
            completedEnrollments,
            droppedEnrollments,
            suspendedEnrollments,
            Math.Round(completionRate, 2),
            Math.Round(dropoutRate, 2),
            averageTimeToCompletion,
            DateTime.UtcNow
        );
    }
}