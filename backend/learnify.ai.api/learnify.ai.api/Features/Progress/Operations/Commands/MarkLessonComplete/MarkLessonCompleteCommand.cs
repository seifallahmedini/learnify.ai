using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Progress;

public record MarkLessonCompleteCommand(
    int LessonId,
    int EnrollmentId
) : ICommand<DetailedLessonProgressResponse?>;

public class MarkLessonCompleteValidator : AbstractValidator<MarkLessonCompleteCommand>
{
    public MarkLessonCompleteValidator()
    {
        RuleFor(x => x.LessonId)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");

        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");
    }
}

public class MarkLessonCompleteHandler : IRequestHandler<MarkLessonCompleteCommand, DetailedLessonProgressResponse?>
{
    private readonly IProgressRepository _progressRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonRepository _lessonRepository;

    public MarkLessonCompleteHandler(
        IProgressRepository progressRepository,
        IEnrollmentRepository enrollmentRepository,
        ILessonRepository lessonRepository)
    {
        _progressRepository = progressRepository;
        _enrollmentRepository = enrollmentRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<DetailedLessonProgressResponse?> Handle(MarkLessonCompleteCommand request, CancellationToken cancellationToken)
    {
        // Verify enrollment exists
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            throw new ArgumentException($"Enrollment with ID {request.EnrollmentId} not found");

        // Verify lesson exists and belongs to the enrolled course
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);
        if (lesson == null)
            throw new ArgumentException($"Lesson with ID {request.LessonId} not found");

        if (lesson.CourseId != enrollment.CourseId)
            throw new ArgumentException("Lesson does not belong to the enrolled course");

        // Get or create progress record
        var progress = await _progressRepository.GetByEnrollmentAndLessonAsync(request.EnrollmentId, request.LessonId, cancellationToken);
        
        if (progress == null)
        {
            progress = new Enrollments.Progress
            {
                EnrollmentId = request.EnrollmentId,
                LessonId = request.LessonId,
                IsCompleted = false,
                TimeSpent = 0,
                LastAccessDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            progress = await _progressRepository.CreateAsync(progress, cancellationToken);
        }

        // Mark as completed if not already completed
        if (!progress.IsCompleted)
        {
            progress.MarkAsCompleted();
            await _progressRepository.UpdateAsync(progress, cancellationToken);

            // Update overall enrollment progress
            await UpdateEnrollmentProgress(enrollment, cancellationToken);
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

    private async Task UpdateEnrollmentProgress(Enrollments.Enrollment enrollment, CancellationToken cancellationToken)
    {
        var totalLessons = await _lessonRepository.GetLessonCountAsync(enrollment.CourseId, cancellationToken);
        if (totalLessons == 0) return;

        var completedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);
        var progressPercentage = (decimal)completedLessons / totalLessons * 100;

        enrollment.Progress = Math.Round(progressPercentage, 2);

        // Mark as completed if all lessons are done
        if (progressPercentage >= 100 && enrollment.Status == Enrollments.EnrollmentStatus.Active)
        {
            enrollment.Status = Enrollments.EnrollmentStatus.Completed;
            enrollment.CompletionDate = DateTime.UtcNow;
        }

        enrollment.UpdatedAt = DateTime.UtcNow;
        await _enrollmentRepository.UpdateAsync(enrollment, cancellationToken);
    }
}