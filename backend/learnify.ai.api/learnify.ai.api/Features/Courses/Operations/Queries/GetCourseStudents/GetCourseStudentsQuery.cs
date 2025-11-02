using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Courses;

public record GetCourseStudentsQuery(
    int CourseId,
    int Page = 1,
    int PageSize = 10
) : IQuery<CourseStudentsResponse>;

public record CourseStudentsResponse(
    int CourseId,
    string CourseTitle,
    IEnumerable<CourseStudentSummary> Students,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public record CourseStudentSummary(
    int UserId,
    string FullName,
    string Email,
    DateTime EnrollmentDate,
    string EnrollmentStatus,
    decimal Progress,
    DateTime? CompletionDate,
    DateTime? LastAccessDate
);

public class GetCourseStudentsValidator : AbstractValidator<GetCourseStudentsQuery>
{
    public GetCourseStudentsValidator()
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

public class GetCourseStudentsHandler : IRequestHandler<GetCourseStudentsQuery, CourseStudentsResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;

    public GetCourseStudentsHandler(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
    }

    public async Task<CourseStudentsResponse> Handle(GetCourseStudentsQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get all enrollments for the course
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var enrollmentsList = enrollments.ToList();

        var totalCount = enrollmentsList.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        // Apply pagination
        var paginatedEnrollments = enrollmentsList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Get user details for each enrollment
        var students = new List<CourseStudentSummary>();
        
        foreach (var enrollment in paginatedEnrollments)
        {
            var user = await _userRepository.GetByIdAsync(enrollment.UserId, cancellationToken);
            if (user != null)
            {
                students.Add(new CourseStudentSummary(
                    user.Id,
                    user.GetFullName(),
                    user.Email,
                    enrollment.EnrollmentDate,
                    enrollment.Status.ToString(),
                    enrollment.Progress,
                    enrollment.CompletionDate,
                    enrollment.UpdatedAt // Using UpdatedAt as proxy for last access
                ));
            }
        }

        return new CourseStudentsResponse(
            course.Id,
            course.Title,
            students,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}