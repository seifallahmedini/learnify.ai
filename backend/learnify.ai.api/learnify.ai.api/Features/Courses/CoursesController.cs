using Microsoft.AspNetCore.Mvc;
using learnify.ai.api.Common.Controllers;
using learnify.ai.api.Common.Models;
using learnify.ai.api.Features.Courses.Operations.Queries.GetCourses;
using learnify.ai.api.Features.Courses.Operations.Queries.GetCourseById;
using learnify.ai.api.Features.Courses.Operations.Commands.CreateCourse;
using learnify.ai.api.Features.Courses.Contracts.Requests;
using learnify.ai.api.Features.Courses.Contracts.Responses;

namespace learnify.ai.api.Features.Courses;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : BaseController
{
    /// <summary>
    /// Get all courses with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CourseListResponse>>> GetCourses([FromQuery] CourseFilterRequest request)
    {
        var query = new GetCoursesQuery(
            request.CategoryId,
            request.InstructorId,
            request.Level,
            request.IsPublished,
            request.MinPrice,
            request.MaxPrice,
            request.SearchTerm,
            request.Page,
            request.PageSize
        );

        var result = await Mediator.Send(query);
        return Ok(result, "Courses retrieved successfully");
    }

    /// <summary>
    /// Get a specific course by ID
    /// </summary>
    [HttpGet("{id:int}")]  
    public async Task<ActionResult<ApiResponse<CourseResponse>>> GetCourse(int id)
    {
        var query = new GetCourseByIdQuery(id);
        var result = await Mediator.Send(query);
        
        if (result == null)
            return NotFound<CourseResponse>($"Course with ID {id} not found");
            
        return Ok(result, "Course retrieved successfully");
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CourseResponse>>> CreateCourse([FromBody] CreateCourseRequest request)
    {
        var command = new CreateCourseCommand(
            request.Title,
            request.Description,
            request.ShortDescription,
            request.InstructorId,
            request.CategoryId,
            request.Price,
            request.DiscountPrice,
            request.DurationHours,
            request.Level,
            request.Language,
            request.ThumbnailUrl,
            request.VideoPreviewUrl,
            request.IsPublished,
            request.MaxStudents,
            request.Prerequisites,
            request.LearningObjectives
        );

        var result = await Mediator.Send(command);
        return Ok(result, "Course created successfully");
    }
}