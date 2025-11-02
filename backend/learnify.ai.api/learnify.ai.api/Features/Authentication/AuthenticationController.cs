using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using learnify.ai.api.Features.Authentication;
using learnify.ai.api.Features.Authentication.Operations.Commands.ChangePassword;
using learnify.ai.api.Features.Authentication.Operations.Commands.ForgotPassword;
using learnify.ai.api.Features.Authentication.Operations.Commands.Login;
using learnify.ai.api.Features.Authentication.Operations.Commands.RefreshToken;
using learnify.ai.api.Features.Authentication.Operations.Commands.Register;
using learnify.ai.api.Features.Authentication.Operations.Commands.ResetPassword;
using learnify.ai.api.Features.Authentication.Operations.Queries.GetCurrentUser;

namespace learnify.ai.api.Features.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : BaseController
{
    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest<RegisterResponse>("Passwords do not match");
        }

        var command = new RegisterCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.PhoneNumber
        );

        var result = await Mediator.Send(command);
        return Ok(result, "Registration successful");
    }

    /// <summary>
    /// Login user and get authentication tokens
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await Mediator.Send(command);

        if (result == null)
        {
            return Unauthorized<AuthResponse>("Invalid email or password");
        }

        return Ok(result, "Login successful");
    }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.Token, request.RefreshToken);
        var result = await Mediator.Send(command);

        if (result == null)
        {
            return Unauthorized<RefreshTokenResponse>("Invalid or expired token");
        }

        return Ok(result, "Token refreshed successfully");
    }

    /// <summary>
    /// Request password reset (forgot password)
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ForgotPasswordResponse>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await Mediator.Send(command);
        return Ok(result, result.Message);
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<ResetPasswordResponse>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            return BadRequest<ResetPasswordResponse>("Passwords do not match");
        }

        var command = new ResetPasswordCommand(
            request.Email,
            request.Token,
            request.NewPassword
        );

        var result = await Mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest<ResetPasswordResponse>(result.Message);
        }

        return Ok(result, result.Message);
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ChangePasswordResponse>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized<ChangePasswordResponse>("User not authenticated");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            return BadRequest<ChangePasswordResponse>("Passwords do not match");
        }

        var command = new ChangePasswordCommand(
            userId,
            request.CurrentPassword,
            request.NewPassword
        );

        var result = await Mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest<ChangePasswordResponse>(result.Message);
        }

        return Ok(result, result.Message);
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfo>>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized<UserInfo>("User not authenticated");
        }

        var query = new GetCurrentUserQuery(userId);
        var result = await Mediator.Send(query);

        if (result == null)
        {
            return NotFound<UserInfo>("User not found");
        }

        return Ok(result, "User information retrieved successfully");
    }
}

