using learnify.ai.api.Domain.Enums;

namespace learnify.ai.api.Features.Assessments;

// Request DTOs for Quiz endpoints - aligned with features-overview.md
public record CreateQuizRequest(
    int CourseId,
    int? LessonId,
    string Title,
    string Description,
    int? TimeLimit = null,
    int PassingScore = 70,
    int MaxAttempts = 3,
    bool IsActive = true
);

public record UpdateQuizRequest(
    string? Title = null,
    string? Description = null,
    int? TimeLimit = null,
    int? PassingScore = null,
    int? MaxAttempts = null,
    bool? IsActive = null
);

public record GetQuizzesRequest(
    int? CourseId = null,
    int? LessonId = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10
);

public record StartQuizAttemptRequest(
    int UserId
);

public record AddQuestionToQuizRequest(
    string QuestionText,
    QuestionType QuestionType,
    int Points = 1,
    int? OrderIndex = null
);
