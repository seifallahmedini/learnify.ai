using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Progress;

public record GetUserLearningStatsQuery(
    int UserId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IQuery<UserLearningStatsResponse?>;

public class GetUserLearningStatsValidator : AbstractValidator<GetUserLearningStatsQuery>
{
    public GetUserLearningStatsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date");
    }
}

public class GetUserLearningStatsHandler : IRequestHandler<GetUserLearningStatsQuery, UserLearningStatsResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly ICourseRepository _courseRepository;

    public GetUserLearningStatsHandler(
        IUserRepository userRepository,
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        ICourseRepository courseRepository)
    {
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _courseRepository = courseRepository;
    }

    public async Task<UserLearningStatsResponse?> Handle(GetUserLearningStatsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;

        // Get all user enrollments
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
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

        // Calculate overall statistics
        var totalEnrollments = enrollmentsList.Count;
        var activeEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Active);
        var completedCourses = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Completed);

        // Calculate total time spent and completed lessons across all enrollments
        int totalTimeSpent = 0;
        int totalCompletedLessons = 0;
        DateTime? lastLearningActivity = null;

        var courseStats = new List<CourseLearningStatsResponse>();

        foreach (var enrollment in enrollmentsList)
        {
            var enrollmentTimeSpent = await _progressRepository.GetTotalTimeSpentAsync(enrollment.Id, cancellationToken);
            var enrollmentCompletedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);
            
            totalTimeSpent += enrollmentTimeSpent;
            totalCompletedLessons += enrollmentCompletedLessons;

            // Get course information
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
            var totalLessons = course != null ? await _progressRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken) : new List<Enrollments.Progress>();
            var totalLessonsCount = totalLessons.Count();

            // Get last access date for this enrollment
            var enrollmentProgress = await _progressRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
            var enrollmentLastAccess = enrollmentProgress.Any() 
                ? enrollmentProgress.Max(p => p.LastAccessDate) 
                : enrollment.UpdatedAt;

            if (lastLearningActivity == null || enrollmentLastAccess > lastLearningActivity)
            {
                lastLearningActivity = enrollmentLastAccess;
            }

            // Format time spent for this course
            var hours = enrollmentTimeSpent / 60;
            var minutes = enrollmentTimeSpent % 60;
            var formattedTime = hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";

            courseStats.Add(new CourseLearningStatsResponse(
                enrollment.CourseId,
                course?.Title ?? "Unknown Course",
                enrollment.Progress,
                enrollmentCompletedLessons,
                totalLessonsCount,
                enrollmentTimeSpent,
                formattedTime,
                enrollment.Status,
                enrollmentLastAccess
            ));
        }

        // Calculate average progress
        var averageProgress = totalEnrollments > 0 
            ? enrollmentsList.Average(e => e.Progress) 
            : 0;

        // Format total time spent
        var totalHours = totalTimeSpent / 60;
        var totalMinutes = totalTimeSpent % 60;
        var formattedTotalTime = totalHours > 0 ? $"{totalHours}h {totalMinutes}m" : $"{totalMinutes}m";

        return new UserLearningStatsResponse(
            user.Id,
            user.GetFullName(),
            totalTimeSpent,
            formattedTotalTime,
            totalCompletedLessons,
            completedCourses,
            activeEnrollments,
            Math.Round(averageProgress, 2),
            totalEnrollments,
            lastLearningActivity,
            courseStats
        );
    }
}