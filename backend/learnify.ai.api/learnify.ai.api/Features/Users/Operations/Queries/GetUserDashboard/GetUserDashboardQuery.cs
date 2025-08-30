using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Assessments;
using learnify.ai.api.Features.Payments;
using learnify.ai.api.Features.Reviews;

namespace learnify.ai.api.Features.Users;

public record GetUserDashboardQuery(int UserId) : IQuery<UserDashboardResponse?>;

public class GetUserDashboardValidator : AbstractValidator<GetUserDashboardQuery>
{
    public GetUserDashboardValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

public class GetUserDashboardHandler : IRequestHandler<GetUserDashboardQuery, UserDashboardResponse?>
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IReviewRepository _reviewRepository;

    public GetUserDashboardHandler(
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        IPaymentRepository paymentRepository,
        IReviewRepository reviewRepository)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _paymentRepository = paymentRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<UserDashboardResponse?> Handle(GetUserDashboardQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return null;

        // Get basic dashboard stats
        var stats = await GetUserDashboardStatsAsync(request.UserId, cancellationToken);
        
        // Get recent activity (simplified)
        var recentActivity = await GetRecentActivityAsync(request.UserId, cancellationToken);
        
        // Get active enrollments (simplified)
        var activeEnrollments = await GetActiveEnrollmentsAsync(request.UserId, cancellationToken);
        
        // Get quiz performance
        var quizPerformance = await GetQuizPerformanceAsync(request.UserId, cancellationToken);

        // Get instructor data if user is an instructor
        InstructorDashboardData? instructorData = null;
        if (user.Role == UserRole.Instructor || user.Role == UserRole.Admin)
        {
            instructorData = await GetInstructorDashboardDataAsync(request.UserId, cancellationToken);
        }

        return new UserDashboardResponse(
            user.Id,
            user.Role,
            stats,
            recentActivity,
            activeEnrollments,
            quizPerformance,
            instructorData
        );
    }

    private async Task<UserDashboardStats> GetUserDashboardStatsAsync(int userId, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId, cancellationToken);
        var enrollmentsList = enrollments.ToList();
        
        var totalEnrollments = enrollmentsList.Count;
        var activeEnrollments = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Active);
        var completedCourses = enrollmentsList.Count(e => e.Status == EnrollmentStatus.Completed);

        // Calculate overall progress
        var overallProgress = enrollmentsList.Any() 
            ? enrollmentsList.Average(e => e.Progress) 
            : 0;

        // Simplified time calculation (use enrollment data)
        var totalTimeSpent = enrollmentsList.Sum(e => (int)(e.Progress * 10)); // Rough estimate

        // Count certificates (completed enrollments)
        var certificatesEarned = completedCourses;

        // Get quiz statistics
        var quizAttempts = await _quizAttemptRepository.GetByUserIdAsync(userId, cancellationToken);
        var completedAttempts = quizAttempts.Where(qa => qa.CompletedAt.HasValue).ToList();
        var quizzesTaken = completedAttempts.Count;
        var quizzesPassed = completedAttempts.Count(qa => qa.IsPassed);

        return new UserDashboardStats(
            totalEnrollments,
            activeEnrollments,
            completedCourses,
            overallProgress,
            totalTimeSpent,
            certificatesEarned,
            quizzesTaken,
            quizzesPassed
        );
    }

    private async Task<IEnumerable<RecentActivityItem>> GetRecentActivityAsync(int userId, CancellationToken cancellationToken)
    {
        var activities = new List<RecentActivityItem>();
        var cutoffDate = DateTime.UtcNow.AddDays(-30); // Last 30 days

        try
        {
            // Recent enrollments
            var recentEnrollments = await _enrollmentRepository.GetByUserIdAsync(userId, cancellationToken);
            var recentEnrollmentsList = recentEnrollments
                .Where(e => e.EnrollmentDate >= cutoffDate)
                .OrderByDescending(e => e.EnrollmentDate)
                .Take(5);

            foreach (var enrollment in recentEnrollmentsList)
            {
                var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
                activities.Add(new RecentActivityItem(
                    "enrollment",
                    "Course Enrollment",
                    $"Enrolled in {course?.Title ?? "Unknown Course"}",
                    enrollment.EnrollmentDate,
                    enrollment.CourseId
                ));
            }

            // Recent quiz completions
            var recentQuizAttempts = await _quizAttemptRepository.GetByUserIdAsync(userId, cancellationToken);
            var recentQuizCompletions = recentQuizAttempts
                .Where(qa => qa.CompletedAt.HasValue && qa.CompletedAt >= cutoffDate)
                .OrderByDescending(qa => qa.CompletedAt)
                .Take(5);

            foreach (var attempt in recentQuizCompletions)
            {
                var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, cancellationToken);
                
                activities.Add(new RecentActivityItem(
                    "quiz_completed",
                    "Quiz Completed",
                    $"Completed quiz: {quiz?.Title ?? "Unknown Quiz"} - {(attempt.IsPassed ? "Passed" : "Failed")}",
                    attempt.CompletedAt!.Value,
                    quiz?.CourseId,
                    null,
                    attempt.QuizId
                ));
            }

            // Recent course completions
            var recentCompletions = recentEnrollments
                .Where(e => e.Status == EnrollmentStatus.Completed && 
                           e.CompletionDate.HasValue && 
                           e.CompletionDate >= cutoffDate)
                .OrderByDescending(e => e.CompletionDate)
                .Take(3);

            foreach (var completion in recentCompletions)
            {
                var course = await _courseRepository.GetByIdAsync(completion.CourseId, cancellationToken);
                activities.Add(new RecentActivityItem(
                    "course_completed",
                    "Course Completed",
                    $"Completed {course?.Title ?? "Unknown Course"}",
                    completion.CompletionDate!.Value,
                    completion.CourseId
                ));
            }
        }
        catch (Exception)
        {
            // Return empty list if there are any issues
            // TODO: Add proper logging
        }

        return activities.OrderByDescending(a => a.ActivityDate).Take(10);
    }

    private async Task<IEnumerable<ActiveEnrollmentSummary>> GetActiveEnrollmentsAsync(int userId, CancellationToken cancellationToken)
    {
        var summaries = new List<ActiveEnrollmentSummary>();

        try
        {
            var allEnrollments = await _enrollmentRepository.GetByUserIdAsync(userId, cancellationToken);
            var activeEnrollments = allEnrollments
                .Where(e => e.Status == EnrollmentStatus.Active)
                .OrderByDescending(e => e.EnrollmentDate)
                .Take(5);

            foreach (var enrollment in activeEnrollments)
            {
                var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
                var instructor = course != null ? await _userRepository.GetByIdAsync(course.InstructorId, cancellationToken) : null;
                
                // Simplified lesson count estimation
                var estimatedTotalLessons = Math.Max(1, (int)(course?.DurationHours ?? 1));

                var estimatedCompletedLessons = (int)(enrollment.Progress / 100m * estimatedTotalLessons);

                summaries.Add(new ActiveEnrollmentSummary(
                    enrollment.Id,
                    enrollment.CourseId,
                    course?.Title ?? "Unknown Course",
                    course?.ThumbnailUrl ?? "",
                    enrollment.Progress,
                    enrollment.EnrollmentDate,
                    estimatedCompletedLessons,
                    estimatedTotalLessons,
                    instructor?.GetFullName() ?? "Unknown Instructor",
                    enrollment.EnrollmentDate
                ));
            }
        }
        catch (Exception)
        {
            // Return empty list if there are any issues
            // TODO: Add proper logging
        }

        return summaries;
    }

    private async Task<QuizPerformanceSummary> GetQuizPerformanceAsync(int userId, CancellationToken cancellationToken)
    {
        try
        {
            var allAttempts = await _quizAttemptRepository.GetByUserIdAsync(userId, cancellationToken);
            var completedAttempts = allAttempts.Where(qa => qa.CompletedAt.HasValue).ToList();
            
            var totalQuizzesTaken = completedAttempts.Count;
            var quizzesPassed = completedAttempts.Count(qa => qa.IsPassed);
            var quizzesFailed = totalQuizzesTaken - quizzesPassed;
            var passRate = totalQuizzesTaken > 0 ? (decimal)quizzesPassed / totalQuizzesTaken * 100 : 0;
            var averageScore = completedAttempts.Any() ? completedAttempts.Average(qa => qa.GetScorePercentage()) : 0;
            var bestScore = completedAttempts.Any() ? completedAttempts.Max(qa => qa.GetScorePercentage()) : 0;

            // Recent quizzes (last 30 days)
            var cutoffDate = DateTime.UtcNow.AddDays(-30);
            var recentAttempts = completedAttempts
                .Where(qa => qa.CompletedAt >= cutoffDate)
                .OrderByDescending(qa => qa.CompletedAt)
                .Take(5);

            var recentQuizAttempts = new List<RecentQuizAttempt>();
            foreach (var attempt in recentAttempts)
            {
                var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, cancellationToken);
                var course = quiz != null ? await _courseRepository.GetByIdAsync(quiz.CourseId, cancellationToken) : null;

                recentQuizAttempts.Add(new RecentQuizAttempt(
                    attempt.QuizId,
                    quiz?.Title ?? "Unknown Quiz",
                    course?.Title ?? "Unknown Course",
                    attempt.Score,
                    attempt.MaxScore,
                    attempt.GetScorePercentage(),
                    attempt.IsPassed,
                    attempt.CompletedAt!.Value
                ));
            }

            return new QuizPerformanceSummary(
                totalQuizzesTaken,
                quizzesPassed,
                quizzesFailed,
                passRate,
                averageScore,
                bestScore,
                recentAttempts.Count(),
                recentQuizAttempts
            );
        }
        catch (Exception)
        {
            // Return empty performance if there are any issues
            return new QuizPerformanceSummary(0, 0, 0, 0, 0, 0, 0, new List<RecentQuizAttempt>());
        }
    }

    private async Task<InstructorDashboardData> GetInstructorDashboardDataAsync(int instructorId, CancellationToken cancellationToken)
    {
        try
        {
            var instructorCourses = await _courseRepository.GetByInstructorIdAsync(instructorId, cancellationToken);
            var coursesList = instructorCourses.ToList();
            
            var totalCourses = coursesList.Count;
            var publishedCourses = coursesList.Count(c => c.IsPublished);
            var draftCourses = totalCourses - publishedCourses;

            // Calculate total students (simplified - use enrollment count)
            var totalStudents = 0;
            foreach (var course in coursesList)
            {
                var studentCount = await _enrollmentRepository.GetEnrollmentCountByCourseAsync(course.Id, cancellationToken);
                totalStudents += studentCount;
            }

            // Simplified revenue calculation (placeholder)
            var totalRevenue = 0m;
            var monthlyRevenue = 0m;

            // Calculate average rating
            var allRatings = new List<double>();
            var totalReviews = 0;
            foreach (var course in coursesList)
            {
                var avgRating = await _reviewRepository.GetAverageRatingAsync(course.Id, cancellationToken);
                if (avgRating > 0) allRatings.Add(avgRating);
                
                var reviewCount = await _reviewRepository.GetReviewCountAsync(course.Id, cancellationToken);
                totalReviews += reviewCount;
            }
            var averageRating = allRatings.Any() ? allRatings.Average() : 0;

            // Get top performing courses (simplified)
            var topCourses = new List<TopPerformingCourse>();
            foreach (var course in coursesList.Take(5))
            {
                var studentCount = await _enrollmentRepository.GetEnrollmentCountByCourseAsync(course.Id, cancellationToken);
                var revenue = 0m; // Placeholder
                var rating = await _reviewRepository.GetAverageRatingAsync(course.Id, cancellationToken);
                var reviewCount = await _reviewRepository.GetReviewCountAsync(course.Id, cancellationToken);
                var completionRate = 75.0m; // Placeholder

                topCourses.Add(new TopPerformingCourse(
                    course.Id,
                    course.Title,
                    studentCount,
                    revenue,
                    rating,
                    reviewCount,
                    completionRate
                ));
            }

            // Recent analytics (placeholder)
            var recentAnalytics = new CourseAnalyticsSummary(
                0, // NewEnrollmentsThisMonth
                0, // CompletionsThisMonth
                monthlyRevenue,
                0, // RevenueGrowth
                totalStudents
            );

            return new InstructorDashboardData(
                totalCourses,
                publishedCourses,
                draftCourses,
                totalStudents,
                totalRevenue,
                monthlyRevenue,
                averageRating,
                totalReviews,
                topCourses.OrderByDescending(c => c.StudentCount),
                recentAnalytics
            );
        }
        catch (Exception)
        {
            // Return minimal instructor data if there are issues
            return new InstructorDashboardData(
                0, 0, 0, 0, 0m, 0m, 0, 0,
                new List<TopPerformingCourse>(),
                new CourseAnalyticsSummary(0, 0, 0m, 0, 0)
            );
        }
    }
}