using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Learnify.Mcp.Server.Shared.Services;
using Learnify.Mcp.Server.Features.Courses.Models;
using System.Net;
using System.Text.Json;

namespace Learnify.Mcp.Server.Features.Courses.Services;

/// <summary>
/// API service for course management operations
/// </summary>
[McpServerToolType]
public class CourseApiService : BaseApiService
{
    public CourseApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<CourseApiService> logger)
        : base(httpClient, configuration, logger, "CourseApiService")
    {
    }

    #region Course CRUD Operations

    /// <summary>
    /// Get all courses with optional filtering
    /// </summary>
    [McpServerTool, Description("Get all courses with optional filtering and pagination")]
    public async Task<string> GetCoursesAsync(
        [Description("Category ID filter (optional)")] int? categoryId = null,
        [Description("Instructor ID filter (optional)")] int? instructorId = null,
        [Description("Course level filter (1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert)")] int? level = null,
        [Description("Published status filter (optional)")] bool? isPublished = null,
        [Description("Featured status filter (optional)")] bool? isFeatured = null,
        [Description("Minimum price filter (optional)")] decimal? minPrice = null,
        [Description("Maximum price filter (optional)")] decimal? maxPrice = null,
        [Description("Search term for title/description (optional)")] string? searchTerm = null,
        [Description("Page number (default: 1)")] int page = 1,
        [Description("Page size (default: 10)")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting courses with filters - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            
            var queryParams = new List<string>();
            if (categoryId.HasValue) queryParams.Add($"categoryId={categoryId}");
            if (instructorId.HasValue) queryParams.Add($"instructorId={instructorId}");
            if (level.HasValue) queryParams.Add($"level={level}");
            if (isPublished.HasValue) queryParams.Add($"isPublished={isPublished}");
            if (isFeatured.HasValue) queryParams.Add($"isFeatured={isFeatured}");
            if (minPrice.HasValue) queryParams.Add($"minPrice={minPrice}");
            if (maxPrice.HasValue) queryParams.Add($"maxPrice={maxPrice}");
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var courses = await GetAsync<CourseListResponse>($"/api/courses{queryString}", cancellationToken);

            if (courses == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "No courses found",
                    details = "The request was successful but no courses matched the specified criteria"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = courses,
                message = "Courses retrieved successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error getting courses");
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "Failed to retrieve courses",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting courses");
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "An unexpected error occurred while retrieving courses",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Get course by ID with full details
    /// </summary>
    [McpServerTool, Description("Get course details by ID")]
    public async Task<string> GetCourseAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting course with ID: {CourseId}", courseId);
            var course = await GetAsync<CourseModel>($"/api/courses/{courseId}", cancellationToken);

            if (course == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found",
                    details = "The specified course does not exist or has been deleted",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = course,
                message = "Course retrieved successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error getting course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to retrieve course {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while retrieving course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    [McpServerTool, Description("Create a new course")]
    public async Task<string> CreateCourseAsync(
        [Description("Course title")] string title,
        [Description("Course description")] string description,
        [Description("Instructor ID")] int instructorId,
        [Description("Category ID")] int categoryId,
        [Description("Course price")] decimal price,
        [Description("Course duration in hours")] int durationHours,
        [Description("Course level (1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert)")] int level,
        [Description("Course language")] string language,
        [Description("Short description (optional)")] string? shortDescription = null,
        [Description("Discount price (optional)")] decimal? discountPrice = null,
        [Description("Thumbnail URL (optional)")] string? thumbnailUrl = null,
        [Description("Video preview URL (optional)")] string? videoPreviewUrl = null,
        [Description("Whether the course is published")] bool isPublished = false,
        [Description("Whether the course is featured")] bool isFeatured = false,
        [Description("Maximum number of students (optional)")] int? maxStudents = null,
        [Description("Prerequisites (optional)")] string? prerequisites = null,
        [Description("Learning objectives (optional)")] string? learningObjectives = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(title))
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Course title is required",
                    details = "Title cannot be empty or whitespace",
                    errorType = "ValidationError"
                });
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Course description is required",
                    details = "Description cannot be empty or whitespace",
                    errorType = "ValidationError"
                });
            }

            if (price < 0)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Course price cannot be negative",
                    details = $"Provided price: {price}",
                    errorType = "ValidationError"
                });
            }

            if (level < 1 || level > 4)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Invalid course level",
                    details = "Level must be between 1 (Beginner) and 4 (Expert)",
                    errorType = "ValidationError"
                });
            }

            _logger.LogInformation("Creating course: {Title}", title);

            var request = new CreateCourseRequest(
                title, description, shortDescription, instructorId, categoryId,
                price, discountPrice, durationHours, (CourseLevel)level, language,
                thumbnailUrl, videoPreviewUrl, isPublished, isFeatured,
                maxStudents, prerequisites, learningObjectives);

            var createdCourse = await PostAsync<CourseModel>("/api/courses", request, cancellationToken);

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = createdCourse,
                message = "Course created successfully"
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("400"))
        {
            _logger.LogError(ex, "Validation error creating course {Title}", title);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "Course creation failed due to validation errors",
                details = ex.Message, // This will now contain the full API error message
                errorType = "ValidationError",
                originalError = ExtractApiErrorDetails(ex.Message)
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("404"))
        {
            _logger.LogError(ex, "Resource not found creating course {Title}", title);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "Course creation failed - instructor or category not found",
                details = ex.Message, // This will show the full API error
                errorType = "NotFound"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error creating course {Title}", title);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "Failed to create course",
                details = ex.Message, // This will contain the detailed API error message
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating course {Title}", title);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "An unexpected error occurred while creating the course",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Update an existing course
    /// </summary>
    [McpServerTool, Description("Update course details")]
    public async Task<string> UpdateCourseAsync(
        [Description("The course ID")] int courseId,
        [Description("Course title (optional)")] string? title = null,
        [Description("Course description (optional)")] string? description = null,
        [Description("Short description (optional)")] string? shortDescription = null,
        [Description("Category ID (optional)")] int? categoryId = null,
        [Description("Course price (optional)")] decimal? price = null,
        [Description("Discount price (optional)")] decimal? discountPrice = null,
        [Description("Course duration in hours (optional)")] int? durationHours = null,
        [Description("Course level (1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert) (optional)")] int? level = null,
        [Description("Course language (optional)")] string? language = null,
        [Description("Thumbnail URL (optional)")] string? thumbnailUrl = null,
        [Description("Video preview URL (optional)")] string? videoPreviewUrl = null,
        [Description("Whether the course is published (optional)")] bool? isPublished = null,
        [Description("Whether the course is featured (optional)")] bool? isFeatured = null,
        [Description("Maximum number of students (optional)")] int? maxStudents = null,
        [Description("Prerequisites (optional)")] string? prerequisites = null,
        [Description("Learning objectives (optional)")] string? learningObjectives = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input parameters
            if (price.HasValue && price < 0)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Course price cannot be negative",
                    details = $"Provided price: {price}",
                    errorType = "ValidationError"
                });
            }

            if (level.HasValue && (level < 1 || level > 4))
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Invalid course level",
                    details = "Level must be between 1 (Beginner) and 4 (Expert)",
                    errorType = "ValidationError"
                });
            }

            _logger.LogInformation("Updating course with ID: {CourseId}", courseId);

            var request = new UpdateCourseRequest(
                title, description, shortDescription, categoryId, price, discountPrice,
                durationHours, level.HasValue ? (CourseLevel)level.Value : null, language,
                thumbnailUrl, videoPreviewUrl, isPublished, isFeatured,
                maxStudents, prerequisites, learningObjectives);

            var updatedCourse = await PutAsync<CourseModel>($"/api/courses/{courseId}", request, cancellationToken);

            if (updatedCourse == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found",
                    details = "The specified course does not exist or has been deleted",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedCourse,
                message = "Course updated successfully"
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("400"))
        {
            _logger.LogError(ex, "Validation error updating course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "Course update failed due to validation errors",
                details = ex.Message, // This will now contain the full API error message
                errorType = "ValidationError"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error updating course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to update course {courseId}",
                details = ex.Message, // This will contain the detailed API error message
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while updating course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Delete a course permanently
    /// </summary>
    [McpServerTool, Description("Delete a course permanently")]
    public async Task<string> DeleteCourseAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting course with ID: {CourseId}", courseId);
            var success = await DeleteAsync($"/api/courses/{courseId}", cancellationToken);

            if (!success)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found",
                    details = "The specified course does not exist or has already been deleted",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                message = "Course deleted successfully"
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("400"))
        {
            _logger.LogError(ex, "Cannot delete course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "Course deletion not allowed",
                details = "Course cannot be deleted because it has active enrollments or dependencies",
                errorType = "BusinessRuleViolation"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error deleting course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to delete course {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while deleting course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    #endregion

    #region Course Publishing Operations

    /// <summary>
    /// Publish a course to make it visible to students
    /// </summary>
    [McpServerTool, Description("Publish a course to make it visible to students")]
    public async Task<string> PublishCourseAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Publishing course with ID: {CourseId}", courseId);
            var publishedCourse = await PutAsync<CourseModel>($"/api/courses/{courseId}/publish", null, cancellationToken);

            if (publishedCourse == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found",
                    details = "The specified course does not exist or has been deleted",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = publishedCourse,
                message = "Course published successfully"
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("400"))
        {
            _logger.LogError(ex, "Cannot publish course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = "Course cannot be published",
                details = "Course may be missing required content, lessons, or may already be published",
                errorType = "BusinessRuleViolation"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error publishing course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to publish course {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error publishing course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while publishing course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Unpublish a course to hide it from students
    /// </summary>
    [McpServerTool, Description("Unpublish a course to hide it from students")]
    public async Task<string> UnpublishCourseAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Unpublishing course with ID: {CourseId}", courseId);
            var unpublishedCourse = await PutAsync<CourseModel>($"/api/courses/{courseId}/unpublish", null, cancellationToken);

            if (unpublishedCourse == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found",
                    details = "The specified course does not exist or has been deleted",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = unpublishedCourse,
                message = "Course unpublished successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error unpublishing course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to unpublish course {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error unpublishing course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while unpublishing course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Feature a course
    /// </summary>
    [McpServerTool, Description("Feature a course to highlight it")]
    public async Task<string> FeatureCourseAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Featuring course with ID: {CourseId}", courseId);
            var featuredCourse = await PutAsync<CourseModel>($"/api/courses/{courseId}/feature", null, cancellationToken);

            if (featuredCourse == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found",
                    details = "The specified course does not exist or has been deleted",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = featuredCourse,
                message = "Course featured successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error featuring course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to feature course {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error featuring course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while featuring course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Unfeature a course
    /// </summary>
    [McpServerTool, Description("Unfeature a course to remove highlighting")]
    public async Task<string> UnfeatureCourseAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Unfeaturing course with ID: {CourseId}", courseId);
            var unfeaturedCourse = await PutAsync<CourseModel>($"/api/courses/{courseId}/unfeature", null, cancellationToken);

            if (unfeaturedCourse == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found",
                    details = "The specified course does not exist or has been deleted",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = unfeaturedCourse,
                message = "Course unfeatured successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error unfeaturing course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to unfeature course {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error unfeaturing course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while unfeaturing course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    #endregion

    #region Course Enrollment Operations

    /// <summary>
    /// Get course enrollments
    /// </summary>
    [McpServerTool, Description("Get all enrollments for a specific course")]
    public async Task<string> GetCourseEnrollmentsAsync(
        [Description("The course ID")] int courseId,
        [Description("Page number (default: 1)")] int page = 1,
        [Description("Page size (default: 10)")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting enrollments for course ID: {CourseId}", courseId);
            var enrollments = await GetAsync<CourseEnrollmentsResponse>($"/api/courses/{courseId}/enrollments?page={page}&pageSize={pageSize}", cancellationToken);

            if (enrollments == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No enrollments found for course ID {courseId}",
                    details = "The course may not exist or has no enrolled students",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = enrollments,
                message = "Course enrollments retrieved successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error getting enrollments for course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to retrieve enrollments for course {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting enrollments for course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while retrieving enrollments for course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Get course statistics
    /// </summary>
    [McpServerTool, Description("Get statistics for a specific course")]
    public async Task<string> GetCourseStatsAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting statistics for course ID: {CourseId}", courseId);
            var stats = await GetAsync<CourseStatsResponse>($"/api/courses/{courseId}/stats", cancellationToken);

            if (stats == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No statistics found for course ID {courseId}",
                    details = "The course may not exist or statistics are not available",
                    errorType = "NotFound"
                });
            }

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = stats,
                message = "Course statistics retrieved successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error getting statistics for course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to retrieve statistics for course {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting statistics for course {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while retrieving statistics for course {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Check if course exists
    /// </summary>
    [McpServerTool, Description("Check if a course exists")]
    public async Task<string> CheckCourseExistsAsync(
        [Description("The course ID to check")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking if course exists: {CourseId}", courseId);
            var course = await GetAsync<CourseModel>($"/api/courses/{courseId}", cancellationToken);
            var exists = course != null;

            return JsonSerializer.Serialize(new
            {
                success = true,
                exists,
                courseId,
                message = exists ? "Course exists" : "Course does not exist"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error checking course existence {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to check if course {courseId} exists",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error checking course existence {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while checking if course {courseId} exists",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    /// <summary>
    /// Get course summary (basic info only)
    /// </summary>
    [McpServerTool, Description("Get course summary (basic information only)")]
    public async Task<string> GetCourseSummaryAsync(
        [Description("The course ID")] int courseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting course summary for ID: {CourseId}", courseId);
            var course = await GetAsync<CourseModel>($"/api/courses/{courseId}", cancellationToken);

            if (course == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found",
                    details = "The specified course does not exist or has been deleted",
                    errorType = "NotFound"
                });
            }

            var summary = new CourseSummaryModel(
                course.Id,
                course.Title,
                course.ShortDescription,
                course.InstructorId,
                course.InstructorName,
                course.Price,
                course.DiscountPrice,
                course.Level,
                course.Language,
                course.IsPublished,
                course.IsFeatured,
                course.CreatedAt
            );

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = summary,
                message = "Course summary retrieved successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "API error getting course summary {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"Failed to retrieve course summary for {courseId}",
                details = ex.Message,
                errorType = "ApiError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting course summary {CourseId}", courseId);
            return JsonSerializer.Serialize(new
            {
                success = false,
                message = $"An unexpected error occurred while retrieving course summary for {courseId}",
                details = ex.Message,
                errorType = "UnknownError"
            });
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Extract detailed error information from API error messages
    /// </summary>
    private static string ExtractApiErrorDetails(string errorMessage)
    {
        try
        {
            // If the error message already contains structured information from BaseApiService, return it as-is
            if (errorMessage.Contains("API Error") || errorMessage.Contains("Validation Error"))
            {
                return errorMessage;
            }
            
            // Try to extract more specific error information from older format messages
            if (errorMessage.Contains("Response status code does not indicate success:"))
            {
                var statusIndex = errorMessage.IndexOf("Response status code does not indicate success:");
                if (statusIndex >= 0)
                {
                    var statusPart = errorMessage.Substring(statusIndex);
                    if (statusPart.Contains("400"))
                        return "Bad Request - Check input parameters for validation errors";
                    if (statusPart.Contains("401"))
                        return "Unauthorized - Authentication required";
                    if (statusPart.Contains("403"))
                        return "Forbidden - Insufficient permissions";
                    if (statusPart.Contains("404"))
                        return "Not Found - Resource does not exist";
                    if (statusPart.Contains("409"))
                        return "Conflict - Resource already exists or business rule violation";
                    if (statusPart.Contains("500"))
                        return "Internal Server Error - Server encountered an error";
                }
            }
            
            return errorMessage;
        }
        catch
        {
            return errorMessage;
        }
    }

    #endregion
}