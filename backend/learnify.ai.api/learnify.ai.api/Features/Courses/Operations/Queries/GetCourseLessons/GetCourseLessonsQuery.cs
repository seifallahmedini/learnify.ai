using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses.Infrastructure.Data;

namespace learnify.ai.api.Features.Courses.Operations.Queries.GetCourseLessons;

public record GetCourseLessonsQuery(
    int CourseId,
    bool? IsPublished = null
) : IQuery<CourseLessonsResponse>;

public record CourseLessonsResponse(
    int CourseId,
    string CourseTitle,
    IEnumerable<LessonSummary> Lessons,
    int TotalCount,
    int PublishedCount,
    int DraftCount,
    int TotalDuration
);

public record LessonSummary(
    int Id,
    string Title,
    string Description,
    int Duration,
    int OrderIndex,
    bool IsFree,
    bool IsPublished,
    string? VideoUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public class GetCourseLessonsValidator : AbstractValidator<GetCourseLessonsQuery>
{
    public GetCourseLessonsValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class GetCourseLessonsHandler : IRequestHandler<GetCourseLessonsQuery, CourseLessonsResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public GetCourseLessonsHandler(
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<CourseLessonsResponse> Handle(GetCourseLessonsQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get lessons for the course
        var lessons = await _lessonRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
        var lessonsList = lessons.ToList();

        // Apply filter if specified
        if (request.IsPublished.HasValue)
        {
            lessonsList = lessonsList.Where(l => l.IsPublished == request.IsPublished.Value).ToList();
        }

        // Calculate statistics
        var totalCount = lessonsList.Count;
        var publishedCount = lessonsList.Count(l => l.IsPublished);
        var draftCount = lessonsList.Count(l => !l.IsPublished);
        var totalDuration = lessonsList.Sum(l => l.Duration);

        // Map to response objects
        var lessonSummaries = lessonsList
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LessonSummary(
                l.Id,
                l.Title,
                l.Description,
                l.Duration,
                l.OrderIndex,
                l.IsFree,
                l.IsPublished,
                l.VideoUrl,
                l.CreatedAt,
                l.UpdatedAt
            ));

        return new CourseLessonsResponse(
            course.Id,
            course.Title,
            lessonSummaries,
            totalCount,
            publishedCount,
            draftCount,
            totalDuration
        );
    }
}