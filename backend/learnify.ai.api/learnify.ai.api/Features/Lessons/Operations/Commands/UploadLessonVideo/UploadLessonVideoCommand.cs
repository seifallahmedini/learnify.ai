using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record UploadLessonVideoCommand(int Id, string VideoUrl) : ICommand<LessonResponse?>;

public class UploadLessonVideoValidator : AbstractValidator<UploadLessonVideoCommand>
{
    public UploadLessonVideoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");

        RuleFor(x => x.VideoUrl)
            .NotEmpty()
            .WithMessage("Video URL is required")
            .MaximumLength(500)
            .WithMessage("Video URL cannot exceed 500 characters");
    }
}

public class UploadLessonVideoHandler : IRequestHandler<UploadLessonVideoCommand, LessonResponse?>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public UploadLessonVideoHandler(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse?> Handle(UploadLessonVideoCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (lesson == null)
            return null;

        lesson.VideoUrl = request.VideoUrl;
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
            updatedLesson.LearningObjectives,
            updatedLesson.Resources,
            updatedLesson.CreatedAt,
            updatedLesson.UpdatedAt
        );
    }
}
