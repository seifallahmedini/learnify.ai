using System.ComponentModel.DataAnnotations;

namespace Learnify.Mcp.Server.Features.Answers.Models;

#region Answer Models

/// <summary>
/// Answer model representing a quiz question answer option
/// </summary>
public record AnswerModel(
    int Id,
    int QuestionId,
    string QuestionText,
    string AnswerText,
    bool IsCorrect,
    int OrderIndex,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Answer summary model for lightweight operations
/// </summary>
public record AnswerSummaryModel(
    int Id,
    int QuestionId,
    string AnswerText,
    bool IsCorrect,
    int OrderIndex,
    DateTime CreatedAt
);

/// <summary>
/// Answer list response model
/// </summary>
public record AnswerListResponse(
    IEnumerable<AnswerSummaryModel> Answers,
    int TotalCount
);

/// <summary>
/// Question answers response model
/// </summary>
public record QuestionAnswersResponse(
    int QuestionId,
    string QuestionText,
    QuestionType QuestionType,
    IEnumerable<AnswerModel> Answers,
    int TotalAnswers,
    int CorrectAnswersCount
);

/// <summary>
/// Answer validation response model
/// </summary>
public record AnswerValidationResponse(
    int AnswerId,
    bool IsValid,
    IEnumerable<string> ValidationErrors,
    IEnumerable<string> Warnings
);

/// <summary>
/// Answer reorder response model
/// </summary>
public record AnswerReorderResponse(
    int QuestionId,
    bool Success,
    string Message,
    IEnumerable<AnswerOrderItem> UpdatedOrders
);

/// <summary>
/// Answer order item for reordering operations
/// </summary>
public record AnswerOrderItem(
    int AnswerId,
    int OrderIndex
);

/// <summary>
/// Answer statistics model
/// </summary>
public record AnswerStatsModel(
    int AnswerId,
    string AnswerText,
    int TimesSelected,
    int TimesCorrect,
    decimal SelectionRate,
    decimal CorrectRate,
    bool IsCorrectAnswer
);

/// <summary>
/// Question answer analytics model
/// </summary>
public record QuestionAnswerAnalyticsModel(
    int QuestionId,
    string QuestionText,
    int TotalAttempts,
    IEnumerable<AnswerStatsModel> AnswerStats,
    int MostSelectedAnswerId,
    decimal CorrectAnswerRate
);

#endregion

#region Request Models

/// <summary>
/// Request model for creating a new answer
/// </summary>
public record CreateAnswerRequest(
    [Required] int QuestionId,
    [Required] string AnswerText,
    bool IsCorrect,
    int OrderIndex = 0
);

/// <summary>
/// Request model for updating an existing answer
/// </summary>
public record UpdateAnswerRequest(
    string? AnswerText,
    bool? IsCorrect,
    int? OrderIndex
);

/// <summary>
/// Request model for filtering answers
/// </summary>
public record AnswerFilterRequest(
    int? QuestionId = null,
    bool? IsCorrect = null,
    string? SearchTerm = null
);

/// <summary>
/// Request model for reordering answers
/// </summary>
public record ReorderAnswersRequest(
    IEnumerable<AnswerOrderItem> AnswerOrders
);

/// <summary>
/// Request model for bulk answer operations
/// </summary>
public record BulkAnswerRequest(
    IEnumerable<int> AnswerIds,
    BulkAnswerOperation Operation
);

/// <summary>
/// Request model for creating multiple answers at once
/// </summary>
public record CreateMultipleAnswersRequest(
    int QuestionId,
    IEnumerable<CreateSingleAnswerRequest> Answers
);

/// <summary>
/// Single answer creation within bulk operation
/// </summary>
public record CreateSingleAnswerRequest(
    string AnswerText,
    bool IsCorrect,
    int OrderIndex
);

#endregion

#region Response Models

/// <summary>
/// Answer response model
/// </summary>
public record AnswerResponse(
    int Id,
    int QuestionId,
    string QuestionText,
    string AnswerText,
    bool IsCorrect,
    int OrderIndex,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Bulk operation response model
/// </summary>
public record BulkAnswerOperationResponse(
    bool Success,
    int ProcessedCount,
    int SuccessCount,
    int FailureCount,
    IEnumerable<string> Errors,
    string Message
);

/// <summary>
/// Answer validation details model
/// </summary>
public record AnswerValidationDetails(
    bool HasValidText,
    bool HasValidOrder,
    bool IsCorrectAnswerValid,
    bool PassesBusinessRules,
    IEnumerable<string> SpecificErrors
);

#endregion

#region Enums

/// <summary>
/// Question types for answer validation
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
/// Bulk answer operation types
/// </summary>
public enum BulkAnswerOperation
{
    Delete = 1,
    MarkCorrect = 2,
    MarkIncorrect = 3,
    Reorder = 4
}

/// <summary>
/// Answer sorting options
/// </summary>
public enum AnswerSortBy
{
    OrderIndex = 1,
    AnswerText = 2,
    IsCorrect = 3,
    CreatedAt = 4,
    SelectionRate = 5
}

#endregion