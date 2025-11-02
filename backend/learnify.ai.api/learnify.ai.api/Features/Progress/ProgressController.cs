using learnify.ai.api.Features.Enrollments;

namespace learnify.ai.api.Features.Progress;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : BaseController
{
    #region Progress Management

    /// <summary>
    /// Get all progress for enrollment
    /// </summary>
    [HttpGet("enrollment/{enrollmentId:int}")]
    public async Task<ActionResult<ApiResponse<EnrollmentProgressResponse>>> GetEnrollmentProgress(int enrollmentId)
    {
        try
        {
            var query = new GetEnrollmentProgressQuery(enrollmentId);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<EnrollmentProgressResponse>($"Enrollment with ID {enrollmentId} not found");

            return Ok(result, "Enrollment progress retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<EnrollmentProgressResponse>("An error occurred while retrieving enrollment progress");
        }
    }

    /// <summary>
    /// Get lesson progress
    /// </summary>
    [HttpGet("lesson/{lessonId:int}")]
    public async Task<ActionResult<ApiResponse<DetailedLessonProgressResponse>>> GetLessonProgress(int lessonId, [FromQuery] int? enrollmentId = null)
    {
        try
        {
            var query = new GetLessonProgressQuery(lessonId, enrollmentId);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<DetailedLessonProgressResponse>($"Lesson with ID {lessonId} not found");

            return Ok(result, "Lesson progress retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<DetailedLessonProgressResponse>(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest<DetailedLessonProgressResponse>("An error occurred while retrieving lesson progress");
        }
    }

    /// <summary>
    /// Mark lesson as complete
    /// </summary>
    [HttpPut("lesson/{lessonId:int}/complete")]
    public async Task<ActionResult<ApiResponse<DetailedLessonProgressResponse>>> MarkLessonComplete(int lessonId, [FromBody] MarkLessonCompleteRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<DetailedLessonProgressResponse>("Validation failed", errors);
        }

        try
        {
            var command = new MarkLessonCompleteCommand(lessonId, request.EnrollmentId);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<DetailedLessonProgressResponse>($"Lesson with ID {lessonId} not found");

            return Ok(result, "Lesson marked as complete successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<DetailedLessonProgressResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<DetailedLessonProgressResponse>($"Failed to mark lesson as complete: {ex.Message}");
        }
    }

    /// <summary>
    /// Track time spent on lesson
    /// </summary>
    [HttpPost("lesson/{lessonId:int}/time")]
    public async Task<ActionResult<ApiResponse<DetailedLessonProgressResponse>>> TrackTimeSpent(int lessonId, [FromBody] TrackTimeSpentRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<DetailedLessonProgressResponse>("Validation failed", errors);
        }

        try
        {
            var command = new TrackTimeSpentCommand(lessonId, request.EnrollmentId, request.TimeSpentMinutes);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<DetailedLessonProgressResponse>($"Lesson with ID {lessonId} not found");

            return Ok(result, "Time tracking updated successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<DetailedLessonProgressResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<DetailedLessonProgressResponse>($"Failed to track time spent: {ex.Message}");
        }
    }

    #endregion

    #region Progress Analytics

    /// <summary>
    /// Get user learning statistics
    /// </summary>
    [HttpGet("user/{userId:int}/stats")]
    public async Task<ActionResult<ApiResponse<UserLearningStatsResponse>>> GetUserLearningStats(
        int userId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var query = new GetUserLearningStatsQuery(userId, startDate, endDate);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<UserLearningStatsResponse>($"User with ID {userId} not found");

            return Ok(result, "User learning statistics retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<UserLearningStatsResponse>("An error occurred while retrieving user learning statistics");
        }
    }

    /// <summary>
    /// Get course progress statistics
    /// </summary>
    [HttpGet("course/{courseId:int}/stats")]
    public async Task<ActionResult<ApiResponse<CourseProgressStatsResponse>>> GetCourseProgressStats(
        int courseId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var query = new GetCourseProgressStatsQuery(courseId, startDate, endDate);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<CourseProgressStatsResponse>($"Course with ID {courseId} not found");

            return Ok(result, "Course progress statistics retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<CourseProgressStatsResponse>("An error occurred while retrieving course progress statistics");
        }
    }

    /// <summary>
    /// Get progress dashboard for user
    /// </summary>
    [HttpGet("dashboard/{userId:int}")]
    public async Task<ActionResult<ApiResponse<ProgressDashboardResponse>>> GetProgressDashboard(
        int userId,
        [FromQuery] int? limit = 10)
    {
        try
        {
            var query = new GetProgressDashboardQuery(userId, limit);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<ProgressDashboardResponse>($"User with ID {userId} not found");

            return Ok(result, "Progress dashboard retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<ProgressDashboardResponse>("An error occurred while retrieving progress dashboard");
        }
    }

    #endregion
}
