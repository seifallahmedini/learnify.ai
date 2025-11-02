using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Assessments;

public record StartQuizAttemptCommand(
    int QuizId,
    int UserId
) : ICommand<StartQuizAttemptResponse>;

public class StartQuizAttemptValidator : AbstractValidator<StartQuizAttemptCommand>
{
    public StartQuizAttemptValidator()
    {
        RuleFor(x => x.QuizId)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");
    }
}

public class StartQuizAttemptHandler : IRequestHandler<StartQuizAttemptCommand, StartQuizAttemptResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IUserRepository _userRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IAnswerRepository _answerRepository;

    public StartQuizAttemptHandler(
        IQuizRepository quizRepository,
        IQuizAttemptRepository quizAttemptRepository,
        IUserRepository userRepository,
        IQuestionRepository questionRepository,
        IAnswerRepository answerRepository)
    {
        _quizRepository = quizRepository;
        _quizAttemptRepository = quizAttemptRepository;
        _userRepository = userRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
    }

    public async Task<StartQuizAttemptResponse> Handle(StartQuizAttemptCommand request, CancellationToken cancellationToken)
    {
        // Verify quiz exists and is active
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            throw new ArgumentException($"Quiz with ID {request.QuizId} not found");

        if (!quiz.IsActive)
            throw new InvalidOperationException("Quiz is not active");

        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new ArgumentException($"User with ID {request.UserId} not found");

        // Check if user has exceeded max attempts
        var attemptCount = await _quizAttemptRepository.GetAttemptCountAsync(request.UserId, request.QuizId, cancellationToken);
        if (attemptCount >= quiz.MaxAttempts)
            throw new InvalidOperationException($"User has exceeded maximum attempts ({quiz.MaxAttempts}) for this quiz");

        // Check if user has an active attempt
        var inProgressAttempts = await _quizAttemptRepository.GetInProgressAttemptsAsync(request.UserId, cancellationToken);
        var activeAttempt = inProgressAttempts.FirstOrDefault(a => a.QuizId == request.QuizId);
        if (activeAttempt != null)
            throw new InvalidOperationException("User already has an active attempt for this quiz");

        // Get quiz questions
        var questions = await _questionRepository.GetActiveQuestionsAsync(request.QuizId, cancellationToken);
        var questionsList = questions.OrderBy(q => q.OrderIndex).ToList();

        if (!questionsList.Any())
            throw new InvalidOperationException("Quiz has no active questions");

        // Calculate max score
        var maxScore = questionsList.Sum(q => q.Points);

        // Create quiz attempt
        var attempt = new QuizAttempt
        {
            QuizId = request.QuizId,
            UserId = request.UserId,
            Score = 0,
            MaxScore = maxScore,
            StartedAt = DateTime.UtcNow,
            TimeSpent = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdAttempt = await _quizAttemptRepository.CreateAsync(attempt, cancellationToken);

        // Build questions for response (without correct answers)
        var quizQuestions = new List<QuizQuestionResponse>();
        foreach (var question in questionsList)
        {
            var answers = await _answerRepository.GetByQuestionIdAsync(question.Id, cancellationToken);
            var answersList = answers.OrderBy(a => a.OrderIndex).ToList();

            var quizAnswers = answersList.Select(a => new QuizAnswerResponse(
                a.Id,
                a.AnswerText,
                a.OrderIndex
                // Note: IsCorrect is not included for student responses
            )).ToList();

            quizQuestions.Add(new QuizQuestionResponse(
                question.Id,
                question.QuestionText,
                question.QuestionType,
                question.Points,
                question.OrderIndex,
                quizAnswers
            ));
        }

        // Calculate expiration time
        DateTime? expiresAt = null;
        if (quiz.HasTimeLimit())
        {
            expiresAt = createdAttempt.StartedAt.AddMinutes(quiz.TimeLimit!.Value);
        }

        return new StartQuizAttemptResponse(
            createdAttempt.Id,
            quiz.Id,
            quiz.Title,
            request.UserId,
            createdAttempt.StartedAt,
            quiz.TimeLimit,
            expiresAt,
            quizQuestions
        );
    }
}