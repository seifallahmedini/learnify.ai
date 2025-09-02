using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Assessments;

public record CreateQuizCommand(
    int CourseId,
    int? LessonId,
    string Title,
    string Description,
    int? TimeLimit = null,
    int PassingScore = 70,
    int MaxAttempts = 3,
    bool IsActive = true
) : ICommand<QuizResponse>;

public class CreateQuizValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.LessonId)
            .GreaterThan(0)
            .When(x => x.LessonId.HasValue)
            .WithMessage("Lesson ID must be greater than 0 when provided");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Quiz title is required")
            .MaximumLength(200)
            .WithMessage("Quiz title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Quiz description is required")
            .MaximumLength(1000)
            .WithMessage("Quiz description cannot exceed 1000 characters");

        RuleFor(x => x.TimeLimit)
            .GreaterThan(0)
            .When(x => x.TimeLimit.HasValue)
            .WithMessage("Time limit must be greater than 0 minutes");

        RuleFor(x => x.PassingScore)
            .InclusiveBetween(0, 100)
            .WithMessage("Passing score must be between 0 and 100");

        RuleFor(x => x.MaxAttempts)
            .GreaterThan(0)
            .WithMessage("Max attempts must be greater than 0");
    }
}

public class CreateQuizHandler : IRequestHandler<CreateQuizCommand, QuizResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public CreateQuizHandler(
        IQuizRepository quizRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        _quizRepository = quizRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<QuizResponse> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Verify lesson exists if provided
        string? lessonTitle = null;
        if (request.LessonId.HasValue)
        {
            var lesson = await _lessonRepository.GetByIdAsync(request.LessonId.Value, cancellationToken);
            if (lesson == null)
                throw new ArgumentException($"Lesson with ID {request.LessonId} not found");

            if (lesson.CourseId != request.CourseId)
                throw new ArgumentException("Lesson does not belong to the specified course");

            lessonTitle = lesson.Title;
        }

        // Create quiz
        var quiz = new Quiz
        {
            CourseId = request.CourseId,
            LessonId = request.LessonId,
            Title = request.Title,
            Description = request.Description,
            TimeLimit = request.TimeLimit,
            PassingScore = request.PassingScore,
            MaxAttempts = request.MaxAttempts,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdQuiz = await _quizRepository.CreateAsync(quiz, cancellationToken);

        return new QuizResponse(
            createdQuiz.Id,
            createdQuiz.CourseId,
            course.Title,
            createdQuiz.LessonId,
            lessonTitle,
            createdQuiz.Title,
            createdQuiz.Description,
            createdQuiz.TimeLimit,
            createdQuiz.PassingScore,
            createdQuiz.MaxAttempts,
            createdQuiz.IsActive,
            0, // QuestionCount - initially 0
            0, // TotalPoints - initially 0
            createdQuiz.CreatedAt,
            createdQuiz.UpdatedAt
        );
    }
}