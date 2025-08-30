using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Enrollments;

public record GetCourseEnrollmentsQuery(
    int CourseId,
    int Page = 1,
    int PageSize = 10,
    EnrollmentStatus? Status = null,
    DateTime? EnrolledAfter = null,
    DateTime? EnrolledBefore = null
) : IQuery<CourseEnrollmentsResponse>;

public class GetCourseEnrollmentsValidator : AbstractValidator<GetCourseEnrollmentsQuery>
{
    public GetCourseEnrollmentsValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetCourseEnrollmentsHandler : IRequestHandler<GetCourseEnrollmentsQuery, CourseEnrollmentsResponse>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCourseEnrollmentsHandler(
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CourseEnrollmentsResponse> Handle(GetCourseEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get all enrollments for the course
        var allEnrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);

        // Apply filters
        var filteredEnrollments = allEnrollments.AsQueryable();

        if (request.Status.HasValue)
        {
            filteredEnrollments = filteredEnrollments.Where(e => e.Status == request.Status.Value);
        }

        if (request.EnrolledAfter.HasValue)
        {
            filteredEnrollments = filteredEnrollments.Where(e => e.EnrollmentDate >= request.EnrolledAfter.Value);
        }

        if (request.EnrolledBefore.HasValue)
        {
            filteredEnrollments = filteredEnrollments.Where(e => e.EnrollmentDate <= request.EnrolledBefore.Value);
        }

        // Get total count before pagination
        var totalCount = filteredEnrollments.Count();

        // Apply pagination
        var pagedEnrollments = filteredEnrollments
            .OrderByDescending(e => e.EnrollmentDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Load user data for each enrollment
        var enrollmentResponses = new List<EnrollmentResponse>();
        foreach (var enrollment in pagedEnrollments)
        {
            var user = await _userRepository.GetByIdAsync(enrollment.UserId, cancellationToken);
            enrollmentResponses.Add(new EnrollmentResponse(
                enrollment.Id,
                enrollment.UserId,
                user?.GetFullName() ?? "Unknown User",
                enrollment.CourseId,
                course.Title,
                enrollment.EnrollmentDate,
                enrollment.CompletionDate,
                enrollment.Progress,
                enrollment.Status,
                enrollment.PaymentId,
                enrollment.CreatedAt,
                enrollment.UpdatedAt
            ));
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new CourseEnrollmentsResponse(
            request.CourseId,
            course.Title,
            enrollmentResponses,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}