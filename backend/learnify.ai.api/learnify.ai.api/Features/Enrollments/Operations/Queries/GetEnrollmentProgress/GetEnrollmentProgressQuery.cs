using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Enrollments;

public record GetEnrollmentProgressQuery(
    int EnrollmentId
) : IQuery<EnrollmentProgressResponse?>;

public class GetEnrollmentProgressValidator : AbstractValidator<GetEnrollmentProgressQuery>
{
    public GetEnrollmentProgressValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");
    }
}

public class GetEnrollmentProgressHandler : IRequestHandler<GetEnrollmentProgressQuery, EnrollmentProgressResponse?>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public GetEnrollmentProgressHandler(
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<EnrollmentProgressResponse?> Handle(GetEnrollmentProgressQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            return null;

        var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
        var allProgress = await _progressRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
        var totalTimeSpent = await _progressRepository.GetTotalTimeSpentAsync(enrollment.Id, cancellationToken);
        var completedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);
        var totalLessons = await _lessonRepository.GetLessonCountAsync(enrollment.CourseId, cancellationToken);

        // Build lesson progress list
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

        // Format total time spent
        var hours = totalTimeSpent / 60;
        var minutes = totalTimeSpent % 60;
        var formattedTime = hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";

        // Get last access date
        var lastAccessDate = allProgress.Any() 
            ? allProgress.Max(p => p.LastAccessDate) 
            : enrollment.UpdatedAt;

        return new EnrollmentProgressResponse(
            enrollment.Id,
            enrollment.UserId,
            enrollment.CourseId,
            course?.Title ?? "Unknown Course",
            enrollment.Progress,
            completedLessons,
            totalLessons,
            totalTimeSpent,
            formattedTime,
            lessonProgressList,
            lastAccessDate
        );
    }
}