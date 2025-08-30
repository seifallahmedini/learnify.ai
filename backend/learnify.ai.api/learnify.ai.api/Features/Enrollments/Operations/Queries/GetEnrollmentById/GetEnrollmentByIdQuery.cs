using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Enrollments;

public record GetEnrollmentByIdQuery(
    int EnrollmentId
) : IQuery<EnrollmentDetailsResponse?>;

public class GetEnrollmentByIdValidator : AbstractValidator<GetEnrollmentByIdQuery>
{
    public GetEnrollmentByIdValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");
    }
}

public class GetEnrollmentByIdHandler : IRequestHandler<GetEnrollmentByIdQuery, EnrollmentDetailsResponse?>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IProgressRepository _progressRepository;
    private readonly ILessonRepository _lessonRepository;

    public GetEnrollmentByIdHandler(
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        IProgressRepository progressRepository,
        ILessonRepository lessonRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _progressRepository = progressRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<EnrollmentDetailsResponse?> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            return null;

        // Load related data
        var user = await _userRepository.GetByIdAsync(enrollment.UserId, cancellationToken);
        var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
        
        // Get progress data
        var completedLessons = await _progressRepository.GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);
        var totalLessons = await _lessonRepository.GetLessonCountAsync(enrollment.CourseId, cancellationToken);
        var totalTimeSpent = await _progressRepository.GetTotalTimeSpentAsync(enrollment.Id, cancellationToken);

        // Payment info (would be loaded from payment repository if needed)
        decimal? paymentAmount = null;
        if (enrollment.PaymentId.HasValue && course != null)
        {
            paymentAmount = course.DiscountPrice ?? course.Price;
        }

        return new EnrollmentDetailsResponse(
            enrollment.Id,
            enrollment.UserId,
            user?.GetFullName() ?? "Unknown User",
            user?.Email ?? "Unknown Email",
            enrollment.CourseId,
            course?.Title ?? "Unknown Course",
            course?.Description ?? "No description available",
            enrollment.EnrollmentDate,
            enrollment.CompletionDate,
            enrollment.Progress,
            enrollment.Status,
            completedLessons,
            totalLessons,
            totalTimeSpent,
            enrollment.PaymentId,
            paymentAmount,
            enrollment.CreatedAt,
            enrollment.UpdatedAt
        );
    }
}