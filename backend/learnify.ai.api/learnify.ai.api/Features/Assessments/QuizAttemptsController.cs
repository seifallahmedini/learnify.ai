using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Controllers;
using learnify.ai.api.Common.Models;

namespace learnify.ai.api.Features.Assessments;

[ApiController]
[Route("api/quiz-attempts")]
public class QuizAttemptsController : BaseController
{
    #region Quiz Attempt Management

    /// <summary>
    /// Submit quiz attempt - matches /api/quiz-attempts/{id}/submit from features overview
    /// </summary>
    [HttpPost("{id:int}/submit")]
    public async Task<ActionResult<ApiResponse<SubmitQuizAttemptResponse>>> SubmitQuizAttempt(int id, [FromBody] SubmitQuizAttemptRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<SubmitQuizAttemptResponse>("Validation failed", errors);
        }

        var command = new SubmitQuizAttemptCommand(id, request.Answers);
        
        try
        {
            var result = await Mediator.Send(command);
            return Ok(result, "Quiz attempt submitted successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound<SubmitQuizAttemptResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<SubmitQuizAttemptResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<SubmitQuizAttemptResponse>($"Failed to submit quiz attempt: {ex.Message}");
        }
    }

    /// <summary>
    /// Get attempt details - matches /api/quiz-attempts/{id} from features overview
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<QuizAttemptDetailResponse>>> GetQuizAttempt(
        int id, 
        [FromQuery] bool includeAnswers = false)
    {
        var query = new GetQuizAttemptByIdQuery(id, includeAnswers);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<QuizAttemptDetailResponse>($"Quiz attempt with ID {id} not found");

        return Ok(result, "Quiz attempt retrieved successfully");
    }

    #endregion

    #region User Quiz Attempts

    /// <summary>
    /// Get user quiz attempts - matches /api/users/{userId}/quiz-attempts from features overview
    /// </summary>
    [HttpGet("users/{userId:int}/quiz-attempts")]
    public async Task<ActionResult<ApiResponse<UserQuizAttemptsResponse>>> GetUserQuizAttempts(
        int userId,
        [FromQuery] int? quizId = null,
        [FromQuery] bool? isCompleted = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUserQuizAttemptsQuery(userId, quizId, isCompleted, page, pageSize);
        
        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "User quiz attempts retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<UserQuizAttemptsResponse>($"User with ID {userId} not found");
        }
    }

    /// <summary>
    /// Get user's best attempt for a specific quiz
    /// </summary>
    [HttpGet("users/{userId:int}/quiz/{quizId:int}/best")]
    public async Task<ActionResult<ApiResponse<QuizAttemptDetailResponse>>> GetUserBestAttempt(int userId, int quizId)
    {
        var query = new GetUserBestAttemptQuery(userId, quizId);
        
        try
        {
            var result = await Mediator.Send(query);
            
            if (result == null)
                return NotFound<QuizAttemptDetailResponse>($"No attempts found for user {userId} on quiz {quizId}");

            return Ok(result, "Best attempt retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound<QuizAttemptDetailResponse>(ex.Message);
        }
    }

    /// <summary>
    /// Get user's latest attempt for a specific quiz
    /// </summary>
    [HttpGet("users/{userId:int}/quiz/{quizId:int}/latest")]
    public async Task<ActionResult<ApiResponse<QuizAttemptDetailResponse>>> GetUserLatestAttempt(int userId, int quizId)
    {
        var query = new GetUserLatestAttemptQuery(userId, quizId);
        
        try
        {
            var result = await Mediator.Send(query);
            
            if (result == null)
                return NotFound<QuizAttemptDetailResponse>($"No attempts found for user {userId} on quiz {quizId}");

            return Ok(result, "Latest attempt retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound<QuizAttemptDetailResponse>(ex.Message);
        }
    }

    #endregion

    #region Quiz Attempt Analytics

    /// <summary>
    /// Get quiz attempt statistics for a user
    /// </summary>
    [HttpGet("users/{userId:int}/stats")]
    public async Task<ActionResult<ApiResponse<QuizAttemptStatsResponse>>> GetUserQuizStats(int userId)
    {
        var query = new GetUserQuizStatsQuery(userId);
        
        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "User quiz statistics retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<QuizAttemptStatsResponse>($"User with ID {userId} not found");
        }
    }

    /// <summary>
    /// Get passed attempts for a user
    /// </summary>
    [HttpGet("users/{userId:int}/passed")]
    public async Task<ActionResult<ApiResponse<UserQuizAttemptsResponse>>> GetUserPassedAttempts(
        int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUserQuizAttemptsQuery(userId, null, true, page, pageSize);
        
        try
        {
            var result = await Mediator.Send(query);
            // Filter only passed attempts
            var passedOnly = new UserQuizAttemptsResponse(
                result.UserId,
                result.UserName,
                result.Attempts.Where(a => a.IsPassed),
                result.Attempts.Count(a => a.IsPassed),
                result.Page,
                result.PageSize,
                (int)Math.Ceiling((double)result.Attempts.Count(a => a.IsPassed) / result.PageSize),
                result.Stats
            );
            
            return Ok(passedOnly, "User passed attempts retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<UserQuizAttemptsResponse>($"User with ID {userId} not found");
        }
    }

    /// <summary>
    /// Get failed attempts for a user
    /// </summary>
    [HttpGet("users/{userId:int}/failed")]
    public async Task<ActionResult<ApiResponse<UserQuizAttemptsResponse>>> GetUserFailedAttempts(
        int userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUserQuizAttemptsQuery(userId, null, true, page, pageSize);
        
        try
        {
            var result = await Mediator.Send(query);
            // Filter only failed attempts
            var failedOnly = new UserQuizAttemptsResponse(
                result.UserId,
                result.UserName,
                result.Attempts.Where(a => a.IsCompleted && !a.IsPassed),
                result.Attempts.Count(a => a.IsCompleted && !a.IsPassed),
                result.Page,
                result.PageSize,
                (int)Math.Ceiling((double)result.Attempts.Count(a => a.IsCompleted && !a.IsPassed) / result.PageSize),
                result.Stats
            );
            
            return Ok(failedOnly, "User failed attempts retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<UserQuizAttemptsResponse>($"User with ID {userId} not found");
        }
    }

    #endregion
}