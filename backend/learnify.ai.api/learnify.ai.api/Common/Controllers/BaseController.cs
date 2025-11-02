using MediatR;
using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Models;

namespace learnify.ai.api.Common.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private ISender _mediator = null!;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected ActionResult<ApiResponse<T>> Ok<T>(T data, string? message = null)
    {
        return base.Ok(ApiResponse<T>.SuccessResult(data, message));
    }

    protected ActionResult<ApiResponse<T>> BadRequest<T>(string message, List<string>? errors = null)
    {
        return base.BadRequest(ApiResponse<T>.ErrorResult(message, errors));
    }

    protected ActionResult<ApiResponse<T>> NotFound<T>(string message)
    {
        return base.NotFound(ApiResponse<T>.ErrorResult(message));
    }

    protected ActionResult<ApiResponse<T>> Unauthorized<T>(string message)
    {
        return StatusCode(401, ApiResponse<T>.ErrorResult(message));
    }
}
