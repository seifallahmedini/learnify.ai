using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Enrollments;

public record UpdateEnrollmentProgressCommand(
    int EnrollmentId,
    int LessonId,
    bool IsCompleted = false,
    int TimeSpentMinutes = 0
) : ICommand<EnrollmentProgressResponse?>;

public class UpdateEnrollmentProgressValidator : AbstractValidator<UpdateEnrollmentProgressCommand>
{
    public UpdateEnrollmentProgressValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");

        RuleFor(x => x.LessonId)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");

        RuleFor(x => x.TimeSpentMinutes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Time spent must be 0 or greater");
    }
}

public class UpdateEnrollmentProgressHandler : IRequestHandler<UpdateEnrollmentProgressCommand, EnrollmentProgressResponse?>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public UpdateEnrollmentProgressHandler(
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<EnrollmentProgressResponse?> Handle(UpdateEnrollmentProgressCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            return null;

        // Verify lesson belongs to the enrolled course
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);
        if (lesson == null || lesson.CourseId != enrollment.CourseId)
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

        // Update progress
        if (request.IsCompleted && !progress.IsCompleted)
        {
            progress.MarkAsCompleted();
        }

        if (request.TimeSpentMinutes > 0)
        {
            progress.AddTimeSpent(request.TimeSpentMinutes);
        }

        progress.LastAccessDate = DateTime.UtcNow;
        progress.UpdatedAt = DateTime.UtcNow;

        await _progressRepository.UpdateAsync(progress, cancellationToken);

        // Recalculate overall enrollment progress
        await UpdateEnrollmentOverallProgress(enrollment, cancellationToken);

        // Return updated progress response
        return await GetEnrollmentProgressResponse(enrollment.Id, cancellationToken);
    }

    private async Task UpdateEnrollmentOverallProgress(Enrollment enrollment, CancellationToken cancellationToken)
    {
        var totalLessons = await _lessonRepository.GetLessonCountAsync(enrollment.CourseId, cancellationToken);
        if (totalLessons == 0) return;

        var completedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);
        var progressPercentage = (decimal)completedLessons / totalLessons * 100;

        enrollment.Progress = Math.Round(progressPercentage, 2);

        // Mark as completed if all lessons are done
        if (progressPercentage >= 100 && enrollment.Status == EnrollmentStatus.Active)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletionDate = DateTime.UtcNow;
        }

        enrollment.UpdatedAt = DateTime.UtcNow;
        await _enrollmentRepository.UpdateAsync(enrollment, cancellationToken);
    }

    private async Task<EnrollmentProgressResponse> GetEnrollmentProgressResponse(int enrollmentId, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId, cancellationToken);
        var course = await _courseRepository.GetByIdAsync(enrollment!.CourseId, cancellationToken);
        var allProgress = await _progressRepository.GetByEnrollmentIdAsync(enrollmentId, cancellationToken);
        var totalTimeSpent = await _progressRepository.GetTotalTimeSpentAsync(enrollmentId, cancellationToken);
        var completedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollmentId, cancellationToken);
        var totalLessons = await _lessonRepository.GetLessonCountAsync(enrollment.CourseId, cancellationToken);

        var lessonProgressList = new List<LessonProgressResponse>();
        foreach (var progress in allProgress)
        {
            var lesson = await _lessonRepository.GetByIdAsync(progress.LessonId, cancellationToken);
            lessonProgressList.Add(new LessonProgressResponse(
                progress.LessonId,
                lesson?.Title ?? "Unknown Lesson",
                progress.IsCompleted,
                progress.CompletionDate,
                progress.TimeSpent,
                progress.GetFormattedTimeSpent(),
                progress.LastAccessDate
            ));
        }

        var hours = totalTimeSpent / 60;
        var minutes = totalTimeSpent % 60;
        var formattedTime = hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";

        return new EnrollmentProgressResponse(
            enrollmentId,
            enrollment.UserId,
            enrollment.CourseId,
            course?.Title ?? "Unknown Course",
            enrollment.Progress,
            completedLessons,
            totalLessons,
            totalTimeSpent,
            formattedTime,
            lessonProgressList,
            allProgress.Any() ? allProgress.Max(p => p.LastAccessDate) : enrollment.UpdatedAt
        );
    }
}