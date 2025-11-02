using MediatR;
using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Controllers;
using learnify.ai.api.Common.Models;
using learnify.ai.api.Common.Enums;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    #region User CRUD Operations

    /// <summary>
    /// Get all users with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<UserListResponse>>> GetUsers([FromQuery] UserFilterRequest request)
    {
        var query = new GetUsersQuery(
            request.Role,
            request.IsActive,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Users retrieved successfully");
    }

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserById(int id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<UserResponse>($"User with ID {id} not found");

        return Ok(result, "User retrieved successfully");
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result, "User created successfully");
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Bio,
            request.DateOfBirth,
            request.PhoneNumber,
            request.IsActive
        );

        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<UserResponse>($"User with ID {id} not found");

        return Ok(result, "User updated successfully");
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
    {
        var command = new DeleteUserCommand(id);
        var result = await Mediator.Send(command);

        if (!result)
            return NotFound<bool>($"User with ID {id} not found");

        return Ok(result, "User deleted successfully");
    }

    #endregion

    #region User Role Management

    /// <summary>
    /// Get instructors only
    /// </summary>
    [HttpGet("instructors")]
    public async Task<ActionResult<ApiResponse<UserListResponse>>> GetInstructors([FromQuery] UserFilterRequest request)
    {
        var query = new GetUsersQuery(
            RoleType.Instructor,
            request.IsActive,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Instructors retrieved successfully");
    }

    /// <summary>
    /// Get students only
    /// </summary>
    [HttpGet("students")]
    public async Task<ActionResult<ApiResponse<UserListResponse>>> GetStudents([FromQuery] UserFilterRequest request)
    {
        var query = new GetUsersQuery(
            RoleType.Student,
            request.IsActive,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Students retrieved successfully");
    }

    /// <summary>
    /// Get admin users only
    /// </summary>
    [HttpGet("admins")]
    public async Task<ActionResult<ApiResponse<UserListResponse>>> GetAdmins([FromQuery] UserFilterRequest request)
    {
        var query = new GetUsersQuery(
            RoleType.Admin,
            request.IsActive,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Admins retrieved successfully");
    }

    /// <summary>
    /// Activate user account
    /// </summary>
    [HttpPut("{id:int}/activate")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> ActivateUser(int id)
    {
        var command = new ActivateUserCommand(id);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<UserResponse>($"User with ID {id} not found");

        return Ok(result, "User activated successfully");
    }

    /// <summary>
    /// Deactivate user account
    /// </summary>
    [HttpPut("{id:int}/deactivate")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> DeactivateUser(int id)
    {
        var command = new DeactivateUserCommand(id);
        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<UserResponse>($"User with ID {id} not found");

        return Ok(result, "User deactivated successfully");
    }

    #endregion

    #region User Learning Data

    /// <summary>
    /// Get user's enrollments
    /// </summary>
    [HttpGet("{id:int}/enrollments")]
    public async Task<ActionResult<ApiResponse<GetUserEnrollmentsResponse>>> GetUserEnrollments(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Domain.Enums.EnrollmentStatus? status = null)
    {
        var query = new GetUserEnrollmentsQuery(id, page, pageSize, status);
        var result = await Mediator.Send(query);
        return Ok(result, "User enrollments retrieved successfully");
    }

    /// <summary>
    /// Get user's quiz attempts
    /// </summary>
    [HttpGet("{id:int}/quiz-attempts")]
    public async Task<ActionResult<ApiResponse<GetUserQuizAttemptsResponse>>> GetUserQuizAttempts(
        int id,
        [FromQuery] int? courseId = null,
        [FromQuery] bool? isPassed = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUserQuizAttemptsQuery(id, courseId, isPassed, page, pageSize);
        var result = await Mediator.Send(query);
        return Ok(result, "User quiz attempts retrieved successfully");
    }

    /// <summary>
    /// Get courses instructed by user
    /// </summary>
    [HttpGet("{id:int}/instructed-courses")]
    public async Task<ActionResult<ApiResponse<GetUserInstructedCoursesResponse>>> GetUserInstructedCourses(
        int id,
        [FromQuery] bool? isPublished = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUserInstructedCoursesQuery(id, isPublished, page, pageSize);
        var result = await Mediator.Send(query);
        return Ok(result, "User instructed courses retrieved successfully");
    }

    #endregion

    #region User Profile & Dashboard

    /// <summary>
    /// Get user profile information
    /// </summary>
    [HttpGet("{id:int}/profile")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserProfile(int id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<UserResponse>($"User with ID {id} not found");

        return Ok(result, "User profile retrieved successfully");
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("{id:int}/profile")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUserProfile(int id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Bio,
            request.DateOfBirth,
            request.PhoneNumber
        );

        var result = await Mediator.Send(command);

        if (result == null)
            return NotFound<UserResponse>($"User with ID {id} not found");

        return Ok(result, "User profile updated successfully");
    }

    /// <summary>
    /// Get comprehensive user dashboard data including enrollment stats, progress summary, 
    /// recent activity, quiz performance, and for instructors, course analytics
    /// </summary>
    [HttpGet("{id:int}/dashboard")]
    public async Task<ActionResult<ApiResponse<UserDashboardResponse>>> GetUserDashboard(int id)
    {
        var query = new GetUserDashboardQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound<UserDashboardResponse>($"User with ID {id} not found");

        return Ok(result, "User dashboard data retrieved successfully");
    }

    #endregion

    #region User Search & Discovery

    /// <summary>
    /// Search users by various criteria
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<UserListResponse>>> SearchUsers(
        [FromQuery] string searchTerm,
        [FromQuery] RoleType? role = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersQuery(role, isActive, searchTerm, page, pageSize);
        var result = await Mediator.Send(query);
        return Ok(result, $"Search results for '{searchTerm}' retrieved successfully");
    }

    /// <summary>
    /// Get active users count
    /// </summary>
    [HttpGet("count/active")]
    public async Task<ActionResult<ApiResponse<object>>> GetActiveUsersCount()
    {
        var query = new GetUsersQuery(IsActive: true, Page: 1, PageSize: 1);
        var result = await Mediator.Send(query);

        object response = new { Count = result.TotalCount };
        return Ok(response, "Active users count retrieved successfully");
    }

    /// <summary>
    /// Get comprehensive user statistics including role distribution, activity metrics, and growth analytics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<UserStatisticsResponse>>> GetUsersStatistics()
    {
        var query = new GetUserStatisticsQuery();
        var result = await Mediator.Send(query);
        return Ok(result, "User statistics retrieved successfully");
    }

    #endregion
}
