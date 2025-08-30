using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record CreateLessonCommand(
    int CourseId,
    string Title,
    string Description,
    string Content,
    string? VideoUrl,
    int Duration,
    bool IsFree = false,
    bool IsPublished = false
) : ICommand<LessonResponse>;

public class CreateLessonValidator : AbstractValidator<CreateLessonCommand>
{
    public CreateLessonValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Lesson title is required")
            .MaximumLength(200)
            .WithMessage("Lesson title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Lesson description is required")
            .MaximumLength(1000)
            .WithMessage("Lesson description cannot exceed 1000 characters");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Lesson content is required");

        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .WithMessage("Lesson duration must be greater than 0 minutes");

        RuleFor(x => x.VideoUrl)
            .MaximumLength(500)
            .WithMessage("Video URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.VideoUrl));
    }
}

public class CreateLessonHandler : IRequestHandler<CreateLessonCommand, LessonResponse>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public CreateLessonHandler(
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        // Verify that the course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get the next order index for this course
        var maxOrderIndex = await _lessonRepository.GetMaxOrderIndexAsync(request.CourseId, cancellationToken);
        var nextOrderIndex = maxOrderIndex + 1;

        // Create the lesson
        var lesson = new Lesson
        {
            CourseId = request.CourseId,
            Title = request.Title,
            Description = request.Description,
            Content = request.Content,
            VideoUrl = request.VideoUrl,
            Duration = request.Duration,
            OrderIndex = nextOrderIndex,
            IsFree = request.IsFree,
            IsPublished = request.IsPublished,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdLesson = await _lessonRepository.CreateAsync(lesson, cancellationToken);

        return new LessonResponse(
            createdLesson.Id,
            createdLesson.CourseId,
            course.Title,
            createdLesson.Title,
            createdLesson.Description,
            createdLesson.Content,
            createdLesson.VideoUrl,
            createdLesson.Duration,
            createdLesson.OrderIndex,
            createdLesson.IsFree,
            createdLesson.IsPublished,
            createdLesson.CreatedAt,
            createdLesson.UpdatedAt
        );
    }
}