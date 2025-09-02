namespace learnify.ai.api.Features.Assessments;

// Request DTOs for Answer endpoints
public record CreateAnswerRequest(
    int QuestionId,
    string AnswerText,
    bool IsCorrect = false,
    int? OrderIndex = null
);

public record UpdateAnswerRequest(
    string? AnswerText = null,
    bool? IsCorrect = null,
    int? OrderIndex = null
);

public record GetAnswersRequest(
    int? QuestionId = null,
    bool? IsCorrect = null
);

public record ReorderAnswersRequest(
    IEnumerable<AnswerOrderItem> AnswerOrders
);

public record AnswerOrderItem(
    int AnswerId,
    int OrderIndex
);