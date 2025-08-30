using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Enrollments;

public record UpdateEnrollmentStatusCommand(
    int EnrollmentId,
    EnrollmentStatus Status
) : ICommand<EnrollmentResponse?>;

public class UpdateEnrollmentStatusValidator : AbstractValidator<UpdateEnrollmentStatusCommand>
{
    public UpdateEnrollmentStatusValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid enrollment status");
    }
}

public class UpdateEnrollmentStatusHandler : IRequestHandler<UpdateEnrollmentStatusCommand, EnrollmentResponse?>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public UpdateEnrollmentStatusHandler(
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<EnrollmentResponse?> Handle(UpdateEnrollmentStatusCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            return null;

        // Validate status transition
        if (enrollment.Status == EnrollmentStatus.Completed && request.Status != EnrollmentStatus.Completed)
            throw new InvalidOperationException("Cannot change status of a completed enrollment");

        enrollment.Status = request.Status;

        // Set completion date if completing
        if (request.Status == EnrollmentStatus.Completed && enrollment.CompletionDate == null)
        {
            enrollment.CompletionDate = DateTime.UtcNow;
            enrollment.Progress = 100;
        }

        enrollment.UpdatedAt = DateTime.UtcNow;

        var updatedEnrollment = await _enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        // Load related data
        var user = await _userRepository.GetByIdAsync(updatedEnrollment.UserId, cancellationToken);
        var course = await _courseRepository.GetByIdAsync(updatedEnrollment.CourseId, cancellationToken);

        return new EnrollmentResponse(
            updatedEnrollment.Id,
            updatedEnrollment.UserId,
            user?.GetFullName() ?? "Unknown User",
            updatedEnrollment.CourseId,
            course?.Title ?? "Unknown Course",
            updatedEnrollment.EnrollmentDate,
            updatedEnrollment.CompletionDate,
            updatedEnrollment.Progress,
            updatedEnrollment.Status,
            updatedEnrollment.PaymentId,
            updatedEnrollment.CreatedAt,
            updatedEnrollment.UpdatedAt
        );
    }
}