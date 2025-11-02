using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Controllers;
using learnify.ai.api.Common.Models;
using learnify.ai.api.Domain.Enums;

namespace learnify.ai.api.Features.Enrollments;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : BaseController
{
    #region Enrollment Operations

    /// <summary>
    /// Enroll user in course
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> EnrollInCourse([FromBody] EnrollInCourseRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<EnrollmentResponse>("Validation failed", errors);
        }

        try
        {
            var command = new EnrollInCourseCommand(
                request.UserId,
                request.CourseId,
                request.PaymentId
            );

            var result = await Mediator.Send(command);
            return Ok(result, "User enrolled in course successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<EnrollmentResponse>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<EnrollmentResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<EnrollmentResponse>($"Failed to enroll in course: {ex.Message}");
        }
    }

    /// <summary>
    /// Get enrollment details
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<EnrollmentDetailsResponse>>> GetEnrollment(int id)
    {
        try
        {
            var query = new GetEnrollmentByIdQuery(id);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<EnrollmentDetailsResponse>($"Enrollment with ID {id} not found");

            return Ok(result, "Enrollment retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<EnrollmentDetailsResponse>("An error occurred while retrieving the enrollment");
        }
    }

    /// <summary>
    /// Unenroll from course
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> UnenrollFromCourse(int id)
    {
        try
        {
            var command = new UnenrollFromCourseCommand(id);
            var result = await Mediator.Send(command);

            if (!result)
                return NotFound<bool>($"Enrollment with ID {id} not found");

            return Ok(result, "Successfully unenrolled from course");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<bool>(ex.Message);
        }
    }

    #endregion

    #region Enrollment Management

    /// <summary>
    /// Get course enrollments
    /// </summary>
    [HttpGet("course/{courseId:int}")]
    public async Task<ActionResult<ApiResponse<CourseEnrollmentsResponse>>> GetCourseEnrollments(
        int courseId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] EnrollmentStatus? status = null,
        [FromQuery] DateTime? enrolledAfter = null,
        [FromQuery] DateTime? enrolledBefore = null)
    {
        try
        {
            var query = new GetCourseEnrollmentsQuery(
                courseId,
                page,
                pageSize,
                status,
                enrolledAfter,
                enrolledBefore
            );

            var result = await Mediator.Send(query);
            return Ok(result, "Course enrollments retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<CourseEnrollmentsResponse>(ex.Message);
        }
    }

    /// <summary>
    /// Update enrollment status
    /// </summary>
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<EnrollmentResponse>>> UpdateEnrollmentStatus(int id, [FromBody] UpdateEnrollmentStatusRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<EnrollmentResponse>("Validation failed", errors);
        }

        try
        {
            var command = new UpdateEnrollmentStatusCommand(id, request.Status);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<EnrollmentResponse>($"Enrollment with ID {id} not found");

            return Ok(result, "Enrollment status updated successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<EnrollmentResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<EnrollmentResponse>($"Failed to update enrollment status: {ex.Message}");
        }
    }

    #endregion

    #region Progress Tracking

    /// <summary>
    /// Get enrollment progress
    /// </summary>
    [HttpGet("{id:int}/progress")]
    public async Task<ActionResult<ApiResponse<EnrollmentProgressResponse>>> GetEnrollmentProgress(int id)
    {
        try
        {
            var query = new GetEnrollmentProgressQuery(id);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<EnrollmentProgressResponse>($"Enrollment with ID {id} not found");

            return Ok(result, "Enrollment progress retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<EnrollmentProgressResponse>("An error occurred while retrieving enrollment progress");
        }
    }

    /// <summary>
    /// Update enrollment progress
    /// </summary>
    [HttpPut("{id:int}/progress")]
    public async Task<ActionResult<ApiResponse<EnrollmentProgressResponse>>> UpdateEnrollmentProgress(int id, [FromBody] UpdateEnrollmentProgressRequest request)
    {
        // Add model validation check
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<EnrollmentProgressResponse>("Validation failed", errors);
        }

        try
        {
            var command = new UpdateEnrollmentProgressCommand(
                id,
                request.LessonId,
                request.IsCompleted,
                request.TimeSpentMinutes
            );

            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<EnrollmentProgressResponse>($"Enrollment with ID {id} not found");

            return Ok(result, "Enrollment progress updated successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<EnrollmentProgressResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<EnrollmentProgressResponse>($"Failed to update enrollment progress: {ex.Message}");
        }
    }

    /// <summary>
    /// Get completion status
    /// </summary>
    [HttpGet("{id:int}/completion")]
    public async Task<ActionResult<ApiResponse<CompletionStatusResponse>>> GetCompletionStatus(int id)
    {
        try
        {
            var query = new GetCompletionStatusQuery(id);
            var result = await Mediator.Send(query);

            if (result == null)
                return NotFound<CompletionStatusResponse>($"Enrollment with ID {id} not found");

            return Ok(result, "Completion status retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<CompletionStatusResponse>("An error occurred while retrieving completion status");
        }
    }

    /// <summary>
    /// Generate completion certificate
    /// </summary>
    [HttpPost("{id:int}/certificate")]
    public async Task<ActionResult<ApiResponse<CertificateResponse>>> GenerateCertificate(int id, [FromBody] GenerateCertificateRequest? request = null)
    {
        // Add model validation check (only if request is not null)
        if (request != null && !ModelState.IsValid)
        {
            var errors = ModelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();
            return BadRequest<CertificateResponse>("Validation failed", errors);
        }

        try
        {
            var command = new GenerateCertificateCommand(id, request?.CertificateTemplate);
            var result = await Mediator.Send(command);

            if (result == null)
                return NotFound<CertificateResponse>($"Enrollment with ID {id} not found");

            return Ok(result, "Certificate generated successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest<CertificateResponse>(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest<CertificateResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest<CertificateResponse>($"Failed to generate certificate: {ex.Message}");
        }
    }

    #endregion

    #region Enrollment Analytics

    /// <summary>
    /// Get enrollment statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<EnrollmentStatisticsResponse>>> GetEnrollmentStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? userId = null,
        [FromQuery] int? courseId = null)
    {
        try
        {
            var query = new GetEnrollmentStatisticsQuery(startDate, endDate, userId, courseId);
            var result = await Mediator.Send(query);
            return Ok(result, "Enrollment statistics retrieved successfully");
        }
        catch (Exception)
        {
            return BadRequest<EnrollmentStatisticsResponse>("An error occurred while retrieving enrollment statistics");
        }
    }

    /// <summary>
    /// Get course completion rate
    /// </summary>
    [HttpGet("course/{courseId:int}/completion-rate")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetCourseCompletionRate(int courseId)
    {
        try
        {
            var query = new GetCourseCompletionRateQuery(courseId);
            var result = await Mediator.Send(query);
            return Ok(result, "Course completion rate retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest<decimal>(ex.Message);
        }
    }

    #endregion
}
