using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Learnify.Mcp.Server.Shared.Services;
using Learnify.Mcp.Server.Features.Quizzes.Models;

namespace Learnify.Mcp.Server.Features.Quizzes.Services;

/// <summary>
/// API service for quiz management operations
/// </summary>
[McpServerToolType]
public class QuizApiService : BaseApiService
{
    public QuizApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<QuizApiService> logger)
        : base(httpClient, configuration, logger, "QuizApiService")
    {
    }

    #region Quiz CRUD Operations

    /// <summary>
    /// Get all quizzes with optional filtering
    /// </summary>
    [McpServerTool, Description("Get all quizzes with optional filtering and pagination")]
    public async Task<string> GetQuizzesAsync(
        [Description("Course ID filter (optional)")] int? courseId = null,
        [Description("Lesson ID filter (optional)")] int? lessonId = null,
        [Description("Active status filter (optional)")] bool? isActive = null,
        [Description("Search term for title/description (optional)")] string? searchTerm = null,
        [Description("Page number (default: 1)")] int page = 1,
        [Description("Page size (default: 10)")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting quizzes with filters - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            
            var queryParams = new List<string>();
            if (courseId.HasValue) queryParams.Add($"courseId={courseId}");
            if (lessonId.HasValue) queryParams.Add($"lessonId={lessonId}");
            if (isActive.HasValue) queryParams.Add($"isActive={isActive}");
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var quizzes = await GetAsync<QuizListResponse>($"/api/quizzes{queryString}", cancellationToken);

            if (quizzes == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "No quizzes found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = quizzes,
                message = "Quizzes retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quizzes");
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get quiz by ID with full details
    /// </summary>
    [McpServerTool, Description("Get quiz details by ID")]
    public async Task<string> GetQuizAsync(
        [Description("The quiz ID")] int quizId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting quiz with ID: {QuizId}", quizId);
            var quiz = await GetAsync<QuizModel>($"/api/quizzes/{quizId}", cancellationToken);

            if (quiz == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Quiz with ID {quizId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = quiz,
                message = "Quiz retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Create a new quiz
    /// </summary>
    [McpServerTool, Description("Create a new quiz")]
    public async Task<string> CreateQuizAsync(
        [Description("Quiz title")] string title,
        [Description("Quiz description")] string description,
        [Description("Passing score (0-100)")] decimal passingScore,
        [Description("Course ID (optional)")] int? courseId = null,
        [Description("Lesson ID (optional)")] int? lessonId = null,
        [Description("Time limit in minutes (optional)")] int? timeLimit = null,
        [Description("Maximum attempts allowed")] int maxAttempts = 3,
        [Description("Whether the quiz is active")] bool isActive = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating quiz: {Title}", title);

            var request = new CreateQuizRequest(
                courseId, lessonId, title, description, timeLimit, passingScore, maxAttempts, isActive);

            var createdQuiz = await PostAsync<QuizModel>("/api/quizzes", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = createdQuiz,
                message = "Quiz created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quiz {Title}", title);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update an existing quiz
    /// </summary>
    [McpServerTool, Description("Update quiz details")]
    public async Task<string> UpdateQuizAsync(
        [Description("The quiz ID")] int quizId,
        [Description("Quiz title (optional)")] string? title = null,
        [Description("Quiz description (optional)")] string? description = null,
        [Description("Time limit in minutes (optional)")] int? timeLimit = null,
        [Description("Passing score (0-100) (optional)")] decimal? passingScore = null,
        [Description("Maximum attempts allowed (optional)")] int? maxAttempts = null,
        [Description("Whether the quiz is active (optional)")] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating quiz with ID: {QuizId}", quizId);

            var request = new UpdateQuizRequest(
                title, description, timeLimit, passingScore, maxAttempts, isActive);

            var updatedQuiz = await PutAsync<QuizModel>($"/api/quizzes/{quizId}", request, cancellationToken);

            if (updatedQuiz == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Quiz with ID {quizId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedQuiz,
                message = "Quiz updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete a quiz permanently
    /// </summary>
    [McpServerTool, Description("Delete a quiz permanently")]
    public async Task<string> DeleteQuizAsync(
        [Description("The quiz ID")] int quizId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting quiz with ID: {QuizId}", quizId);
            var success = await DeleteAsync($"/api/quizzes/{quizId}", cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success,
                message = success ? "Quiz deleted successfully" : "Failed to delete quiz"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Quiz Activation Operations

    /// <summary>
    /// Activate a quiz
    /// </summary>
    [McpServerTool, Description("Activate a quiz to make it available to students")]
    public async Task<string> ActivateQuizAsync(
        [Description("The quiz ID")] int quizId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Activating quiz with ID: {QuizId}", quizId);
            var activatedQuiz = await PutAsync<QuizModel>($"/api/quizzes/{quizId}/activate", null, cancellationToken);

            if (activatedQuiz == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Quiz with ID {quizId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = activatedQuiz,
                message = "Quiz activated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Deactivate a quiz
    /// </summary>
    [McpServerTool, Description("Deactivate a quiz to make it unavailable to students")]
    public async Task<string> DeactivateQuizAsync(
        [Description("The quiz ID")] int quizId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deactivating quiz with ID: {QuizId}", quizId);
            var deactivatedQuiz = await PutAsync<QuizModel>($"/api/quizzes/{quizId}/deactivate", null, cancellationToken);

            if (deactivatedQuiz == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Quiz with ID {quizId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = deactivatedQuiz,
                message = "Quiz deactivated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Course and Lesson Integration

    /// <summary>
    /// Get all quizzes for a specific course
    /// </summary>
    [McpServerTool, Description("Get all quizzes for a specific course")]
    public async Task<string> GetCourseQuizzesAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting quizzes for course ID: {CourseId}", courseId);
            var courseQuizzes = await GetAsync<CourseQuizzesResponse>($"/api/quizzes/courses/{courseId}/quizzes", cancellationToken);

            if (courseQuizzes == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No quizzes found for course ID {courseId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = courseQuizzes,
                message = "Course quizzes retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quizzes for course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all quizzes for a specific lesson
    /// </summary>
    [McpServerTool, Description("Get all quizzes for a specific lesson")]
    public async Task<string> GetLessonQuizzesAsync(
        [Description("The lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting quizzes for lesson ID: {LessonId}", lessonId);
            var lessonQuizzes = await GetAsync<QuizListResponse>($"/api/quizzes/lessons/{lessonId}/quizzes", cancellationToken);

            if (lessonQuizzes == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No quizzes found for lesson ID {lessonId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = lessonQuizzes,
                message = "Lesson quizzes retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quizzes for lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Question Management

    /// <summary>
    /// Get all questions for a specific quiz
    /// </summary>
    [McpServerTool, Description("Get all questions for a specific quiz")]
    public async Task<string> GetQuizQuestionsAsync(
        [Description("The quiz ID")] int quizId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting questions for quiz ID: {QuizId}", quizId);
            var quizQuestions = await GetAsync<QuizQuestionsResponse>($"/api/quizzes/{quizId}/questions", cancellationToken);

            if (quizQuestions == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No questions found for quiz ID {quizId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = quizQuestions,
                message = "Quiz questions retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions for quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Add a question to a quiz
    /// </summary>
    [McpServerTool, Description("Add a new question to a quiz")]
    public async Task<string> AddQuestionToQuizAsync(
        [Description("The quiz ID")] int quizId,
        [Description("The question text")] string questionText,
        [Description("Question type (1=MultipleChoice, 2=TrueFalse, 3=ShortAnswer, 4=Essay, 5=FillInTheBlank)")] int questionType,
        [Description("Points for this question")] int points = 1,
        [Description("Order index (optional)")] int? orderIndex = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding question to quiz {QuizId}: {QuestionText}", quizId, questionText);

            var request = new AddQuestionToQuizRequest(
                questionText, (QuestionType)questionType, points, orderIndex);

            var addedQuestion = await PostAsync<QuestionSummaryModel>($"/api/quizzes/{quizId}/questions", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = addedQuestion,
                message = "Question added to quiz successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding question to quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Quiz Attempts Management

    /// <summary>
    /// Start a quiz attempt for a user
    /// </summary>
    [McpServerTool, Description("Start a quiz attempt for a specific user")]
    public async Task<string> StartQuizAttemptAsync(
        [Description("The quiz ID")] int quizId,
        [Description("The user ID")] int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting quiz attempt for quiz {QuizId} by user {UserId}", quizId, userId);

            var request = new StartQuizAttemptRequest(userId);
            var attemptResponse = await PostAsync<StartQuizAttemptResponse>($"/api/quizzes/{quizId}/start", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = attemptResponse,
                message = "Quiz attempt started successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting quiz attempt for quiz {QuizId} by user {UserId}", quizId, userId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get all attempts for a specific quiz
    /// </summary>
    [McpServerTool, Description("Get all attempts for a specific quiz")]
    public async Task<string> GetQuizAttemptsAsync(
        [Description("The quiz ID")] int quizId,
        [Description("Page number (default: 1)")] int page = 1,
        [Description("Page size (default: 10)")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting attempts for quiz ID: {QuizId}", quizId);
            
            var queryString = $"?page={page}&pageSize={pageSize}";
            var quizAttempts = await GetAsync<QuizAttemptsResponse>($"/api/quizzes/{quizId}/attempts{queryString}", cancellationToken);

            if (quizAttempts == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No attempts found for quiz ID {quizId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = quizAttempts,
                message = "Quiz attempts retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attempts for quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get quiz statistics
    /// </summary>
    [McpServerTool, Description("Get comprehensive statistics for a quiz")]
    public async Task<string> GetQuizStatsAsync(
        [Description("The quiz ID")] int quizId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting statistics for quiz ID: {QuizId}", quizId);
            var stats = await GetAsync<QuizStatsModel>($"/api/quizzes/{quizId}/stats", cancellationToken);

            if (stats == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No statistics found for quiz ID {quizId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = stats,
                message = "Quiz statistics retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for quiz {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Check if quiz exists
    /// </summary>
    [McpServerTool, Description("Check if a quiz exists")]
    public async Task<string> CheckQuizExistsAsync(
        [Description("The quiz ID to check")] int quizId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking if quiz exists: {QuizId}", quizId);
            var quiz = await GetAsync<QuizModel>($"/api/quizzes/{quizId}", cancellationToken);
            var exists = quiz != null;

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                exists,
                message = exists ? "Quiz exists" : "Quiz does not exist"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking quiz existence {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get quiz summary (basic info only)
    /// </summary>
    [McpServerTool, Description("Get quiz summary (basic information only)")]
    public async Task<string> GetQuizSummaryAsync(
        [Description("The quiz ID")] int quizId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting quiz summary for ID: {QuizId}", quizId);
            var quiz = await GetAsync<QuizModel>($"/api/quizzes/{quizId}", cancellationToken);

            if (quiz == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Quiz with ID {quizId} not found"
                });
            }

            var summary = new QuizSummaryModel(
                quiz.Id,
                quiz.CourseId,
                quiz.CourseTitle,
                quiz.LessonId,
                quiz.LessonTitle,
                quiz.Title,
                quiz.Description,
                quiz.PassingScore,
                quiz.MaxAttempts,
                quiz.IsActive,
                quiz.QuestionCount,
                quiz.CreatedAt
            );

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = summary,
                message = "Quiz summary retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quiz summary {QuizId}", quizId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion
}