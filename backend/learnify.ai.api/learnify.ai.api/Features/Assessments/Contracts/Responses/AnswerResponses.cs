namespace learnify.ai.api.Features.Assessments;

// Response DTOs for Answer endpoints
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

public record AnswerSummaryResponse(
    int Id,
    int QuestionId,
    string AnswerText,
    bool IsCorrect,
    int OrderIndex
);

public record AnswerListResponse(
    IEnumerable<AnswerSummaryResponse> Answers,
    int TotalCount,
    int QuestionId,
    string QuestionText
);

public record QuestionAnswersResponse(
    int QuestionId,
    string QuestionText,
    QuestionType QuestionType,
    IEnumerable<AnswerSummaryResponse> Answers,
    int TotalAnswers,
    int CorrectAnswersCount
);

public record AnswerValidationResponse(
    int AnswerId,
    bool IsValid,
    string Message,
    IEnumerable<string> ValidationErrors
);

public record AnswerReorderResponse(
    int QuestionId,
    IEnumerable<AnswerOrderResult> Results,
    bool Success,
    string Message
);

public record AnswerOrderResult(
    int AnswerId,
    int OldOrderIndex,
    int NewOrderIndex,
    bool Success
);