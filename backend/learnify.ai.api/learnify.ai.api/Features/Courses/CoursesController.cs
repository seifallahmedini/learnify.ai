using learnify.ai.api.Features.Lessons;

namespace learnify.ai.api.Features.Courses;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : BaseController
{
    #region Course CRUD Operations

    /// <summary>
    /// Get all courses with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetCourses([FromQuery] CourseFilterRequest request)
    {
        var query = new GetCoursesQuery(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            request.IsPublished,
            request.IsFeatured,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Courses retrieved successfully");
    }

    /// <summary>
    /// Get a specific course by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetCourse(int id)
    {
        var query = new GetCourseByIdQuery(id);
        var result = await Mediator.Send(query);
        
        if (result == null)
            return NotFound<CourseResponse>($"Course with ID {id} not found");
            
        return Ok(result, "Course retrieved successfully");
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> CreateCourse([FromBody] CreateCourseRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<CourseResponse>("Validation failed", errors);
        }

        try
        {
            var command = new CreateCourseCommand(
                request.Title,
                request.Description,
                request.ShortDescription,
                request.InstructorId,
                request.CategoryId,
                request.Price,
                request.DiscountPrice,
                request.DurationHours,
                request.Level,
                request.Language,
                request.ThumbnailUrl,
                request.VideoPreviewUrl,
                request.IsPublished,
                request.IsFeatured,
                request.MaxStudents,
                request.Prerequisites,
                request.LearningObjectives
            );

            var result = await Mediator.Send(command);
            return Ok(result, "Course created successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<CourseResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<CourseResponse>($"Failed to create course: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing course
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> UpdateCourse(int id, [FromBody] UpdateCourseRequest request)
    {
        var command = new UpdateCourseCommand(
            id,
            request.Title,
            request.Description,
            request.ShortDescription,
            request.CategoryId,
            request.Price,
            request.DiscountPrice,
            request.DurationHours,
            request.Level,
            request.Language,
            request.ThumbnailUrl,
            request.VideoPreviewUrl,
            request.IsPublished,
            request.IsFeatured,
            request.MaxStudents,
            request.Prerequisites,
            request.LearningObjectives
        );

        var result = await Mediator.Send(command);
        
        if (result == null)
            return NotFound<CourseResponse>($"Course with ID {id} not found");

        return Ok(result, "Course updated successfully");
    }

    /// <summary>
    /// Delete a course
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCourse(int id)
    {
        var command = new DeleteCourseCommand(id);
        var result = await Mediator.Send(command);

        if (!result)
            return NotFound<bool>($"Course with ID {id} not found");

        return Ok(result, "Course deleted successfully");
    }

    #endregion

    #region Course Discovery

    /// <summary>
    /// Get published courses only
    /// </summary>
    [HttpGet("published")]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetPublishedCourses([FromQuery] CourseFilterRequest request)
    {
        var query = new GetCoursesQuery(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            true, // IsPublished = true
            request.IsFeatured,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Published courses retrieved successfully");
    }

    /// <summary>
    /// Search courses by title or description
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> SearchCourses([FromQuery] string searchTerm, [FromQuery] CourseFilterRequest request)
    {
        var query = new GetCoursesQuery(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            request.IsPublished,
            request.IsFeatured,
            request.MinPrice,
            request.MaxPrice,
            searchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, $"Search results for '{searchTerm}' retrieved successfully");
    }

    /// <summary>
    /// Get featured courses based on ratings, enrollment count, and manual featuring
    /// </summary>
    [HttpGet("featured")]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetFeaturedCourses([FromQuery] CourseFilterRequest request)
    {
        var query = new GetFeaturedCoursesQuery(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Featured courses retrieved successfully");
    }

    /// <summary>
    /// Get popular courses based on enrollment count
    /// </summary>
    [HttpGet("popular")]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetPopularCourses([FromQuery] CourseFilterRequest request)
    {
        var query = new GetPopularCoursesQuery(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Popular courses retrieved successfully");
    }

    /// <summary>
    /// Get recently added courses ordered by creation date
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetRecentCourses([FromQuery] CourseFilterRequest request)
    {
        var query = new GetRecentCoursesQuery(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Recent courses retrieved successfully");
    }

    #endregion

    #region Course Organization

    /// <summary>
    /// Get courses by category
    /// </summary>
    [HttpGet("category/{categoryId:int}")]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetCoursesByCategory(int categoryId, [FromQuery] CourseFilterRequest request)
    {
        var query = new GetCoursesQuery(
            categoryId,
            request.InstructorId,
            request.Level,
            request.IsPublished,
            request.IsFeatured,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, $"Courses for category {categoryId} retrieved successfully");
    }

    /// <summary>
    /// Get courses by instructor
    /// </summary>
    [HttpGet("instructor/{instructorId:int}")]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetCoursesByInstructor(int instructorId, [FromQuery] CourseFilterRequest request)
    {
        var query = new GetCoursesQuery(
            request.CategoryId,
            instructorId,
            request.Level,
            request.IsPublished,
            request.IsFeatured,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, $"Courses for instructor {instructorId} retrieved successfully");
    }

    /// <summary>
    /// Get courses by difficulty level
    /// </summary>
    [HttpGet("level/{level}")]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetCoursesByLevel(CourseLevel level, [FromQuery] CourseFilterRequest request)
    {
        var query = new GetCoursesQuery(
            request.CategoryId,
            request.InstructorId,
            level,
            request.IsPublished,
            request.IsFeatured,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, $"Courses for level {level} retrieved successfully");
    }

    #endregion

    #region Course Management

    /// <summary>
    /// Publish a course
    /// </summary>
    [HttpPut("{id:int}/publish")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> PublishCourse(int id)
    {
        var command = new PublishCourseCommand(id);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<CourseResponse>($"Course with ID {id} not found");

        return Ok(result, "Course published successfully");
    }

    /// <summary>
    /// Unpublish a course
    /// </summary>
    [HttpPut("{id:int}/unpublish")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> UnpublishCourse(int id)
    {
        var command = new UnpublishCourseCommand(id);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<CourseResponse>($"Course with ID {id} not found");

        return Ok(result, "Course unpublished successfully");
    }

    /// <summary>
    /// Feature or unfeature a course
    /// </summary>
    [HttpPut("{id:int}/feature")]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> FeatureCourse(int id, [FromQuery] bool featured = true)
    {
        var command = new FeatureCourseCommand(id, featured);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<CourseResponse>($"Course with ID {id} not found");

        var message = featured ? "Course featured successfully" : "Course unfeatured successfully";
        return Ok(result, message);
    }

    /// <summary>
    /// Get comprehensive course analytics including enrollment stats, completion rates, and revenue
    /// </summary>
    [HttpGet("{id:int}/analytics")]
    public async Task<ActionResult<ApiResponse<CourseAnalyticsResponse>>> GetCourseAnalytics(int id)
    {
        var query = new GetCourseAnalyticsQuery(id);
        
        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Course analytics retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<CourseAnalyticsResponse>($"Course with ID {id} not found");
        }
    }

    /// <summary>
    /// Get enrolled students for a course with pagination
    /// </summary>
    [HttpGet("{id:int}/students")]
    public async Task<ActionResult<ApiResponse<CourseStudentsResponse>>> GetCourseStudents(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetCourseStudentsQuery(id, page, pageSize);
        
        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Course students retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<CourseStudentsResponse>($"Course with ID {id} not found");
        }
    }

    /// <summary>
    /// Get detailed course completion rate and progress statistics
    /// </summary>
    [HttpGet("{id:int}/completion-rate")]
    public async Task<ActionResult<ApiResponse<CourseCompletionRateResponse>>> GetCourseCompletionRate(int id)
    {
        var query = new GetCourseCompletionRateQuery(id);
        
        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Course completion rate retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<CourseCompletionRateResponse>($"Course with ID {id} not found");
        }
    }

    #endregion

    #region Course Lessons

    /// <summary>
    /// Get course lessons with optional publishing filter
    /// </summary>
    [HttpGet("{courseId:int}/lessons")]
    public async Task<ActionResult<ApiResponse<CourseLessonsResponse>>> GetCourseLessons(int courseId, [FromQuery] bool? isPublished = null)
    {
        var query = new GetCourseLessonsQuery(courseId, isPublished);
        
        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Course lessons retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<CourseLessonsResponse>($"Course with ID {courseId} not found");
        }
    }

    /// <summary>
    /// Add lesson to course
    /// </summary>
    [HttpPost("{courseId:int}/lessons")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> CreateCourseLesson(int courseId, [FromBody] CreateLessonRequest request)
    {
        var command = new CreateLessonCommand(
            courseId,
            request.Title,
            request.Description,
            request.Content,
            request.VideoUrl,
            request.Duration,
            request.IsFree,
            request.IsPublished
        );

        try
        {
            var result = await Mediator.Send(command);
            return Ok(result, "Lesson created successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound<LessonResponse>(ex.Message);
        }
    }

    #endregion
}
