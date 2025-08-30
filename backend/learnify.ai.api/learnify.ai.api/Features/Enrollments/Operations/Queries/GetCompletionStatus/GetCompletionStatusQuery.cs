using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Enrollments;

public record GetCompletionStatusQuery(
    int EnrollmentId
) : IQuery<CompletionStatusResponse?>;

public class GetCompletionStatusValidator : AbstractValidator<GetCompletionStatusQuery>
{
    public GetCompletionStatusValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");
    }
}

public class GetCompletionStatusHandler : IRequestHandler<GetCompletionStatusQuery, CompletionStatusResponse?>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly ILessonRepository _lessonRepository;

    public GetCompletionStatusHandler(
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        ILessonRepository lessonRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<CompletionStatusResponse?> Handle(GetCompletionStatusQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            return null;

        var completedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);
        var totalLessons = await _lessonRepository.GetTotalLessonCountByCourseAsync(enrollment.CourseId, cancellationToken);

        // Check if eligible for certificate (100% completion)
        var isEligibleForCertificate = enrollment.IsCompleted() && enrollment.Progress >= 100;
        
        // Generate certificate URL if eligible (placeholder)
        string? certificateUrl = null;
        if (isEligibleForCertificate)
        {
            certificateUrl = $"/certificates/CERT-{enrollment.Id}-{DateTime.UtcNow:yyyyMMdd}.pdf";
        }

        return new CompletionStatusResponse(
            enrollment.Id,
            enrollment.IsCompleted(),
            enrollment.CompletionDate,
            enrollment.Progress,
            completedLessons,
            totalLessons,
            isEligibleForCertificate,
            certificateUrl
        );
    }
}