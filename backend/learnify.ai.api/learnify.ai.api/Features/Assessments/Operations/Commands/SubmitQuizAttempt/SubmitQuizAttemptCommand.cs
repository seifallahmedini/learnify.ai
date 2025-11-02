using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Assessments;

public record SubmitQuizAttemptCommand(
    int AttemptId,
    IEnumerable<QuizAnswerSubmission> Answers
) : ICommand<SubmitQuizAttemptResponse>;

public class SubmitQuizAttemptValidator : AbstractValidator<SubmitQuizAttemptCommand>
{
    public SubmitQuizAttemptValidator()
    {
        RuleFor(x => x.AttemptId)
            .GreaterThan(0)
            .WithMessage("Attempt ID must be greater than 0");

        RuleFor(x => x.Answers)
            .NotEmpty()
            .WithMessage("Answers are required");

        RuleForEach(x => x.Answers).ChildRules(answer =>
        {
            answer.RuleFor(a => a.QuestionId)
                .GreaterThan(0)
                .WithMessage("Question ID must be greater than 0");

            answer.RuleFor(a => a.SelectedAnswerIds)
                .NotEmpty()
                .WithMessage("At least one answer must be selected for each question");
        });
    }
}

public class SubmitQuizAttemptHandler : IRequestHandler<SubmitQuizAttemptCommand, SubmitQuizAttemptResponse>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly IUserRepository _userRepository;

    public SubmitQuizAttemptHandler(
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        IQuestionRepository questionRepository,
        IAnswerRepository answerRepository,
        IUserRepository userRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
        _userRepository = userRepository;
    }

    public async Task<SubmitQuizAttemptResponse> Handle(SubmitQuizAttemptCommand request, CancellationToken cancellationToken)
    {
        // Get the quiz attempt
        var attempt = await _quizAttemptRepository.GetByIdAsync(request.AttemptId, cancellationToken);
        if (attempt == null)
            throw new ArgumentException($"Quiz attempt with ID {request.AttemptId} not found");

        if (attempt.IsCompleted())
            throw new InvalidOperationException("Quiz attempt has already been completed");

        // Get the quiz
        var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, cancellationToken);
        if (quiz == null)
            throw new InvalidOperationException("Associated quiz not found");

        // Check time limit if applicable
        if (quiz.HasTimeLimit())
        {
            var elapsed = DateTime.UtcNow - attempt.StartedAt;
            if (elapsed.TotalMinutes > quiz.TimeLimit!.Value)
            {
                throw new InvalidOperationException("Time limit exceeded for this quiz attempt");
            }
        }

        // Get all questions for the quiz
        var questions = await _questionRepository.GetActiveQuestionsAsync(quiz.Id, cancellationToken);
        var questionsList = questions.ToList();

        // Validate that all questions are answered
        var answeredQuestionIds = request.Answers.Select(a => a.QuestionId).ToHashSet();
        var requiredQuestionIds = questionsList.Select(q => q.Id).ToHashSet();
        
        var missingQuestions = requiredQuestionIds.Except(answeredQuestionIds);
        if (missingQuestions.Any())
        {
            throw new InvalidOperationException($"Missing answers for questions: {string.Join(", ", missingQuestions)}");
        }

        // Calculate score
        int totalScore = 0;
        var attemptAnswers = new List<QuizAttemptAnswerResponse>();

        foreach (var answer in request.Answers)
        {
            var question = questionsList.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question == null)
                throw new ArgumentException($"Question with ID {answer.QuestionId} not found in quiz");

            // Get all answers for this question
            var questionAnswers = await _answerRepository.GetByQuestionIdAsync(question.Id, cancellationToken);
            var questionAnswersList = questionAnswers.ToList();

            // Get correct answers
            var correctAnswers = questionAnswersList.Where(a => a.IsCorrect).Select(a => a.Id).ToHashSet();
            var selectedAnswers = answer.SelectedAnswerIds.ToHashSet();

            // Check if answer is correct
            bool isCorrect = false;
            int pointsEarned = 0;

            if (question.IsMultipleChoice() || question.IsTrueFalse())
            {
                // For single-choice questions, must select exactly one correct answer
                isCorrect = selectedAnswers.Count == 1 && 
                           correctAnswers.Count == 1 && 
                           selectedAnswers.SetEquals(correctAnswers);
            }
            else
            {
                // For other question types, implement specific logic as needed
                isCorrect = selectedAnswers.SetEquals(correctAnswers);
            }

            if (isCorrect)
            {
                pointsEarned = question.Points;
                totalScore += pointsEarned;
            }

            // Build answer options with selection and correctness info
            var answerOptions = questionAnswersList.Select(a => new QuizAttemptAnswerOptionResponse(
                a.Id,
                a.AnswerText,
                selectedAnswers.Contains(a.Id),
                a.IsCorrect
            )).ToList();

            attemptAnswers.Add(new QuizAttemptAnswerResponse(
                question.Id,
                question.QuestionText,
                question.QuestionType,
                question.Points,
                selectedAnswers,
                answerOptions,
                isCorrect,
                pointsEarned
            ));
        }

        // Complete the attempt
        attempt.CompleteAttempt(totalScore, quiz.PassingScore);
        await _quizAttemptRepository.UpdateAsync(attempt, cancellationToken);

        // Get user information
        var user = await _userRepository.GetByIdAsync(attempt.UserId, cancellationToken);

        return new SubmitQuizAttemptResponse(
            attempt.Id,
            quiz.Id,
            quiz.Title,
            attempt.UserId,
            user?.GetFullName() ?? "Unknown User",
            attempt.StartedAt,
            attempt.CompletedAt!.Value,
            attempt.Score,
            attempt.MaxScore,
            attempt.GetScorePercentage(),
            attempt.IsPassed,
            attempt.TimeSpent,
            attempt.GetFormattedTimeSpent(),
            attemptAnswers.OrderBy(a => questionsList.First(q => q.Id == a.QuestionId).OrderIndex)
        );
    }
}