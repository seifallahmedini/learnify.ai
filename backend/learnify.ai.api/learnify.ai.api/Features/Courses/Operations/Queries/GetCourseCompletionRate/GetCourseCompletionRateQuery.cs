using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses.Infrastructure.Data;
using learnify.ai.api.Features.Enrollments.Infrastructure.Data;

namespace learnify.ai.api.Features.Courses.Operations.Queries.GetCourseCompletionRate;

public record GetCourseCompletionRateQuery(int CourseId) : IQuery<CourseCompletionRateResponse>;

public record CourseCompletionRateResponse(
    int CourseId,
    string CourseTitle,
    decimal CompletionRate,
    int TotalEnrollments,
    int CompletedEnrollments,
    int ActiveEnrollments,
    int DroppedEnrollments,
    decimal AverageProgress,
    DateTime CalculatedAt
);

public class GetCourseCompletionRateValidator : AbstractValidator<GetCourseCompletionRateQuery>
{
    public GetCourseCompletionRateValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class GetCourseCompletionRateHandler : IRequestHandler<GetCourseCompletionRateQuery, CourseCompletionRateResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetCourseCompletionRateHandler(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<CourseCompletionRateResponse> Handle(GetCourseCompletionRateQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get all enrollments for the course
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var enrollmentsList = enrollments.ToList();

        var totalEnrollments = enrollmentsList.Count;
        
        if (totalEnrollments == 0)
        {
            return new CourseCompletionRateResponse(
                course.Id,
                course.Title,
                0,
                0,
                0,
                0,
                0,
                0,
                DateTime.UtcNow
            );
        }

        var completedEnrollments = enrollmentsList.Count(e => e.Status == Enrollments.Core.Models.EnrollmentStatus.Completed);
        var activeEnrollments = enrollmentsList.Count(e => e.Status == Enrollments.Core.Models.EnrollmentStatus.Active);
        var droppedEnrollments = enrollmentsList.Count(e => e.Status == Enrollments.Core.Models.EnrollmentStatus.Dropped);

        var completionRate = (decimal)completedEnrollments / totalEnrollments * 100;
        var averageProgress = enrollmentsList.Average(e => e.Progress);

        return new CourseCompletionRateResponse(
            course.Id,
            course.Title,
            Math.Round(completionRate, 2),
            totalEnrollments,
            completedEnrollments,
            activeEnrollments,
            droppedEnrollments,
            Math.Round(averageProgress, 2),
            DateTime.UtcNow
        );
    }
}