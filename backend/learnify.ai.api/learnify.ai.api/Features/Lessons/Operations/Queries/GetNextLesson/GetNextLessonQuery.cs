using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record GetNextLessonQuery(int Id) : IQuery<LessonResponse?>;

public class GetNextLessonValidator : AbstractValidator<GetNextLessonQuery>
{
    public GetNextLessonValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");
    }
}

public class GetNextLessonHandler : IRequestHandler<GetNextLessonQuery, LessonResponse?>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public GetNextLessonHandler(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse?> Handle(GetNextLessonQuery request, CancellationToken cancellationToken)
    {
        var currentLesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (currentLesson == null)
            return null;

        var nextLesson = await _lessonRepository.GetNextLessonAsync(
            currentLesson.CourseId, 
            currentLesson.OrderIndex, 
            cancellationToken);

        if (nextLesson == null)
            return null;

        var course = await _courseRepository.GetByIdAsync(nextLesson.CourseId, cancellationToken);

        return new LessonResponse(
            nextLesson.Id,
            nextLesson.CourseId,
            course?.Title ?? "Unknown Course",
            nextLesson.Title,
            nextLesson.Description,
            nextLesson.Content,
            nextLesson.VideoUrl,
            nextLesson.Duration,
            nextLesson.OrderIndex,
            nextLesson.IsFree,
            nextLesson.IsPublished,
            nextLesson.CreatedAt,
            nextLesson.UpdatedAt
        );
    }
}