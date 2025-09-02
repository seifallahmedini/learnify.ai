namespace learnify.ai.api.Features.Assessments;

// Request DTOs for Quiz Attempt endpoints
public record SubmitQuizAttemptRequest(
    IEnumerable<QuizAnswerSubmission> Answers
);

public record QuizAnswerSubmission(
    int QuestionId,
    IEnumerable<int> SelectedAnswerIds
);

public record GetQuizAttemptRequest(
    bool IncludeAnswers = false
);

public record GetUserQuizAttemptsRequest(
    int? QuizId = null,
    bool? IsCompleted = null,
    int Page = 1,
    int PageSize = 10
);