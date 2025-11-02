using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;

using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Domain.Enums;
namespace learnify.ai.api.Features.Assessments;

public record AddQuestionToQuizCommand(
    int QuizId,
    string QuestionText,
    QuestionType QuestionType,
    int Points = 1,
    int? OrderIndex = null
) : ICommand<QuestionSummaryResponse>;

public class AddQuestionToQuizValidator : AbstractValidator<AddQuestionToQuizCommand>
{
    public AddQuestionToQuizValidator()
    {
        RuleFor(x => x.QuizId)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");

        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage("Question text is required")
            .MaximumLength(1000)
            .WithMessage("Question text cannot exceed 1000 characters");

        RuleFor(x => x.Points)
            .GreaterThan(0)
            .WithMessage("Points must be greater than 0");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .When(x => x.OrderIndex.HasValue)
            .WithMessage("Order index must be 0 or greater");
    }
}

public class AddQuestionToQuizHandler : IRequestHandler<AddQuestionToQuizCommand, QuestionSummaryResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;

    public AddQuestionToQuizHandler(
        IQuizRepository quizRepository,
        IQuestionRepository questionRepository)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
    }

    public async Task<QuestionSummaryResponse> Handle(AddQuestionToQuizCommand request, CancellationToken cancellationToken)
    {
        // Verify quiz exists
        var quiz = await _quizRepository.GetByIdAsync(request.QuizId, cancellationToken);
        if (quiz == null)
            throw new ArgumentException($"Quiz with ID {request.QuizId} not found");

        // Determine order index
        var orderIndex = request.OrderIndex ?? await _questionRepository.GetMaxOrderIndexAsync(request.QuizId, cancellationToken) + 1;

        // Create question
        var question = new Question
        {
            QuizId = request.QuizId,
            QuestionText = request.QuestionText,
            QuestionType = request.QuestionType,
            Points = request.Points,
            OrderIndex = orderIndex,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdQuestion = await _questionRepository.CreateAsync(question, cancellationToken);

        return new QuestionSummaryResponse(
            createdQuestion.Id,
            createdQuestion.QuizId,
            createdQuestion.QuestionText,
            createdQuestion.QuestionType,
            createdQuestion.Points,
            createdQuestion.OrderIndex,
            createdQuestion.IsActive,
            0 // AnswerCount - initially 0
        );
    }
}