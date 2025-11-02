namespace learnify.ai.api.Features.Assessments;

[ApiController]
[Route("api/[controller]")]
public class AnswersController : BaseController
{
    #region Answer Management

    /// <summary>
    /// Get all answers or answers for a specific question
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<AnswerListResponse>>> GetAnswers(
        [FromQuery] int? questionId = null,
        [FromQuery] bool? isCorrect = null)
    {
        try
        {
            var query = new GetAnswersQuery(questionId, isCorrect);
            var result = await Mediator.Send(query);
            return Ok(result, "Answers retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<AnswerListResponse>(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest<AnswerListResponse>("An error occurred while retrieving answers");
        }
    }

    /// <summary>
    /// Get answer by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AnswerResponse>>> GetAnswer(int id)
    {
        try
        {
            var query = new GetAnswerByIdQuery(id);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<AnswerResponse>($"Answer with ID {id} not found");

            return Ok(result, "Answer retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<AnswerResponse>("An error occurred while retrieving the answer");
        }
    }

    /// <summary>
    /// Create new answer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AnswerResponse>>> CreateAnswer([FromBody] CreateAnswerRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<AnswerResponse>("Validation failed", errors);
        }

        try
        {
            var command = new CreateAnswerCommand(
                request.QuestionId,
                request.AnswerText,
                request.IsCorrect,
                request.OrderIndex
            );

            var result = await Mediator.Send(command);
            return Ok(result, "Answer created successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<AnswerResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<AnswerResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<AnswerResponse>($"Failed to create answer: {ex.Message}");
        }
    }

    /// <summary>
    /// Update answer
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<AnswerResponse>>> UpdateAnswer(int id, [FromBody] UpdateAnswerRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<AnswerResponse>("Validation failed", errors);
        }

        try
        {
            var command = new UpdateAnswerCommand(id, request.AnswerText, request.IsCorrect, request.OrderIndex);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<AnswerResponse>($"Answer with ID {id} not found");

            return Ok(result, "Answer updated successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<AnswerResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<AnswerResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<AnswerResponse>($"Failed to update answer: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete answer
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAnswer(int id)
    {
        try
        {
            var command = new DeleteAnswerCommand(id);
            var result = await Mediator.Send(command);

            if (!result)
                return NotFound<bool>($"Answer with ID {id} not found");

            return Ok(result, "Answer deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<bool>(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest<bool>("An error occurred while deleting the answer");
        }
    }

    #endregion

    #region Question Answers

    /// <summary>
    /// Get all answers for a specific question
    /// </summary>
    [HttpGet("question/{questionId:int}")]
    public async Task<ActionResult<ApiResponse<QuestionAnswersResponse>>> GetQuestionAnswers(int questionId)
    {
        try
        {
            var query = new GetQuestionAnswersQuery(questionId);
            var result = await Mediator.Send(query);
            return Ok(result, "Question answers retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<QuestionAnswersResponse>(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest<QuestionAnswersResponse>("An error occurred while retrieving question answers");
        }
    }

    /// <summary>
    /// Reorder answers for a question
    /// </summary>
    [HttpPut("question/{questionId:int}/reorder")]
    public async Task<ActionResult<ApiResponse<AnswerReorderResponse>>> ReorderAnswers(int questionId, [FromBody] ReorderAnswersRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<AnswerReorderResponse>("Validation failed", errors);
        }

        try
        {
            var command = new ReorderAnswersCommand(questionId, request.AnswerOrders);
            var result = await Mediator.Send(command);
            return Ok(result, result.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest<AnswerReorderResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<AnswerReorderResponse>($"Failed to reorder answers: {ex.Message}");
        }
    }

    #endregion

    #region Bulk Answer Operations

    /// <summary>
    /// Create multiple answers for a question at once
    /// </summary>
    [HttpPost("question/{questionId:int}/bulk")]
    public async Task<ActionResult<ApiResponse<BulkAnswerOperationResponse>>> CreateMultipleAnswers(int questionId, [FromBody] CreateMultipleAnswersRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<BulkAnswerOperationResponse>("Validation failed", errors);
        }

        try
        {
            // Ensure the questionId from route matches the request
            var command = new CreateMultipleAnswersCommand(questionId, request.Answers);
            var result = await Mediator.Send(command);
            
            return Ok(result, result.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest<BulkAnswerOperationResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<BulkAnswerOperationResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<BulkAnswerOperationResponse>($"Failed to create multiple answers: {ex.Message}");
        }
    }

    /// <summary>
    /// Perform bulk operations on multiple answers
    /// </summary>
    [HttpPost("bulk")]
    public async Task<ActionResult<ApiResponse<BulkAnswerOperationResponse>>> BulkAnswerOperation([FromBody] BulkAnswerOperationRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<BulkAnswerOperationResponse>("Validation failed", errors);
        }

        try
        {
            var command = new BulkAnswerOperationCommand(request.AnswerIds, request.Operation);
            var result = await Mediator.Send(command);
            
            return Ok(result, result.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest<BulkAnswerOperationResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<BulkAnswerOperationResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<BulkAnswerOperationResponse>($"Failed to perform bulk operation: {ex.Message}");
        }
    }

    #endregion

    #region Answer Validation

    /// <summary>
    /// Validate answer business rules and constraints
    /// </summary>
    [HttpGet("{id:int}/validate")]
    public async Task<ActionResult<ApiResponse<AnswerValidationResponse>>> ValidateAnswer(int id)
    {
        try
        {
            var query = new ValidateAnswerQuery(id);
            var result = await Mediator.Send(query);
            
            var message = result.IsValid ? "Answer validation passed" : "Answer validation failed";
            return Ok(result, message);
        }
        catch (Exception)
        {
            return BadRequest<AnswerValidationResponse>("An error occurred while validating the answer");
        }
    }

    #endregion
}
