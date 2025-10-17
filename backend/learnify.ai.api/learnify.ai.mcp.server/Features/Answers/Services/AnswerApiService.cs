using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Learnify.Mcp.Server.Shared.Services;
using Learnify.Mcp.Server.Features.Answers.Models;

namespace Learnify.Mcp.Server.Features.Answers.Services;

/// <summary>
/// API service for answer management operations
/// </summary>
[McpServerToolType]
public class AnswerApiService : BaseApiService
{
    public AnswerApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AnswerApiService> logger)
        : base(httpClient, configuration, logger, "AnswerApiService")
    {
    }

    #region Answer CRUD Operations

    /// <summary>
    /// Get all answers with optional filtering
    /// </summary>
    [McpServerTool, Description("Get all answers with optional filtering")]
    public async Task<string> GetAnswersAsync(
        [Description("Question ID filter (optional)")] int? questionId = null,
        [Description("Correct answer filter (optional)")] bool? isCorrect = null,
        [Description("Search term for answer text (optional)")] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting answers with filters - QuestionId: {QuestionId}, IsCorrect: {IsCorrect}", 
                questionId, isCorrect);
            
            var queryParams = new List<string>();
            if (questionId.HasValue) queryParams.Add($"questionId={questionId}");
            if (isCorrect.HasValue) queryParams.Add($"isCorrect={isCorrect}");
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var answers = await GetAsync<AnswerListResponse>($"/api/answers{queryString}", cancellationToken);

            if (answers == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "No answers found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = answers,
                message = "Answers retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting answers");
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get answer by ID with full details
    /// </summary>
    [McpServerTool, Description("Get answer details by ID")]
    public async Task<string> GetAnswerAsync(
        [Description("The answer ID")] int answerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting answer with ID: {AnswerId}", answerId);
            var answer = await GetAsync<AnswerResponse>($"/api/answers/{answerId}", cancellationToken);

            if (answer == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Answer with ID {answerId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = answer,
                message = "Answer retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting answer {AnswerId}", answerId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Create a new answer
    /// </summary>
    [McpServerTool, Description("Create a new answer for a quiz question")]
    public async Task<string> CreateAnswerAsync(
        [Description("Question ID")] int questionId,
        [Description("Answer text")] string answerText,
        [Description("Whether this is the correct answer")] bool isCorrect,
        [Description("Order index for answer display")] int orderIndex = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating answer for question {QuestionId}: {AnswerText}", questionId, answerText);

            var request = new CreateAnswerRequest(questionId, answerText, isCorrect, orderIndex);
            var createdAnswer = await PostAsync<AnswerResponse>("/api/answers", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = createdAnswer,
                message = "Answer created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating answer for question {QuestionId}", questionId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update an existing answer
    /// </summary>
    [McpServerTool, Description("Update answer details")]
    public async Task<string> UpdateAnswerAsync(
        [Description("The answer ID")] int answerId,
        [Description("Answer text (optional)")] string? answerText = null,
        [Description("Whether this is the correct answer (optional)")] bool? isCorrect = null,
        [Description("Order index for answer display (optional)")] int? orderIndex = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating answer with ID: {AnswerId}", answerId);

            var request = new UpdateAnswerRequest(answerText, isCorrect, orderIndex);
            var updatedAnswer = await PutAsync<AnswerResponse>($"/api/answers/{answerId}", request, cancellationToken);

            if (updatedAnswer == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Answer with ID {answerId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedAnswer,
                message = "Answer updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating answer {AnswerId}", answerId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete an answer permanently
    /// </summary>
    [McpServerTool, Description("Delete an answer permanently")]
    public async Task<string> DeleteAnswerAsync(
        [Description("The answer ID")] int answerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting answer with ID: {AnswerId}", answerId);
            var success = await DeleteAsync($"/api/answers/{answerId}", cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success,
                message = success ? "Answer deleted successfully" : "Failed to delete answer"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting answer {AnswerId}", answerId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Question Answer Management

    /// <summary>
    /// Get all answers for a specific question
    /// </summary>
    [McpServerTool, Description("Get all answers for a specific question")]
    public async Task<string> GetQuestionAnswersAsync(
        [Description("The question ID")] int questionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting answers for question ID: {QuestionId}", questionId);
            var questionAnswers = await GetAsync<QuestionAnswersResponse>($"/api/answers/question/{questionId}", cancellationToken);

            if (questionAnswers == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No answers found for question ID {questionId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = questionAnswers,
                message = "Question answers retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting answers for question {QuestionId}", questionId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Reorder answers for a question
    /// </summary>
    [McpServerTool, Description("Reorder answers for a specific question")]
    public async Task<string> ReorderQuestionAnswersAsync(
        [Description("The question ID")] int questionId,
        [Description("Comma-separated list of answer IDs in new order")] string answerIdsOrder,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Reordering answers for question {QuestionId}", questionId);

            // Parse the comma-separated answer IDs
            var answerIds = answerIdsOrder.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.Parse(id.Trim()))
                .ToList();

            // Create answer order items with sequential order indices
            var answerOrders = answerIds.Select((answerId, index) => 
                new AnswerOrderItem(answerId, index + 1)).ToList();

            var request = new ReorderAnswersRequest(answerOrders);
            var reorderResponse = await PutAsync<AnswerReorderResponse>($"/api/answers/question/{questionId}/reorder", request, cancellationToken);

            if (reorderResponse == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Failed to reorder answers for question ID {questionId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = reorderResponse,
                message = "Answers reordered successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering answers for question {QuestionId}", questionId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Create multiple answers for a question at once
    /// </summary>
    [McpServerTool, Description("Create multiple answers for a question at once")]
    public async Task<string> CreateMultipleAnswersAsync(
        [Description("The question ID")] int questionId,
        [Description("JSON array of answers with text, isCorrect, and orderIndex")] string answersJson,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating multiple answers for question {QuestionId}", questionId);

            // Parse the answers JSON
            var answersData = System.Text.Json.JsonSerializer.Deserialize<CreateSingleAnswerRequest[]>(answersJson);
            if (answersData == null || !answersData.Any())
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Invalid or empty answers data provided"
                });
            }

            var request = new CreateMultipleAnswersRequest(questionId, answersData);
            var result = await PostAsync<BulkAnswerOperationResponse>($"/api/answers/question/{questionId}/bulk", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = result?.Success ?? false,
                data = result,
                message = result?.Message ?? "Bulk answer creation completed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating multiple answers for question {QuestionId}", questionId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Answer Validation

    /// <summary>
    /// Validate answer business rules and constraints
    /// </summary>
    [McpServerTool, Description("Validate answer business rules and constraints")]
    public async Task<string> ValidateAnswerAsync(
        [Description("The answer ID")] int answerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating answer with ID: {AnswerId}", answerId);
            var validationResult = await GetAsync<AnswerValidationResponse>($"/api/answers/{answerId}/validate", cancellationToken);

            if (validationResult == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Answer with ID {answerId} not found for validation"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = validationResult,
                message = validationResult.IsValid ? "Answer validation passed" : "Answer validation failed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating answer {AnswerId}", answerId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Validate all answers for a question
    /// </summary>
    [McpServerTool, Description("Validate all answers for a specific question")]
    public async Task<string> ValidateQuestionAnswersAsync(
        [Description("The question ID")] int questionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating all answers for question {QuestionId}", questionId);
            
            // Get all answers for the question first
            var questionAnswers = await GetAsync<QuestionAnswersResponse>($"/api/answers/question/{questionId}", cancellationToken);
            if (questionAnswers == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No answers found for question ID {questionId}"
                });
            }

            // Validate each answer
            var validationResults = new List<AnswerValidationResponse>();
            foreach (var answer in questionAnswers.Answers)
            {
                var validation = await GetAsync<AnswerValidationResponse>($"/api/answers/{answer.Id}/validate", cancellationToken);
                if (validation != null)
                {
                    validationResults.Add(validation);
                }
            }

            var overallValid = validationResults.All(v => v.IsValid);
            var totalErrors = validationResults.SelectMany(v => v.ValidationErrors).ToList();

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = new
                {
                    questionId,
                    overallValid,
                    totalAnswers = questionAnswers.Answers.Count(),
                    validAnswers = validationResults.Count(v => v.IsValid),
                    invalidAnswers = validationResults.Count(v => !v.IsValid),
                    validationResults,
                    totalErrors
                },
                message = overallValid ? "All answers are valid" : $"Found {validationResults.Count(v => !v.IsValid)} invalid answers"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating answers for question {QuestionId}", questionId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Answer Analytics

    /// <summary>
    /// Get answer selection statistics
    /// </summary>
    [McpServerTool, Description("Get answer selection statistics and analytics")]
    public async Task<string> GetAnswerStatsAsync(
        [Description("The answer ID")] int answerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting statistics for answer ID: {AnswerId}", answerId);
            var stats = await GetAsync<AnswerStatsModel>($"/api/answers/{answerId}/stats", cancellationToken);

            if (stats == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No statistics found for answer ID {answerId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = stats,
                message = "Answer statistics retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for answer {AnswerId}", answerId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get comprehensive analytics for all answers of a question
    /// </summary>
    [McpServerTool, Description("Get comprehensive analytics for all answers of a question")]
    public async Task<string> GetQuestionAnswerAnalyticsAsync(
        [Description("The question ID")] int questionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting answer analytics for question ID: {QuestionId}", questionId);
            var analytics = await GetAsync<QuestionAnswerAnalyticsModel>($"/api/answers/question/{questionId}/analytics", cancellationToken);

            if (analytics == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No analytics found for question ID {questionId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = analytics,
                message = "Question answer analytics retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting answer analytics for question {QuestionId}", questionId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Perform bulk operations on multiple answers
    /// </summary>
    [McpServerTool, Description("Perform bulk operations on multiple answers")]
    public async Task<string> BulkAnswerOperationAsync(
        [Description("Comma-separated list of answer IDs")] string answerIds,
        [Description("Operation type (1=Delete, 2=MarkCorrect, 3=MarkIncorrect, 4=Reorder)")] int operation,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Performing bulk operation {Operation} on answers: {AnswerIds}", operation, answerIds);

            // Parse the comma-separated answer IDs
            var ids = answerIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.Parse(id.Trim()))
                .ToList();

            var request = new BulkAnswerRequest(ids, (BulkAnswerOperation)operation);
            var result = await PostAsync<BulkAnswerOperationResponse>("/api/answers/bulk", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = result?.Success ?? false,
                data = result,
                message = result?.Message ?? "Bulk operation completed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk operation on answers");
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
    /// Check if answer exists
    /// </summary>
    [McpServerTool, Description("Check if an answer exists")]
    public async Task<string> CheckAnswerExistsAsync(
        [Description("The answer ID to check")] int answerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking if answer exists: {AnswerId}", answerId);
            var answer = await GetAsync<AnswerResponse>($"/api/answers/{answerId}", cancellationToken);
            var exists = answer != null;

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                exists,
                message = exists ? "Answer exists" : "Answer does not exist"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking answer existence {AnswerId}", answerId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get answer summary (basic info only)
    /// </summary>
    [McpServerTool, Description("Get answer summary (basic information only)")]
    public async Task<string> GetAnswerSummaryAsync(
        [Description("The answer ID")] int answerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting answer summary for ID: {AnswerId}", answerId);
            var answer = await GetAsync<AnswerResponse>($"/api/answers/{answerId}", cancellationToken);

            if (answer == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Answer with ID {answerId} not found"
                });
            }

            var summary = new AnswerSummaryModel(
                answer.Id,
                answer.QuestionId,
                answer.AnswerText,
                answer.IsCorrect,
                answer.OrderIndex,
                answer.CreatedAt
            );

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = summary,
                message = "Answer summary retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting answer summary {AnswerId}", answerId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion
}