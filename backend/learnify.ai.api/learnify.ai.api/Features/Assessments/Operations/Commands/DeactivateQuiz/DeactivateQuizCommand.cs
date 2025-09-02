using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Assessments;

public record DeactivateQuizCommand(
    int Id
) : ICommand<QuizResponse?>;

public class DeactivateQuizValidator : AbstractValidator<DeactivateQuizCommand>
{
    public DeactivateQuizValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");
    }
}

public class DeactivateQuizHandler : IRequestHandler<DeactivateQuizCommand, QuizResponse?>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public DeactivateQuizHandler(
        IQuizRepository quizRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        _quizRepository = quizRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<QuizResponse?> Handle(DeactivateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quiz == null)
            return null;

        quiz.IsActive = false;
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