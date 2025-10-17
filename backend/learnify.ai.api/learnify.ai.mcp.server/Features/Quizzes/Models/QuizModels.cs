using System.ComponentModel.DataAnnotations;

namespace Learnify.Mcp.Server.Features.Quizzes.Models;

#region Quiz Models

/// <summary>
/// Quiz model representing the main quiz entity
/// </summary>
public record QuizModel(
    int Id,
    int? CourseId,
    string? CourseTitle,
    int? LessonId,
    string? LessonTitle,
    string Title,
    string Description,
    int? TimeLimit,
    decimal PassingScore,
    int MaxAttempts,
    bool IsActive,
    int QuestionCount,
    int TotalPoints,
    int AttemptCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Quiz summary model for lightweight operations
/// </summary>
public record QuizSummaryModel(
    int Id,
    int? CourseId,
    string? CourseTitle,
    int? LessonId,
    string? LessonTitle,
    string Title,
    string Description,
    decimal PassingScore,
    int MaxAttempts,
    bool IsActive,
    int QuestionCount,
    DateTime CreatedAt
);

/// <summary>
/// Quiz list response model with pagination
/// </summary>
public record QuizListResponse(
    IEnumerable<QuizSummaryModel> Quizzes,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

/// <summary>
/// Course quizzes response model
/// </summary>
public record CourseQuizzesResponse(
    int CourseId,
    string CourseTitle,
    IEnumerable<QuizSummaryModel> Quizzes,
    int TotalCount
);

/// <summary>
/// Quiz statistics model
/// </summary>
public record QuizStatsModel(
    int QuizId,
    string QuizTitle,
    int TotalAttempts,
    int CompletedAttempts,
    int PassedAttempts,
    decimal AverageScore,
    decimal PassRate,
    int UniqueParticipants,
    TimeSpan AverageCompletionTime,
    DateTime LastAttempt
);

#endregion

#region Question Models

/// <summary>
/// Question model for quiz questions
/// </summary>
public record QuestionModel(
    int Id,
    int QuizId,
    string QuestionText,
    QuestionType Type,
    int Points,
    int OrderIndex,
    bool IsRequired,
    IEnumerable<AnswerOptionModel> AnswerOptions,
    DateTime CreatedAt
);

/// <summary>
/// Question summary model
/// </summary>
public record QuestionSummaryModel(
    int Id,
    int QuizId,
    string QuestionText,
    QuestionType Type,
    int Points,
    int OrderIndex,
    int AnswerOptionsCount
);

/// <summary>
/// Answer option model
/// </summary>
public record AnswerOptionModel(
    int Id,
    int QuestionId,
    string OptionText,
    bool IsCorrect,
    int OrderIndex
);

/// <summary>
/// Quiz questions response model
/// </summary>
public record QuizQuestionsResponse(
    int QuizId,
    string QuizTitle,
    IEnumerable<QuestionModel> Questions,
    int TotalQuestions,
    int TotalPoints
);

#endregion

#region Attempt Models

/// <summary>
/// Quiz attempt model
/// </summary>
public record QuizAttemptModel(
    int Id,
    int QuizId,
    string QuizTitle,
    int UserId,
    string UserName,
    DateTime StartedAt,
    DateTime? CompletedAt,
    decimal? Score,
    bool IsPassed,
    bool IsCompleted,
    TimeSpan? Duration,
    IEnumerable<QuestionAttemptModel> QuestionAttempts
);

/// <summary>
/// Quiz attempt summary model
/// </summary>
public record QuizAttemptSummaryModel(
    int Id,
    int QuizId,
    string QuizTitle,
    int UserId,
    string UserName,
    DateTime StartedAt,
    DateTime? CompletedAt,
    decimal? Score,
    bool IsPassed,
    bool IsCompleted,
    TimeSpan? Duration
);

/// <summary>
/// Question attempt model
/// </summary>
public record QuestionAttemptModel(
    int Id,
    int AttemptId,
    int QuestionId,
    string QuestionText,
    int? SelectedAnswerId,
    string? AnswerText,
    bool IsCorrect,
    int PointsEarned
);

/// <summary>
/// Quiz attempts response model
/// </summary>
public record QuizAttemptsResponse(
    int QuizId,
    string QuizTitle,
    IEnumerable<QuizAttemptSummaryModel> Attempts,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

/// <summary>
/// Start quiz attempt response model
/// </summary>
public record StartQuizAttemptResponse(
    int AttemptId,
    int QuizId,
    string QuizTitle,
    DateTime StartedAt,
    DateTime? ExpiresAt,
    IEnumerable<QuestionModel> Questions
);

#endregion

#region Request Models

/// <summary>
/// Request model for creating a new quiz
/// </summary>
public record CreateQuizRequest(
    int? CourseId,
    int? LessonId,
    [Required] string Title,
    [Required] string Description,
    int? TimeLimit,
    decimal PassingScore,
    int MaxAttempts = 3,
    bool IsActive = false
);

/// <summary>
/// Request model for updating an existing quiz
/// </summary>
public record UpdateQuizRequest(
    string? Title,
    string? Description,
    int? TimeLimit,
    decimal? PassingScore,
    int? MaxAttempts,
    bool? IsActive
);

/// <summary>
/// Request model for filtering quizzes
/// </summary>
public record QuizFilterRequest(
    int? CourseId = null,
    int? LessonId = null,
    bool? IsActive = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
);

/// <summary>
/// Request model for adding question to quiz
/// </summary>
public record AddQuestionToQuizRequest(
    [Required] string QuestionText,
    QuestionType QuestionType,
    int Points = 1,
    int? OrderIndex = null
);

/// <summary>
/// Request model for starting quiz attempt
/// </summary>
public record StartQuizAttemptRequest(
    int UserId
);

/// <summary>
/// Request model for submitting quiz attempt
/// </summary>
public record SubmitQuizAttemptRequest(
    int AttemptId,
    IEnumerable<QuestionAnswerRequest> Answers
);

/// <summary>
/// Request model for question answer
/// </summary>
public record QuestionAnswerRequest(
    int QuestionId,
    int? SelectedAnswerId,
    string? AnswerText
);

#endregion

#region Enums

/// <summary>
/// Quiz question types
/// </summary>
public enum QuestionType
{
    MultipleChoice = 1,
    TrueFalse = 2,
    ShortAnswer = 3,
    Essay = 4,
    FillInTheBlank = 5
}

/// <summary>
/// Quiz attempt status
/// </summary>
public enum AttemptStatus
{
    InProgress = 1,
    Completed = 2,
    TimedOut = 3,
    Abandoned = 4
}

#endregion