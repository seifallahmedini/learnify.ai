using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Controllers;
using learnify.ai.api.Common.Models;

namespace learnify.ai.api.Features.Assessments;

[ApiController]
[Route("api/[controller]")]
public class QuizzesController : BaseController
{
    #region Quiz CRUD Operations

    /// <summary>
    /// Get all quizzes with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<QuizListResponse>>> GetQuizzes(
        [FromQuery] int? courseId = null,
        [FromQuery] int? lessonId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetQuizzesQuery(courseId, lessonId, isActive, page, pageSize);
        var result = await Mediator.Send(query);
        return Ok(result, "Quizzes retrieved successfully");
    }

    /// <summary>
    /// Get quiz by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<QuizResponse>>> GetQuiz(int id)
    {
        var query = new GetQuizByIdQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<QuizResponse>($"Quiz with ID {id} not found");

        return Ok(result, "Quiz retrieved successfully");
    }

    /// <summary>
    /// Create new quiz
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<QuizResponse>>> CreateQuiz([FromBody] CreateQuizRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<QuizResponse>("Validation failed", errors);
        }

        var command = new CreateQuizCommand(
            request.CourseId,
            request.LessonId,
            request.Title,
            request.Description,
            request.TimeLimit,
            request.PassingScore,
            request.MaxAttempts,
            request.IsActive
        );

        try
        {
            var result = await Mediator.Send(command);
            return Ok(result, "Quiz created successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<QuizResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<QuizResponse>($"Failed to create quiz: {ex.Message}");
        }
    }

    /// <summary>
    /// Update quiz
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<QuizResponse>>> UpdateQuiz(int id, [FromBody] UpdateQuizRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<QuizResponse>("Validation failed", errors);
        }

        try
        {
            var command = new UpdateQuizCommand(
                id,
                request.Title,
                request.Description,
                request.TimeLimit,
                request.PassingScore,
                request.MaxAttempts,
                request.IsActive
            );

            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<QuizResponse>($"Quiz with ID {id} not found");

            return Ok(result, "Quiz updated successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<QuizResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<QuizResponse>($"Failed to update quiz: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete quiz
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteQuiz(int id)
    {
        var command = new DeleteQuizCommand(id);

        try
        {
            var result = await Mediator.Send(command);

            if (!result)
                return NotFound<bool>($"Quiz with ID {id} not found");

            return Ok(result, "Quiz deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<bool>(ex.Message);
        }
    }

    #endregion

    #region Quiz Organization

    /// <summary>
    /// Get course quizzes - matches /api/courses/{courseId}/quizzes from features overview
    /// </summary>
    [HttpGet("courses/{courseId:int}/quizzes")]
    public async Task<ActionResult<ApiResponse<CourseQuizzesResponse>>> GetCourseQuizzes(int courseId)
    {
        var query = new GetCourseQuizzesQuery(courseId);

        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Course quizzes retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<CourseQuizzesResponse>($"Course with ID {courseId} not found");
        }
    }

    /// <summary>
    /// Get lesson quizzes - matches /api/lessons/{lessonId}/quizzes from features overview
    /// </summary>
    [HttpGet("lessons/{lessonId:int}/quizzes")]
    public async Task<ActionResult<ApiResponse<QuizListResponse>>> GetLessonQuizzes(int lessonId)
    {
        var query = new GetQuizzesQuery(LessonId: lessonId);
        var result = await Mediator.Send(query);
        return Ok(result, "Lesson quizzes retrieved successfully");
    }

    /// <summary>
    /// Activate quiz
    /// </summary>
    [HttpPut("{id:int}/activate")]
    public async Task<ActionResult<ApiResponse<QuizResponse>>> ActivateQuiz(int id)
    {
        var command = new ActivateQuizCommand(id);

        try
        {
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<QuizResponse>($"Quiz with ID {id} not found");

            return Ok(result, "Quiz activated successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<QuizResponse>(ex.Message);
        }
    }

    /// <summary>
    /// Deactivate quiz
    /// </summary>
    [HttpPut("{id:int}/deactivate")]
    public async Task<ActionResult<ApiResponse<QuizResponse>>> DeactivateQuiz(int id)
    {
        var command = new DeactivateQuizCommand(id);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<QuizResponse>($"Quiz with ID {id} not found");

        return Ok(result, "Quiz deactivated successfully");
    }

    #endregion

    #region Question Management

    /// <summary>
    /// Get quiz questions - matches /api/quizzes/{quizId}/questions from features overview
    /// </summary>
    [HttpGet("{quizId:int}/questions")]
    public async Task<ActionResult<ApiResponse<QuizQuestionsResponse>>> GetQuizQuestions(int quizId)
    {
        var query = new GetQuizQuestionsQuery(quizId);

        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Quiz questions retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<QuizQuestionsResponse>($"Quiz with ID {quizId} not found");
        }
    }

    /// <summary>
    /// Add question to quiz - matches /api/quizzes/{quizId}/questions from features overview
    /// </summary>
    [HttpPost("{quizId:int}/questions")]
    public async Task<ActionResult<ApiResponse<QuestionSummaryResponse>>> AddQuestionToQuiz(int quizId, [FromBody] AddQuestionToQuizRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<QuestionSummaryResponse>("Validation failed", errors);
        }

        var command = new AddQuestionToQuizCommand(
            quizId,
            request.QuestionText,
            request.QuestionType,
            request.Points,
            request.OrderIndex
        );

        try
        {
            var result = await Mediator.Send(command);
            return Ok(result, "Question added to quiz successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound<QuestionSummaryResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<QuestionSummaryResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<QuestionSummaryResponse>($"Failed to add question to quiz: {ex.Message}");
        }
    }

    #endregion

    #region Quiz Attempts

    /// <summary>
    /// Start quiz attempt - matches /api/quizzes/{id}/start from features overview
    /// </summary>
    [HttpPost("{id:int}/start")]
    public async Task<ActionResult<ApiResponse<StartQuizAttemptResponse>>> StartQuizAttempt(int id, [FromBody] StartQuizAttemptRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<StartQuizAttemptResponse>("Validation failed", errors);
        }

        var command = new StartQuizAttemptCommand(id, request.UserId);

        try
        {
            var result = await Mediator.Send(command);
            return Ok(result, "Quiz attempt started successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound<StartQuizAttemptResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<StartQuizAttemptResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<StartQuizAttemptResponse>($"Failed to start quiz attempt: {ex.Message}");
        }
    }

    /// <summary>
    /// Get all quiz attempts - matches /api/quizzes/{id}/attempts from features overview
    /// </summary>
    [HttpGet("{id:int}/attempts")]
    public async Task<ActionResult<ApiResponse<QuizAttemptsResponse>>> GetQuizAttempts(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetQuizAttemptsQuery(id, page, pageSize);

        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Quiz attempts retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<QuizAttemptsResponse>($"Quiz with ID {id} not found");
        }
    }

    #endregion
}