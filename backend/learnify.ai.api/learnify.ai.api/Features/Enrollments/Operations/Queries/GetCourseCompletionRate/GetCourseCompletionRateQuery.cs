using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Enrollments;

public record GetCourseCompletionRateQuery(
    int CourseId
) : IQuery<decimal>;

public class GetCourseCompletionRateValidator : AbstractValidator<GetCourseCompletionRateQuery>
{
    public GetCourseCompletionRateValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class GetCourseCompletionRateHandler : IRequestHandler<GetCourseCompletionRateQuery, decimal>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCourseCompletionRateHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<decimal> Handle(GetCourseCompletionRateQuery request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get all enrollments for the course
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var enrollmentsList = enrollments.ToList();

        if (!enrollmentsList.Any())
            return 0;

        var totalEnrollments = enrollmentsList.Count;
        var completedEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Completed);

        var completionRate = (decimal)completedEnrollments / totalEnrollments * 100;
        return Math.Round(completionRate, 2);
    }
}