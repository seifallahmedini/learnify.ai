using MediatR;
using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Controllers;
using learnify.ai.api.Common.Models;
using learnify.ai.api.Features.Users.Commands.CreateUser;
using learnify.ai.api.Features.Users.Queries.GetUsers;
using learnify.ai.api.Features.Users.Queries.GetUserById;
using learnify.ai.api.Features.Users.Contracts.Requests;
using learnify.ai.api.Features.Users.Contracts.Responses;
using learnify.ai.api.Features.Users.Models;

namespace learnify.ai.api.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
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
        return Ok(result);
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

        return Ok(result);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Role,
            request.Bio,
            request.DateOfBirth,
            request.PhoneNumber
        );

        var result = await Mediator.Send(command);
        return Ok(result, "User created successfully");
    }

    /// <summary>
    /// Get instructors only
    /// </summary>
    [HttpGet("instructors")]
    public async Task<ActionResult<ApiResponse<UserListResponse>>> GetInstructors([FromQuery] UserFilterRequest request)
    {
        var query = new GetUsersQuery(
            UserRole.Instructor,
            request.IsActive,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get students only
    /// </summary>
    [HttpGet("students")]
    public async Task<ActionResult<ApiResponse<UserListResponse>>> GetStudents([FromQuery] UserFilterRequest request)
    {
        var query = new GetUsersQuery(
            UserRole.Student,
            request.IsActive,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result);
    }
}