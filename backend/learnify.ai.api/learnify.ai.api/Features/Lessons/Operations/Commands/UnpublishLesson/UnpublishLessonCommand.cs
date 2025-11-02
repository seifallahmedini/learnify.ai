using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record UnpublishLessonCommand(int Id) : ICommand<LessonResponse?>;

public class UnpublishLessonValidator : AbstractValidator<UnpublishLessonCommand>
{
    public UnpublishLessonValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");
    }
}

public class UnpublishLessonHandler : IRequestHandler<UnpublishLessonCommand, LessonResponse?>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public UnpublishLessonHandler(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse?> Handle(UnpublishLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (lesson == null)
            return null;

        lesson.IsPublished = false;
        lesson.UpdatedAt = DateTime.UtcNow;
        var updatedLesson = await _lessonRepository.UpdateAsync(lesson, cancellationToken);

        var course = await _courseRepository.GetByIdAsync(updatedLesson.CourseId, cancellationToken);

        return new LessonResponse(
            updatedLesson.Id,
            updatedLesson.CourseId,
            course?.Title ?? "Unknown Course",
            updatedLesson.Title,
            updatedLesson.Description,
            updatedLesson.Content,
            updatedLesson.VideoUrl,
            updatedLesson.Duration,
            updatedLesson.OrderIndex,
            updatedLesson.IsFree,
            updatedLesson.IsPublished,
            updatedLesson.CreatedAt,
            updatedLesson.UpdatedAt
        );
    }
}
