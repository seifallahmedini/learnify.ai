using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Controllers;
using learnify.ai.api.Common.Models;

namespace learnify.ai.api.Features.Lessons;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : BaseController
{
    #region Lesson CRUD Operations

    /// <summary>
    /// Get lesson by ID with full details
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> GetLesson(int id)
    {
        var query = new GetLessonByIdQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<LessonResponse>($"Lesson with ID {id} not found");

        return Ok(result, "Lesson retrieved successfully");
    }

    /// <summary>
    /// Update lesson details
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> UpdateLesson(int id, [FromBody] UpdateLessonRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<LessonResponse>("Validation failed", errors);
        }

        try
        {
            var command = new UpdateLessonCommand(
                id,
                request.Title,
                request.Description,
                request.Content,
                request.VideoUrl,
                request.Duration,
                request.OrderIndex,
                request.IsFree,
                request.IsPublished
            );

            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<LessonResponse>($"Lesson with ID {id} not found");

            return Ok(result, "Lesson updated successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<LessonResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<LessonResponse>($"Failed to update lesson: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete lesson permanently
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteLesson(int id)
    {
        var command = new DeleteLessonCommand(id);
        var result = await Mediator.Send(command);

        if (!result)
            return NotFound<bool>($"Lesson with ID {id} not found");

        return Ok(result, "Lesson deleted successfully");
    }

    #endregion

    #region Lesson Organization

    /// <summary>
    /// Reorder lesson within a course
    /// </summary>
    [HttpPut("{id:int}/reorder")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> ReorderLesson(int id, [FromBody] ReorderLessonRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<LessonResponse>("Validation failed", errors);
        }

        try
        {
            var command = new ReorderLessonCommand(id, request.NewOrderIndex);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<LessonResponse>($"Lesson with ID {id} not found");

            return Ok(result, "Lesson reordered successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<LessonResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<LessonResponse>($"Failed to reorder lesson: {ex.Message}");
        }
    }

    /// <summary>
    /// Get next lesson in the course sequence
    /// </summary>
    [HttpGet("{id:int}/next")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> GetNextLesson(int id)
    {
        var query = new GetNextLessonQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<LessonResponse>($"No next lesson found for lesson ID {id}");

        return Ok(result, "Next lesson retrieved successfully");
    }

    /// <summary>
    /// Get previous lesson in the course sequence
    /// </summary>
    [HttpGet("{id:int}/previous")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> GetPreviousLesson(int id)
    {
        var query = new GetPreviousLessonQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<LessonResponse>($"No previous lesson found for lesson ID {id}");

        return Ok(result, "Previous lesson retrieved successfully");
    }

    #endregion

    #region Lesson Content

    /// <summary>
    /// Upload or update lesson video URL
    /// </summary>
    [HttpPost("{id:int}/video")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> UploadLessonVideo(int id, [FromBody] UploadVideoRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<LessonResponse>("Validation failed", errors);
        }

        try
        {
            var command = new UploadLessonVideoCommand(id, request.VideoUrl);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<LessonResponse>($"Lesson with ID {id} not found");

            return Ok(result, "Lesson video uploaded successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<LessonResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<LessonResponse>($"Failed to upload lesson video: {ex.Message}");
        }
    }

    /// <summary>
    /// Update lesson content
    /// </summary>
    [HttpPut("{id:int}/content")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> UpdateLessonContent(int id, [FromBody] UpdateContentRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<LessonResponse>("Validation failed", errors);
        }

        try
        {
            var command = new UpdateLessonContentCommand(id, request.Content);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<LessonResponse>($"Lesson with ID {id} not found");

            return Ok(result, "Lesson content updated successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<LessonResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<LessonResponse>($"Failed to update lesson content: {ex.Message}");
        }
    }

    /// <summary>
    /// Get lesson resources and attachments
    /// </summary>
    [HttpGet("{id:int}/resources")]
    public async Task<ActionResult<ApiResponse<LessonResourcesResponse>>> GetLessonResources(int id)
    {
        var query = new GetLessonResourcesQuery(id);
        
        try
        {
            var result = await Mediator.Send(query);
            return Ok(result, "Lesson resources retrieved successfully");
        }
        catch (ArgumentException)
        {
            return NotFound<LessonResourcesResponse>($"Lesson with ID {id} not found");
        }
    }

    #endregion

    #region Lesson Access Control

    /// <summary>
    /// Publish lesson to make it visible to students
    /// </summary>
    [HttpPut("{id:int}/publish")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> PublishLesson(int id)
    {
        var command = new PublishLessonCommand(id);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<LessonResponse>($"Lesson with ID {id} not found");

        return Ok(result, "Lesson published successfully");
    }

    /// <summary>
    /// Unpublish lesson to hide it from students
    /// </summary>
    [HttpPut("{id:int}/unpublish")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> UnpublishLesson(int id)
    {
        var command = new UnpublishLessonCommand(id);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<LessonResponse>($"Lesson with ID {id} not found");

        return Ok(result, "Lesson unpublished successfully");
    }

    /// <summary>
    /// Make lesson free or premium
    /// </summary>
    [HttpPut("{id:int}/free")]
    public async Task<ActionResult<ApiResponse<LessonResponse>>> MakeLessonFree(int id, [FromQuery] bool isFree = true)
    {
        var command = new MakeLessonFreeCommand(id, isFree);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<LessonResponse>($"Lesson with ID {id} not found");

        var message = isFree ? "Lesson made free successfully" : "Lesson made premium successfully";
        return Ok(result, message);
    }

    #endregion
}