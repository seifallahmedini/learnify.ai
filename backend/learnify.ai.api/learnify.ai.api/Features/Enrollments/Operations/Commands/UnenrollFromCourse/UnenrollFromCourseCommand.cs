using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
namespace learnify.ai.api.Features.Enrollments;

public record UnenrollFromCourseCommand(
    int EnrollmentId
) : ICommand<bool>;

public class UnenrollFromCourseValidator : AbstractValidator<UnenrollFromCourseCommand>
{
    public UnenrollFromCourseValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .GreaterThan(0)
            .WithMessage("Enrollment ID must be greater than 0");
    }
}

public class UnenrollFromCourseHandler : IRequestHandler<UnenrollFromCourseCommand, bool>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IProgressRepository _progressRepository;

    public UnenrollFromCourseHandler(
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
    }

    public async Task<bool> Handle(UnenrollFromCourseCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null)
            return false;

        // Check if enrollment can be cancelled
        if (enrollment.Status == EnrollmentStatus.Completed)
            throw new InvalidOperationException("Cannot unenroll from a completed course");

        // Update status to dropped instead of deleting
        enrollment.Status = EnrollmentStatus.Dropped;
        enrollment.UpdatedAt = DateTime.UtcNow;

        await _enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        return true;
    }
}