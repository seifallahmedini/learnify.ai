using learnify.ai.api.Domain.Enums;

namespace learnify.ai.api.Features.Assessments;

// Response DTOs for Quiz Attempt endpoints
public record QuizAttemptDetailResponse(
    int Id,
    int QuizId,
    string QuizTitle,
    int UserId,
    string UserName,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int? Score,
    int MaxScore,
    int ScorePercentage,
    bool IsCompleted,
    bool IsPassed,
    int TimeSpentMinutes,
    string FormattedTimeSpent,
    int? TimeRemainingMinutes,
    IEnumerable<QuizAttemptAnswerResponse>? Answers = null
);

public record QuizAttemptAnswerResponse(
    int QuestionId,
    string QuestionText,
    QuestionType QuestionType,
    int Points,
    IEnumerable<int> SelectedAnswerIds,
    IEnumerable<QuizAttemptAnswerOptionResponse> AnswerOptions,
    bool IsCorrect,
    int PointsEarned
);

public record QuizAttemptAnswerOptionResponse(
    int Id,
    string AnswerText,
    bool IsSelected,
    bool IsCorrect
);

public record SubmitQuizAttemptResponse(
    int AttemptId,
    int QuizId,
    string QuizTitle,
    int UserId,
    string UserName,
    DateTime StartedAt,
    DateTime CompletedAt,
    int Score,
    int MaxScore,
    int ScorePercentage,
    bool IsPassed,
    int TimeSpentMinutes,
    string FormattedTimeSpent,
    IEnumerable<QuizAttemptAnswerResponse> Answers
);

public record UserQuizAttemptsResponse(
    int UserId,
    string UserName,
    IEnumerable<UserQuizAttemptSummaryResponse> Attempts,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    QuizAttemptStatsResponse Stats
);

public record UserQuizAttemptSummaryResponse(
    int Id,
    int QuizId,
    string QuizTitle,
    int CourseId,
    string CourseTitle,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int? Score,
    int MaxScore,
    int? ScorePercentage,
    bool IsCompleted,
    bool IsPassed,
    int TimeSpentMinutes,
    string FormattedTimeSpent
);

public record QuizAttemptStatsResponse(
    int TotalAttempts,
    int CompletedAttempts,
    int PassedAttempts,
    int FailedAttempts,
    double AverageScore,
    int? BestScore,
    int TotalTimeSpentMinutes,
    string FormattedTotalTimeSpent
);
