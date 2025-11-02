using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Enrollments;

public record EnrollInCourseCommand(
    int UserId,
    int CourseId,
    int? PaymentId = null
) : ICommand<EnrollmentResponse>;

public class EnrollInCourseValidator : AbstractValidator<EnrollInCourseCommand>
{
    public EnrollInCourseValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .When(x => x.PaymentId.HasValue)
            .WithMessage("Payment ID must be greater than 0 when provided");
    }
}

public class EnrollInCourseHandler : IRequestHandler<EnrollInCourseCommand, EnrollmentResponse>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollInCourseHandler(
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<EnrollmentResponse> Handle(EnrollInCourseCommand request, CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Check if course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Check if course is published
        if (!course.IsPublished)
            throw new InvalidOperationException("Cannot enroll in unpublished course");

        // Check if user is already enrolled
        var existingEnrollment = await _enrollmentRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId, cancellationToken);
        if (existingEnrollment != null)
            throw new InvalidOperationException("User is already enrolled in this course");

        // Check course capacity
        if (course.MaxStudents.HasValue)
        {
            var enrollmentCount = await _enrollmentRepository.GetEnrollmentCountByCourseAsync(request.CourseId, cancellationToken);
            if (enrollmentCount >= course.MaxStudents.Value)
                throw new InvalidOperationException("Course has reached maximum enrollment capacity");
        }

        // Create enrollment
        var enrollment = new Enrollment
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Active,
            Progress = 0,
            PaymentId = request.PaymentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdEnrollment = await _enrollmentRepository.CreateAsync(enrollment, cancellationToken);

        return new EnrollmentResponse(
            createdEnrollment.Id,
            createdEnrollment.UserId,
            user.GetFullName(),
            createdEnrollment.CourseId,
            course.Title,
            createdEnrollment.EnrollmentDate,
            createdEnrollment.CompletionDate,
            createdEnrollment.Progress,
            createdEnrollment.Status,
            createdEnrollment.PaymentId,
            createdEnrollment.CreatedAt,
            createdEnrollment.UpdatedAt
        );
    }
}