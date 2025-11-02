using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Progress;

public record TrackTimeSpentCommand(
    int LessonId,
    int EnrollmentId,
    int TimeSpentMinutes
) : ICommand<DetailedLessonProgressResponse?>;

public class TrackTimeSpentValidator : AbstractValidator<TrackTimeSpentCommand>
{
    public TrackTimeSpentValidator()
    {
        RuleFor(x => x.LessonId)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");

        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");

        RuleFor(x => x.TimeSpentMinutes)
            .GreaterThan(0)
            .WithMessage("Time spent must be greater than 0");
    }
}

public class TrackTimeSpentHandler : IRequestHandler<TrackTimeSpentCommand, DetailedLessonProgressResponse?>
{
    private readonly IProgressRepository _progressRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonRepository _lessonRepository;

    public TrackTimeSpentHandler(
        IProgressRepository progressRepository,
        IEnrollmentRepository enrollmentRepository,
        ILessonRepository lessonRepository)
    {
        _progressRepository = progressRepository;
        _enrollmentRepository = enrollmentRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<DetailedLessonProgressResponse?> Handle(TrackTimeSpentCommand request, CancellationToken cancellationToken)
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
            progress = new Domain.Entities.Progress
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

        // Add time spent
        progress.AddTimeSpent(request.TimeSpentMinutes);
        await _progressRepository.UpdateAsync(progress, cancellationToken);

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
