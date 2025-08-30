using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Progress;

public record GetCourseProgressStatsQuery(
    int CourseId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IQuery<CourseProgressStatsResponse?>;

public class GetCourseProgressStatsValidator : AbstractValidator<GetCourseProgressStatsQuery>
{
    public GetCourseProgressStatsValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date");
    }
}

public class GetCourseProgressStatsHandler : IRequestHandler<GetCourseProgressStatsQuery, CourseProgressStatsResponse?>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;

    public GetCourseProgressStatsHandler(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
    }

    public async Task<CourseProgressStatsResponse?> Handle(GetCourseProgressStatsQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            return null;

        // Get all enrollments for the course
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var enrollmentsList = enrollments.ToList();

        // Apply date filters if provided
        if (request.StartDate.HasValue)
        {
            enrollmentsList = enrollmentsList.Where(e => e.EnrollmentDate >= request.StartDate.Value).ToList();
        }

        if (request.EndDate.HasValue)
        {
            enrollmentsList = enrollmentsList.Where(e => e.EnrollmentDate <= request.EndDate.Value).ToList();
        }

        if (!enrollmentsList.Any())
        {
            return new CourseProgressStatsResponse(
                course.Id,
                course.Title,
                0,
                0,
                0,
                0,
                0,
                0,
                "0m",
                DateTime.UtcNow
            );
        }

        // Calculate statistics
        var totalEnrollments = enrollmentsList.Count;
        var completedEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Completed);
        var activeEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Active);

        // Calculate average progress
        var averageProgress = enrollmentsList.Average(e => e.Progress);

        // Calculate completion rate
        var completionRate = (decimal)completedEnrollments / totalEnrollments * 100;

        // Calculate average time to complete (for completed enrollments only)
        var completedEnrollmentsWithDates = enrollmentsList
            .Where(e => e.Status == EnrollmentStatus.Completed && e.CompletionDate.HasValue)
            .ToList();

        var averageTimeToCompleteMinutes = 0;
        if (completedEnrollmentsWithDates.Any())
        {
            var totalMinutes = completedEnrollmentsWithDates
                .Sum(e => (e.CompletionDate!.Value - e.EnrollmentDate).TotalMinutes);
            averageTimeToCompleteMinutes = (int)(totalMinutes / completedEnrollmentsWithDates.Count);
        }

        // Format average time to complete
        var hours = averageTimeToCompleteMinutes / 60;
        var minutes = averageTimeToCompleteMinutes % 60;
        var formattedAverageTime = hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";

        return new CourseProgressStatsResponse(
            course.Id,
            course.Title,
            Math.Round(averageProgress, 2),
            Math.Round(completionRate, 2),
            totalEnrollments,
            completedEnrollments,
            activeEnrollments,
            averageTimeToCompleteMinutes,
            formattedAverageTime,
            DateTime.UtcNow
        );
    }
}