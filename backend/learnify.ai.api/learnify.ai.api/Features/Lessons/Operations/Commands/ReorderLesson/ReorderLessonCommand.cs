using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record ReorderLessonCommand(int Id, int NewOrderIndex) : ICommand<LessonResponse?>;

public class ReorderLessonValidator : AbstractValidator<ReorderLessonCommand>
{
    public ReorderLessonValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");

        RuleFor(x => x.NewOrderIndex)
            .GreaterThan(0)
            .WithMessage("Order index must be greater than 0");
    }
}

public class ReorderLessonHandler : IRequestHandler<ReorderLessonCommand, LessonResponse?>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public ReorderLessonHandler(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse?> Handle(ReorderLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (lesson == null)
            return null;

        var oldOrderIndex = lesson.OrderIndex;
        var newOrderIndex = request.NewOrderIndex;

        // Get all lessons in the same course
        var courseLessons = await _lessonRepository.GetByCourseIdAsync(lesson.CourseId, cancellationToken);
        var lessonsToUpdate = courseLessons.ToList();

        // Update order indexes
        if (newOrderIndex > oldOrderIndex)
        {
            // Moving down: shift lessons up
            foreach (var l in lessonsToUpdate.Where(l => l.OrderIndex > oldOrderIndex && l.OrderIndex <= newOrderIndex))
            {
                l.OrderIndex--;
                l.UpdatedAt = DateTime.UtcNow;
                await _lessonRepository.UpdateAsync(l, cancellationToken);
            }
        }
        else if (newOrderIndex < oldOrderIndex)
        {
            // Moving up: shift lessons down
            foreach (var l in lessonsToUpdate.Where(l => l.OrderIndex >= newOrderIndex && l.OrderIndex < oldOrderIndex))
            {
                l.OrderIndex++;
                l.UpdatedAt = DateTime.UtcNow;
                await _lessonRepository.UpdateAsync(l, cancellationToken);
            }
        }

        // Update the target lesson
        lesson.OrderIndex = newOrderIndex;
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
