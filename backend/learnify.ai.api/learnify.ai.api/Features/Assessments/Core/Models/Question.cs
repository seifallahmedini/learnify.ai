namespace learnify.ai.api.Features.Assessments.Core.Models;

public class Question
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public int Points { get; set; } = 1;
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Business methods
    public bool IsMultipleChoice() => QuestionType == QuestionType.MultipleChoice;
    public bool IsTrueFalse() => QuestionType == QuestionType.TrueFalse;
    public bool IsShortAnswer() => QuestionType == QuestionType.ShortAnswer;
    public bool IsEssay() => QuestionType == QuestionType.Essay;
}

public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    ShortAnswer = 3,
    Essay = 4
}