namespace learnify.ai.api.Features.Reviews;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : BaseController
{
    #region Review Management

    /// <summary>
    /// Get all reviews
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ReviewListResponse>>> GetReviews(
        [FromQuery] int? courseId = null, 
        [FromQuery] int? userId = null, 
        [FromQuery] bool? isApproved = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetReviewsQuery(courseId, userId, isApproved, page, pageSize);
            var result = await Mediator.Send(query);
            return Ok(result, "Reviews retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<ReviewListResponse>("An error occurred while retrieving reviews");
        }
    }

    /// <summary>
    /// Get review by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> GetReview(int id)
    {
        try
        {
            var query = new GetReviewByIdQuery(id);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<ReviewResponse>($"Review with ID {id} not found");

            return Ok(result, "Review retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<ReviewResponse>("An error occurred while retrieving the review");
        }
    }

    /// <summary>
    /// Create new review
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> CreateReview([FromBody] CreateReviewRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<ReviewResponse>("Validation failed", errors);
        }

        try
        {
            var command = new CreateReviewCommand(
                request.CourseId,
                request.UserId,
                request.Rating,
                request.Comment
            );

            var result = await Mediator.Send(command);
            return Ok(result, "Review created successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<ReviewResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<ReviewResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<ReviewResponse>($"Failed to create review: {ex.Message}");
        }
    }

    /// <summary>
    /// Update review
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ReviewResponse>>> UpdateReview(int id, [FromBody] UpdateReviewRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<ReviewResponse>("Validation failed", errors);
        }

        try
        {
            var command = new UpdateReviewCommand(id, request.Rating, request.Comment);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<ReviewResponse>($"Review with ID {id} not found");

            return Ok(result, "Review updated successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<ReviewResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<ReviewResponse>($"Failed to update review: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete review
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteReview(int id)
    {
        try
        {
            var command = new DeleteReviewCommand(id);
            var result = await Mediator.Send(command);

            if (!result)
                return NotFound<bool>($"Review with ID {id} not found");

            return Ok(result, "Review deleted successfully");
        }
        catch (Exception)
        {
            return BadRequest<bool>("An error occurred while deleting the review");
        }
    }

    #endregion

    #region Course Reviews

    /// <summary>
    /// Get course reviews
    /// </summary>
    [HttpGet("course/{courseId:int}")]
    public async Task<ActionResult<ApiResponse<CourseReviewsResponse>>> GetCourseReviews(
        int courseId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] int? rating = null,
        [FromQuery] bool? isApproved = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetCourseReviewsQuery(courseId, page, pageSize, rating, isApproved, fromDate, toDate);
            var result = await Mediator.Send(query);
            return Ok(result, "Course reviews retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<CourseReviewsResponse>(ex.Message);
        }
    }

    /// <summary>
    /// Get course average rating
    /// </summary>
    [HttpGet("course/{courseId:int}/rating")]
    public async Task<ActionResult<ApiResponse<CourseRatingResponse>>> GetCourseRating(int courseId)
    {
        try
        {
            var query = new GetCourseRatingQuery(courseId);
            var result = await Mediator.Send(query);
            return Ok(result, "Course rating retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<CourseRatingResponse>(ex.Message);
        }
    }

    /// <summary>
    /// Get course review statistics
    /// </summary>
    [HttpGet("course/{courseId:int}/stats")]
    public async Task<ActionResult<ApiResponse<CourseReviewStatsResponse>>> GetCourseReviewStats(int courseId)
    {
        try
        {
            var query = new GetCourseReviewStatsQuery(courseId);
            var result = await Mediator.Send(query);
            return Ok(result, "Course review statistics retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<CourseReviewStatsResponse>(ex.Message);
        }
    }

    #endregion

    #region User Reviews

    /// <summary>
    /// Get user reviews
    /// </summary>
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<ApiResponse<UserReviewsResponse>>> GetUserReviews(
        int userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isApproved = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetUserReviewsQuery(userId, page, pageSize, isApproved, fromDate, toDate);
            var result = await Mediator.Send(query);
            return Ok(result, "User reviews retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<UserReviewsResponse>(ex.Message);
        }
    }

    /// <summary>
    /// Get reviews given by user (alias for GetUserReviews for backward compatibility)
    /// </summary>
    [HttpGet("user/{userId:int}/given")]
    public async Task<ActionResult<ApiResponse<UserReviewsResponse>>> GetReviewsGivenByUser(
        int userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isApproved = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetUserReviewsQuery(userId, page, pageSize, isApproved, fromDate, toDate);
            var result = await Mediator.Send(query);
            return Ok(result, "Reviews given by user retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<UserReviewsResponse>(ex.Message);
        }
    }

    #endregion

    #region Review Moderation

    /// <summary>
    /// Approve review
    /// </summary>
    [HttpPut("{id:int}/approve")]
    public async Task<ActionResult<ApiResponse<ReviewModerationResponse>>> ApproveReview(int id)
    {
        try
        {
            var command = new ApproveReviewCommand(id);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<ReviewModerationResponse>($"Review with ID {id} not found");

            return Ok(result, "Review approved successfully");
        }
        catch (Exception)
        {
            return BadRequest<ReviewModerationResponse>("An error occurred while approving the review");
        }
    }

    /// <summary>
    /// Reject review
    /// </summary>
    [HttpPut("{id:int}/reject")]
    public async Task<ActionResult<ApiResponse<ReviewModerationResponse>>> RejectReview(int id)
    {
        try
        {
            var command = new RejectReviewCommand(id);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<ReviewModerationResponse>($"Review with ID {id} not found");

            return Ok(result, "Review rejected successfully");
        }
        catch (Exception)
        {
            return BadRequest<ReviewModerationResponse>("An error occurred while rejecting the review");
        }
    }

    /// <summary>
    /// Get pending reviews
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<ApiResponse<PendingReviewsResponse>>> GetPendingReviews(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var query = new GetPendingReviewsQuery(page, pageSize, fromDate, toDate);
            var result = await Mediator.Send(query);
            return Ok(result, "Pending reviews retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<PendingReviewsResponse>("An error occurred while retrieving pending reviews");
        }
    }

    #endregion
}
