using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Learnify.Mcp.Server.Shared.Services;
using Learnify.Mcp.Server.Features.Courses.Models;

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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "No courses found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = courses,
                message = "Courses retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses");
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = course,
                message = "Course retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
            _logger.LogInformation("Creating course: {Title}", title);

            var request = new CreateCourseRequest(
                title, description, shortDescription, instructorId, categoryId,
                price, discountPrice, durationHours, (CourseLevel)level, language,
                thumbnailUrl, videoPreviewUrl, isPublished, isFeatured,
                maxStudents, prerequisites, learningObjectives);

            var createdCourse = await PostAsync<CourseModel>("/api/courses", request, cancellationToken);

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = createdCourse,
                message = "Course created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course {Title}", title);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
            _logger.LogInformation("Updating course with ID: {CourseId}", courseId);

            var request = new UpdateCourseRequest(
                title, description, shortDescription, categoryId, price, discountPrice,
                durationHours, level.HasValue ? (CourseLevel)level.Value : null, language,
                thumbnailUrl, videoPreviewUrl, isPublished, isFeatured,
                maxStudents, prerequisites, learningObjectives);

            var updatedCourse = await PutAsync<CourseModel>($"/api/courses/{courseId}", request, cancellationToken);

            if (updatedCourse == null)
            {
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = updatedCourse,
                message = "Course updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success,
                message = success ? "Course deleted successfully" : "Failed to delete course"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = publishedCourse,
                message = "Course published successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = unpublishedCourse,
                message = "Course unpublished successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = featuredCourse,
                message = "Course featured successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error featuring course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = unfeaturedCourse,
                message = "Course unfeatured successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfeaturing course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No enrollments found for course ID {courseId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = enrollments,
                message = "Course enrollments retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enrollments for course {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"No statistics found for course ID {courseId}"
                });
            }

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = stats,
                message = "Course statistics retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics for course {CourseId}", courseId);
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

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                exists,
                message = exists ? "Course exists" : "Course does not exist"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking course existence {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
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
                return System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"Course with ID {courseId} not found"
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

            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = true,
                data = summary,
                message = "Course summary retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course summary {CourseId}", courseId);
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion
}