using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Progress;

public record GetEnrollmentProgressQuery(
    int EnrollmentId
) : IQuery<EnrollmentProgressDetailResponse?>;

public class GetEnrollmentProgressValidator : AbstractValidator<GetEnrollmentProgressQuery>
{
    public GetEnrollmentProgressValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");
    }
}

public class GetEnrollmentProgressHandler : IRequestHandler<GetEnrollmentProgressQuery, EnrollmentProgressDetailResponse?>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public GetEnrollmentProgressHandler(
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<EnrollmentProgressDetailResponse?> Handle(GetEnrollmentProgressQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            return null;

        // Get related data
        var user = await _userRepository.GetByIdAsync(enrollment.UserId, cancellationToken);
        var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);

        if (user == null || course == null)
            throw new InvalidOperationException("Associated user or course not found");

        // Get all lessons for the course
        var courseLessons = await _lessonRepository.GetByCourseIdAsync(enrollment.CourseId, cancellationToken);
        var lessonsOrdered = courseLessons.OrderBy(l => l.OrderIndex).ToList();

        // Get all progress for this enrollment
        var allProgress = await _progressRepository.GetByEnrollmentIdAsync(request.EnrollmentId, cancellationToken);
        var progressDict = allProgress.ToDictionary(p => p.LessonId);

        // Build lesson progress responses
        var lessonProgressList = new List<DetailedLessonProgressResponse>();
        int totalTimeSpent = 0;
        int completedLessons = 0;

        foreach (var lesson in lessonsOrdered)
        {
            if (progressDict.TryGetValue(lesson.Id, out var progress))
            {
                lessonProgressList.Add(new DetailedLessonProgressResponse(
                    lesson.Id,
                    lesson.Title,
                    request.EnrollmentId,
                    progress.IsCompleted,
                    progress.CompletionDate,
                    progress.TimeSpent,
                    progress.GetFormattedTimeSpent(),
                    progress.LastAccessDate
                ));

                totalTimeSpent += progress.TimeSpent;
                if (progress.IsCompleted)
                    completedLessons++;
            }
            else
            {
                // No progress for this lesson yet
                lessonProgressList.Add(new DetailedLessonProgressResponse(
                    lesson.Id,
                    lesson.Title,
                    request.EnrollmentId,
                    false,
                    null,
                    0,
                    "0m",
                    DateTime.UtcNow
                ));
            }
        }

        // Calculate overall progress
        var overallProgress = lessonsOrdered.Count > 0 
            ? (decimal)completedLessons / lessonsOrdered.Count * 100 
            : 0;

        // Format total time spent
        var hours = totalTimeSpent / 60;
        var minutes = totalTimeSpent % 60;
        var formattedTotalTime = hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";

        // Get last access date
        var lastAccessDate = allProgress.Any() 
            ? allProgress.Max(p => p.LastAccessDate)
            : enrollment.CreatedAt;

        return new EnrollmentProgressDetailResponse(
            enrollment.Id,
            enrollment.UserId,
            enrollment.CourseId,
            course.Title,
            lessonProgressList,
            Math.Round(overallProgress, 2),
            completedLessons,
            lessonsOrdered.Count,
            totalTimeSpent,
            formattedTotalTime,
            lastAccessDate
        );
    }
}