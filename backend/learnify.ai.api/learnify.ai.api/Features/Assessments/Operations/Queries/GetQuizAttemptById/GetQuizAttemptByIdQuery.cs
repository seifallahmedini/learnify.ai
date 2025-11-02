using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Assessments;

public record GetQuizAttemptByIdQuery(
    int Id,
    bool IncludeAnswers = false
) : IQuery<QuizAttemptDetailResponse?>;

public class GetQuizAttemptByIdValidator : AbstractValidator<GetQuizAttemptByIdQuery>
{
    public GetQuizAttemptByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Attempt ID must be greater than 0");
    }
}

public class GetQuizAttemptByIdHandler : IRequestHandler<GetQuizAttemptByIdQuery, QuizAttemptDetailResponse?>
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IUserRepository _userRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IAnswerRepository _answerRepository;

    public GetQuizAttemptByIdHandler(
        IQuizAttemptRepository quizAttemptRepository,
        IQuizRepository quizRepository,
        IUserRepository userRepository,
        IQuestionRepository questionRepository,
        IAnswerRepository answerRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _quizRepository = quizRepository;
        _userRepository = userRepository;
        _questionRepository = questionRepository;
        _answerRepository = answerRepository;
    }

    public async Task<QuizAttemptDetailResponse?> Handle(GetQuizAttemptByIdQuery request, CancellationToken cancellationToken)
    {
        var attempt = await _quizAttemptRepository.GetByIdAsync(request.Id, cancellationToken);
        if (attempt == null)
            return null;

        // Get related data
        var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId, cancellationToken);
        var user = await _userRepository.GetByIdAsync(attempt.UserId, cancellationToken);

        if (quiz == null || user == null)
            throw new InvalidOperationException("Associated quiz or user not found");

        // Calculate time remaining if quiz is in progress and has time limit
        int? timeRemainingMinutes = null;
        if (!attempt.IsCompleted() && quiz.HasTimeLimit())
        {
            var elapsed = (DateTime.UtcNow - attempt.StartedAt).TotalMinutes;
            var remaining = quiz.TimeLimit!.Value - elapsed;
            timeRemainingMinutes = remaining > 0 ? (int)Math.Ceiling(remaining) : 0;
        }

        // Get answers if requested and attempt is completed
        IEnumerable<QuizAttemptAnswerResponse>? answers = null;
        if (request.IncludeAnswers && attempt.IsCompleted())
        {
            answers = await GetAttemptAnswers(attempt, cancellationToken);
        }

        return new QuizAttemptDetailResponse(
            attempt.Id,
            attempt.QuizId,
            quiz.Title,
            attempt.UserId,
            user.GetFullName(),
            attempt.StartedAt,
            attempt.CompletedAt,
            attempt.IsCompleted() ? attempt.Score : null,
            attempt.MaxScore,
            attempt.IsCompleted() ? attempt.GetScorePercentage() : 0,
            attempt.IsCompleted(),
            attempt.IsPassed,
            attempt.TimeSpent,
            attempt.GetFormattedTimeSpent(),
            timeRemainingMinutes,
            answers
        );
    }

    private async Task<IEnumerable<QuizAttemptAnswerResponse>> GetAttemptAnswers(QuizAttempt attempt, CancellationToken cancellationToken)
    {
        // Note: In a real implementation, you would store the user's answers in a separate table
        // For now, this is a placeholder that would need to be implemented based on your data model
        
        var questions = await _questionRepository.GetActiveQuestionsAsync(attempt.QuizId, cancellationToken);
        var questionsList = questions.OrderBy(q => q.OrderIndex).ToList();

        var answers = new List<QuizAttemptAnswerResponse>();

        foreach (var question in questionsList)
        {
            var questionAnswers = await _answerRepository.GetByQuestionIdAsync(question.Id, cancellationToken);
            var answerOptions = questionAnswers.Select(a => new QuizAttemptAnswerOptionResponse(
                a.Id,
                a.AnswerText,
                false, // Would need to get from stored user answers
                a.IsCorrect
            )).ToList();

            answers.Add(new QuizAttemptAnswerResponse(
                question.Id,
                question.QuestionText,
                question.QuestionType,
                question.Points,
                new List<int>(), // Would need to get from stored user answers
                answerOptions,
                false, // Would need to calculate from stored answers
                0 // Would need to calculate from stored answers
            ));
        }

        return answers;
    }
}