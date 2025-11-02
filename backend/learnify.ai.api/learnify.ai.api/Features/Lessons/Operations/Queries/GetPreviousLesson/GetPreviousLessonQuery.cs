using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record GetPreviousLessonQuery(int Id) : IQuery<LessonResponse?>;

public class GetPreviousLessonValidator : AbstractValidator<GetPreviousLessonQuery>
{
    public GetPreviousLessonValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");
    }
}

public class GetPreviousLessonHandler : IRequestHandler<GetPreviousLessonQuery, LessonResponse?>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public GetPreviousLessonHandler(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse?> Handle(GetPreviousLessonQuery request, CancellationToken cancellationToken)
    {
        var currentLesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (currentLesson == null)
            return null;

        var previousLesson = await _lessonRepository.GetPreviousLessonAsync(
            currentLesson.CourseId, 
            currentLesson.OrderIndex, 
            cancellationToken);

        if (previousLesson == null)
            return null;

        var course = await _courseRepository.GetByIdAsync(previousLesson.CourseId, cancellationToken);

        return new LessonResponse(
            previousLesson.Id,
            previousLesson.CourseId,
            course?.Title ?? "Unknown Course",
            previousLesson.Title,
            previousLesson.Description,
            previousLesson.Content,
            previousLesson.VideoUrl,
            previousLesson.Duration,
            previousLesson.OrderIndex,
            previousLesson.IsFree,
            previousLesson.IsPublished,
            previousLesson.LearningObjectives,
            previousLesson.Resources,
            previousLesson.CreatedAt,
            previousLesson.UpdatedAt
        );
    }
}
