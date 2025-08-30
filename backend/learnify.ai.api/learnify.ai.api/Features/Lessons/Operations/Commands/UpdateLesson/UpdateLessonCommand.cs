using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record UpdateLessonCommand(
    int Id,
    string? Title = null,
    string? Description = null,
    string? Content = null,
    string? VideoUrl = null,
    int? Duration = null,
    int? OrderIndex = null,
    bool? IsFree = null,
    bool? IsPublished = null
) : ICommand<LessonResponse?>;

public class UpdateLessonValidator : AbstractValidator<UpdateLessonCommand>
{
    public UpdateLessonValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Lesson title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Lesson description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .WithMessage("Lesson duration must be greater than 0 minutes")
            .When(x => x.Duration.HasValue);

        RuleFor(x => x.VideoUrl)
            .MaximumLength(500)
            .WithMessage("Video URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.VideoUrl));

        RuleFor(x => x.OrderIndex)
            .GreaterThan(0)
            .WithMessage("Order index must be greater than 0")
            .When(x => x.OrderIndex.HasValue);
    }
}

public class UpdateLessonHandler : IRequestHandler<UpdateLessonCommand, LessonResponse?>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public UpdateLessonHandler(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse?> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (lesson == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.Title))
            lesson.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Description))
            lesson.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Content))
            lesson.Content = request.Content;

        if (request.VideoUrl != null)
            lesson.VideoUrl = request.VideoUrl;

        if (request.Duration.HasValue)
            lesson.Duration = request.Duration.Value;

        if (request.OrderIndex.HasValue)
            lesson.OrderIndex = request.OrderIndex.Value;

        if (request.IsFree.HasValue)
            lesson.IsFree = request.IsFree.Value;

        if (request.IsPublished.HasValue)
            lesson.IsPublished = request.IsPublished.Value;

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