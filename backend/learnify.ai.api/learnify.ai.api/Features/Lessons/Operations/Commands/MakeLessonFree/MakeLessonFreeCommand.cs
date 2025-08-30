using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record MakeLessonFreeCommand(int Id, bool IsFree = true) : ICommand<LessonResponse?>;

public class MakeLessonFreeValidator : AbstractValidator<MakeLessonFreeCommand>
{
    public MakeLessonFreeValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");
    }
}

public class MakeLessonFreeHandler : IRequestHandler<MakeLessonFreeCommand, LessonResponse?>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public MakeLessonFreeHandler(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse?> Handle(MakeLessonFreeCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (lesson == null)
            return null;

        lesson.IsFree = request.IsFree;
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