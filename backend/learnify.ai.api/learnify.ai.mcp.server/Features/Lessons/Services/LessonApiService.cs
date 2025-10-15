using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Learnify.Mcp.Server.Shared.Services;
using Learnify.Mcp.Server.Features.Lessons.Models;

namespace Learnify.Mcp.Server.Features.Lessons.Services;

/// <summary>
/// API service for lesson management operations
/// </summary>
[McpServerToolType]
public class LessonApiService : BaseApiService
{
    public LessonApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<LessonApiService> logger)
        : base(httpClient, configuration, logger, "LessonApiService")
    {
    }

    #region Lesson CRUD Operations

    /// <summary>
    /// Get lesson by ID with full details
    /// </summary>
    [McpServerTool, Description("Get lesson details by ID")]
    public async Task<string> GetLessonAsync(
        [Description("The lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting lesson with ID: {LessonId}", lessonId);
            var lesson = await GetAsync<LessonModel>($"/api/lessons/{lessonId}", cancellationToken);

            if (lesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = lesson,
                message = "Lesson retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update lesson details
    /// </summary>
    [McpServerTool, Description("Update lesson details")]
    public async Task<string> UpdateLessonAsync(
        [Description("The lesson ID")] int lessonId,
        [Description("The lesson title")] string? title = null,
        [Description("The lesson description")] string? description = null,
        [Description("The lesson content")] string? content = null,
        [Description("The lesson video URL")] string? videoUrl = null,
        [Description("The lesson duration in minutes")] int? duration = null,
        [Description("The lesson order index")] int? orderIndex = null,
        [Description("Whether the lesson is free")] bool? isFree = null,
        [Description("Whether the lesson is published")] bool? isPublished = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating lesson with ID: {LessonId}", lessonId);

            var request = new UpdateLessonRequest(
                title, description, content, videoUrl, duration, orderIndex, isFree, isPublished);

            var updatedLesson = await PutAsync<LessonModel>($"/api/lessons/{lessonId}", request, cancellationToken);

            if (updatedLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedLesson,
                message = "Lesson updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Delete lesson permanently
    /// </summary>
    [McpServerTool, Description("Delete a lesson permanently")]
    public async Task<string> DeleteLessonAsync(
        [Description("The lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting lesson with ID: {LessonId}", lessonId);
            var success = await DeleteAsync($"/api/lessons/{lessonId}", cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success,
                message = success ? "Lesson deleted successfully" : "Failed to delete lesson"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Lesson Organization

    /// <summary>
    /// Reorder lesson within a course
    /// </summary>
    [McpServerTool, Description("Reorder a lesson within its course")]
    public async Task<string> ReorderLessonAsync(
        [Description("The lesson ID to reorder")] int lessonId,
        [Description("The new order index position")] int newOrderIndex,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Reordering lesson {LessonId} to position {NewOrder}", lessonId, newOrderIndex);

            var request = new ReorderLessonRequest(newOrderIndex);
            var reorderedLesson = await PutAsync<LessonModel>($"/api/lessons/{lessonId}/reorder", request, cancellationToken);

            if (reorderedLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = reorderedLesson,
                message = "Lesson reordered successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get next lesson in the course sequence
    /// </summary>
    [McpServerTool, Description("Get the next lesson in the course sequence")]
    public async Task<string> GetNextLessonAsync(
        [Description("The current lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting next lesson for lesson ID: {LessonId}", lessonId);
            var nextLesson = await GetAsync<LessonModel>($"/api/lessons/{lessonId}/next", cancellationToken);

            if (nextLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No next lesson found for lesson ID {lessonId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = nextLesson,
                message = "Next lesson retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next lesson for {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get previous lesson in the course sequence
    /// </summary>
    [McpServerTool, Description("Get the previous lesson in the course sequence")]
    public async Task<string> GetPreviousLessonAsync(
        [Description("The current lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting previous lesson for lesson ID: {LessonId}", lessonId);
            var previousLesson = await GetAsync<LessonModel>($"/api/lessons/{lessonId}/previous", cancellationToken);

            if (previousLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No previous lesson found for lesson ID {lessonId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = previousLesson,
                message = "Previous lesson retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting previous lesson for {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Lesson Content

    /// <summary>
    /// Upload or update lesson video URL
    /// </summary>
    [McpServerTool, Description("Upload or update lesson video")]
    public async Task<string> UploadLessonVideoAsync(
        [Description("The lesson ID")] int lessonId,
        [Description("The video URL")] string videoUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Uploading video for lesson ID: {LessonId}", lessonId);

            var request = new UploadVideoRequest(videoUrl);
            var updatedLesson = await PostAsync<LessonModel>($"/api/lessons/{lessonId}/video", request, cancellationToken);

            if (updatedLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedLesson,
                message = "Video uploaded successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading video for lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Update lesson content
    /// </summary>
    [McpServerTool, Description("Update lesson content")]
    public async Task<string> UpdateLessonContentAsync(
        [Description("The lesson ID")] int lessonId,
        [Description("The new lesson content")] string content,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating content for lesson ID: {LessonId}", lessonId);

            var request = new UpdateContentRequest(content);
            var updatedLesson = await PutAsync<LessonModel>($"/api/lessons/{lessonId}/content", request, cancellationToken);

            if (updatedLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedLesson,
                message = "Content updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating content for lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get lesson resources and attachments
    /// </summary>
    [McpServerTool, Description("Get lesson resources and attachments")]
    public async Task<string> GetLessonResourcesAsync(
        [Description("The lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting resources for lesson ID: {LessonId}", lessonId);
            var resources = await GetAsync<LessonResourcesModel>($"/api/lessons/{lessonId}/resources", cancellationToken);

            if (resources == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No resources found for lesson ID {lessonId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = resources,
                message = "Resources retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resources for lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Lesson Access Control

    /// <summary>
    /// Publish lesson to make it visible to students
    /// </summary>
    [McpServerTool, Description("Publish a lesson to make it visible to students")]
    public async Task<string> PublishLessonAsync(
        [Description("The lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Publishing lesson with ID: {LessonId}", lessonId);
            var publishedLesson = await PutAsync<LessonModel>($"/api/lessons/{lessonId}/publish", null, cancellationToken);

            if (publishedLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = publishedLesson,
                message = "Lesson published successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Unpublish lesson to hide it from students
    /// </summary>
    [McpServerTool, Description("Unpublish a lesson to hide it from students")]
    public async Task<string> UnpublishLessonAsync(
        [Description("The lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Unpublishing lesson with ID: {LessonId}", lessonId);
            var unpublishedLesson = await PutAsync<LessonModel>($"/api/lessons/{lessonId}/unpublish", null, cancellationToken);

            if (unpublishedLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = unpublishedLesson,
                message = "Lesson unpublished successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Make lesson free or premium
    /// </summary>
    [McpServerTool, Description("Make a lesson free or premium")]
    public async Task<string> MakeLessonFreeAsync(
        [Description("The lesson ID")] int lessonId,
        [Description("Whether the lesson should be free")] bool isFree = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Setting lesson {LessonId} free status to: {IsFree}", lessonId, isFree);
            var updatedLesson = await PutAsync<LessonModel>($"/api/lessons/{lessonId}/free?isFree={isFree}", null, cancellationToken);

            if (updatedLesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            var action = isFree ? "free" : "premium";
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedLesson,
                message = $"Lesson made {action} successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting free status for lesson {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Course Lessons

    /// <summary>
    /// Get course lessons with optional publishing filter
    /// </summary>
    [McpServerTool, Description("Get all lessons for a specific course")]
    public async Task<string> GetCourseLessonsAsync(
        [Description("The course ID")] int courseId,
        [Description("Filter by published status (optional)")] bool? isPublished = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting lessons for course ID: {CourseId}, Published filter: {IsPublished}", courseId, isPublished);

            var queryString = isPublished.HasValue ? $"?isPublished={isPublished.Value}" : "";
            var courseLessons = await GetAsync<CourseLessonsModel>($"/api/courses/{courseId}/lessons{queryString}", cancellationToken);

            if (courseLessons == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No lessons found for course ID {courseId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = courseLessons,
                message = "Course lessons retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lessons for course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Add lesson to course
    /// </summary>
    [McpServerTool, Description("Create a new lesson in a course")]
    public async Task<string> CreateCourseLessonAsync(
        [Description("The course ID")] int courseId,
        [Description("The lesson title")] string title,
        [Description("The lesson description")] string description,
        [Description("The lesson content")] string content,
        [Description("The lesson duration in minutes")] int duration,
        [Description("The lesson video URL (optional)")] string? videoUrl = null,
        [Description("Whether the lesson is free")] bool isFree = false,
        [Description("Whether the lesson is published")] bool isPublished = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating lesson for course ID: {CourseId}", courseId);

            var request = new CreateLessonRequest(
                title, description, content, videoUrl, duration, isFree, isPublished);

            var createdLesson = await PostAsync<LessonModel>($"/api/courses/{courseId}/lessons", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = createdLesson,
                message = "Lesson created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lesson for course {CourseId}", courseId);
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
    /// Check if lesson exists
    /// </summary>
    [McpServerTool, Description("Check if a lesson exists")]
    public async Task<string> CheckLessonExistsAsync(
        [Description("The lesson ID to check")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking if lesson exists: {LessonId}", lessonId);
            var lesson = await GetAsync<LessonModel>($"/api/lessons/{lessonId}", cancellationToken);
            var exists = lesson != null;

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                exists,
                message = exists ? "Lesson exists" : "Lesson does not exist"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking lesson existence {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Get lesson summary (basic info only)
    /// </summary>
    [McpServerTool, Description("Get lesson summary (basic information only)")]
    public async Task<string> GetLessonSummaryAsync(
        [Description("The lesson ID")] int lessonId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting lesson summary for ID: {LessonId}", lessonId);
            var lesson = await GetAsync<LessonModel>($"/api/lessons/{lessonId}", cancellationToken);

            if (lesson == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Lesson with ID {lessonId} not found"
                });
            }

            var summary = new LessonSummaryModel(
                lesson.Id,
                lesson.CourseId,
                lesson.Title,
                lesson.Description,
                lesson.Duration,
                lesson.OrderIndex,
                lesson.IsFree,
                lesson.IsPublished,
                lesson.CreatedAt
            );

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = summary,
                message = "Lesson summary retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lesson summary {LessonId}", lessonId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion
}