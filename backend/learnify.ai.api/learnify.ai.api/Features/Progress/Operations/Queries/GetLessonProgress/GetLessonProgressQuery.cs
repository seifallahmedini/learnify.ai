using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Progress;

public record GetLessonProgressQuery(
    int LessonId,
    int? EnrollmentId = null
) : IQuery<DetailedLessonProgressResponse?>;

public class GetLessonProgressValidator : AbstractValidator<GetLessonProgressQuery>
{
    public GetLessonProgressValidator()
    {
        RuleFor(x => x.LessonId)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");

        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .When(x => x.EnrollmentId.HasValue)
            .WithMessage("Enrollment ID must be greater than 0 when provided");
    }
}

public class GetLessonProgressHandler : IRequestHandler<GetLessonProgressQuery, DetailedLessonProgressResponse?>
{
    private readonly IProgressRepository _progressRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public GetLessonProgressHandler(
        IProgressRepository progressRepository,
        ILessonRepository lessonRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _progressRepository = progressRepository;
        _lessonRepository = lessonRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<DetailedLessonProgressResponse?> Handle(GetLessonProgressQuery request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);
        if (lesson == null)
            return null;

        Enrollments.Progress? progress = null;

        if (request.EnrollmentId.HasValue)
        {
            // Get specific enrollment progress
            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId.Value, cancellationToken);
            if (enrollment == null)
                throw new ArgumentException($"Enrollment with ID {request.EnrollmentId} not found");

            if (lesson.CourseId != enrollment.CourseId)
                throw new ArgumentException("Lesson does not belong to the enrolled course");

            progress = await _progressRepository.GetByEnrollmentAndLessonAsync(request.EnrollmentId.Value, request.LessonId, cancellationToken);
        }
        else
        {
            // Get first available progress for this lesson (if any)
            var progressList = await _progressRepository.GetByLessonIdAsync(request.LessonId, cancellationToken);
            progress = progressList.FirstOrDefault();
        }

        // If no progress found, create a default response
        if (progress == null)
        {
            return new DetailedLessonProgressResponse(
                lesson.Id,
                lesson.Title,
                request.EnrollmentId,
                false,
                null,
                0,
                "0m",
                DateTime.UtcNow
            );
        }

        return new DetailedLessonProgressResponse(
            progress.LessonId,
            lesson.Title,
            progress.EnrollmentId,
            progress.IsCompleted,
            progress.CompletionDate,
            progress.TimeSpent,
            progress.GetFormattedTimeSpent(),
            progress.LastAccessDate
        );
    }
}