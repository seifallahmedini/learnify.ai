using learnify.ai.api.Domain.Enums;

namespace learnify.ai.api.Features.Assessments;

// Response DTOs for Quiz endpoints
public record QuizResponse(
    int Id,
    int CourseId,
    string CourseTitle,
    int? LessonId,
    string? LessonTitle,
    string Title,
    string Description,
    int? TimeLimit,
    int PassingScore,
    int MaxAttempts,
    bool IsActive,
    int QuestionCount,
    int TotalPoints,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record QuizSummaryResponse(
    int Id,
    int CourseId,
    int? LessonId,
    string Title,
    string Description,
    int? TimeLimit,
    int PassingScore,
    bool IsActive,
    int QuestionCount,
    DateTime CreatedAt
);

public record QuizListResponse(
    IEnumerable<QuizSummaryResponse> Quizzes,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public record CourseQuizzesResponse(
    int CourseId,
    string CourseTitle,
    IEnumerable<QuizSummaryResponse> Quizzes,
    int TotalCount,
    int ActiveQuizzes,
    int InactiveQuizzes
);

public record LessonQuizzesResponse(
    int LessonId,
    string LessonTitle,
    int CourseId,
    string CourseTitle,
    IEnumerable<QuizSummaryResponse> Quizzes,
    int TotalCount
);

public record QuizQuestionsResponse(
    int QuizId,
    string QuizTitle,
    IEnumerable<QuestionSummaryResponse> Questions,
    int TotalQuestions,
    int TotalPoints
);

public record QuestionSummaryResponse(
    int Id,
    int QuizId,
    string QuestionText,
    QuestionType QuestionType,
    int Points,
    int OrderIndex,
    bool IsActive,
    int AnswerCount
);

public record QuizAttemptResponse(
    int Id,
    int QuizId,
    string QuizTitle,
    int UserId,
    string UserName,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int? Score,
    int MaxScore,
    bool IsCompleted,
    int TimeSpentMinutes,
    int? TimeRemainingMinutes
);

public record QuizAttemptsResponse(
    int QuizId,
    string QuizTitle,
    IEnumerable<QuizAttemptResponse> Attempts,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    double AverageScore,
    int CompletedAttempts
);

public record StartQuizAttemptResponse(
    int AttemptId,
    int QuizId,
    string QuizTitle,
    int UserId,
    DateTime StartedAt,
    int? TimeLimit,
    DateTime? ExpiresAt,
    IEnumerable<QuizQuestionResponse> Questions
);

public record QuizQuestionResponse(
    int Id,
    string QuestionText,
    QuestionType QuestionType,
    int Points,
    int OrderIndex,
    IEnumerable<QuizAnswerResponse> Answers
);

public record QuizAnswerResponse(
    int Id,
    string AnswerText,
    int OrderIndex
    // Note: IsCorrect is not included for student responses
);
