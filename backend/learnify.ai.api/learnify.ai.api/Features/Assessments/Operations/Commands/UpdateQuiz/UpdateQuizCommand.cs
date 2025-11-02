using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Assessments;

public record UpdateQuizCommand(
    int Id,
    string? Title = null,
    string? Description = null,
    int? TimeLimit = null,
    int? PassingScore = null,
    int? MaxAttempts = null,
    bool? IsActive = null
) : ICommand<QuizResponse?>;

public class UpdateQuizValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Quiz title cannot exceed 200 characters and cannot be empty");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Quiz description cannot exceed 1000 characters and cannot be empty");

        RuleFor(x => x.TimeLimit)
            .GreaterThan(0)
            .When(x => x.TimeLimit.HasValue)
            .WithMessage("Time limit must be greater than 0 minutes");

        RuleFor(x => x.PassingScore)
            .InclusiveBetween(0, 100)
            .When(x => x.PassingScore.HasValue)
            .WithMessage("Passing score must be between 0 and 100");

        RuleFor(x => x.MaxAttempts)
            .GreaterThan(0)
            .When(x => x.MaxAttempts.HasValue)
            .WithMessage("Max attempts must be greater than 0");
    }
}

public class UpdateQuizHandler : IRequestHandler<UpdateQuizCommand, QuizResponse?>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IQuestionRepository _questionRepository;

    public UpdateQuizHandler(
        IQuizRepository quizRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        IQuestionRepository questionRepository)
    {
        _quizRepository = quizRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
        _questionRepository = questionRepository;
    }

    public async Task<QuizResponse?> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quiz == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.Title))
        {
            quiz.Title = request.Title;
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            quiz.Description = request.Description;
        }

        if (request.TimeLimit.HasValue)
        {
            quiz.TimeLimit = request.TimeLimit.Value;
        }

        if (request.PassingScore.HasValue)
        {
            quiz.PassingScore = request.PassingScore.Value;
        }

        if (request.MaxAttempts.HasValue)
        {
            quiz.MaxAttempts = request.MaxAttempts.Value;
        }

        if (request.IsActive.HasValue)
        {
            quiz.IsActive = request.IsActive.Value;
        }

        quiz.UpdatedAt = DateTime.UtcNow;
        var updatedQuiz = await _quizRepository.UpdateAsync(quiz, cancellationToken);

        // Load related data
        var course = await _courseRepository.GetByIdAsync(updatedQuiz.CourseId, cancellationToken);
        string? lessonTitle = null;
        if (updatedQuiz.LessonId.HasValue)
        {
            var lesson = await _lessonRepository.GetByIdAsync(updatedQuiz.LessonId.Value, cancellationToken);
            lessonTitle = lesson?.Title;
        }

        var questionCount = await _quizRepository.GetQuestionCountAsync(updatedQuiz.Id, cancellationToken);
        var totalPoints = await _quizRepository.GetTotalPointsAsync(updatedQuiz.Id, cancellationToken);

        return new QuizResponse(
            updatedQuiz.Id,
            updatedQuiz.CourseId,
            course?.Title ?? "Unknown Course",
            updatedQuiz.LessonId,
            lessonTitle,
            updatedQuiz.Title,
            updatedQuiz.Description,
            updatedQuiz.TimeLimit,
            updatedQuiz.PassingScore,
            updatedQuiz.MaxAttempts,
            updatedQuiz.IsActive,
            questionCount,
            totalPoints,
            updatedQuiz.CreatedAt,
            updatedQuiz.UpdatedAt
        );
    }
}
