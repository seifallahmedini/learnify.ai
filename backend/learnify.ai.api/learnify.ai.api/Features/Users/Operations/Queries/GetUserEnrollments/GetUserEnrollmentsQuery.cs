using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Users.Data;
using learnify.ai.api.Features.Courses.Infrastructure.Data;
using learnify.ai.api.Features.Enrollments.Infrastructure.Data;
using learnify.ai.api.Features.Enrollments.Core.Models;

namespace learnify.ai.api.Features.Users.Queries.GetUserEnrollments;

public record GetUserEnrollmentsQuery(
    int UserId,
    int Page = 1,
    int PageSize = 10,
    EnrollmentStatus? Status = null
) : IQuery<GetUserEnrollmentsResponse>;

public record GetUserEnrollmentsResponse(
    IEnumerable<UserEnrollmentDto> Enrollments,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
)
{
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public record UserEnrollmentDto(
    int Id,
    int CourseId,
    string CourseTitle,
    string CourseThumbnailUrl,
    DateTime EnrollmentDate,
    DateTime? CompletionDate,
    decimal Progress,
    EnrollmentStatus Status,
    decimal CoursePrice,
    string InstructorName
);

public class GetUserEnrollmentsValidator : AbstractValidator<GetUserEnrollmentsQuery>
{
    public GetUserEnrollmentsValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetUserEnrollmentsHandler : IRequestHandler<GetUserEnrollmentsQuery, GetUserEnrollmentsResponse>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public GetUserEnrollmentsHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<GetUserEnrollmentsResponse> Handle(GetUserEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var allEnrollments = await _enrollmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        // Apply status filter if specified
        if (request.Status.HasValue)
        {
            allEnrollments = allEnrollments.Where(e => e.Status == request.Status.Value);
        }

        var totalCount = allEnrollments.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var paginatedEnrollments = allEnrollments
            .OrderByDescending(e => e.EnrollmentDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var enrollmentDtos = new List<UserEnrollmentDto>();

        foreach (var enrollment in paginatedEnrollments)
        {
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId, cancellationToken);
            var instructor = await _userRepository.GetByIdAsync(course!.InstructorId, cancellationToken);

            enrollmentDtos.Add(new UserEnrollmentDto(
                enrollment.Id,
                course.Id,
                course.Title,
                course.ThumbnailUrl ?? "",
                enrollment.EnrollmentDate,
                enrollment.CompletionDate,
                enrollment.Progress,
                enrollment.Status,
                course.GetEffectivePrice(),
                instructor!.GetFullName()
            ));
        }

        return new GetUserEnrollmentsResponse(
            enrollmentDtos,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}