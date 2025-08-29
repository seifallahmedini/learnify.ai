using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses.Contracts.Responses;
using learnify.ai.api.Features.Courses.Infrastructure.Data;
using learnify.ai.api.Features.Courses.Core.Models;

namespace learnify.ai.api.Features.Courses.Operations.Commands.CreateCourse;

public record CreateCourseCommand(
    string Title,
    string Description,
    string ShortDescription,
    int InstructorId,
    int CategoryId,
    decimal Price,
    decimal? DiscountPrice,
    int DurationHours,
    CourseLevel Level,
    string Language,
    string? ThumbnailUrl,
    string? VideoPreviewUrl,
    bool IsPublished,
    int? MaxStudents,
    string? Prerequisites,
    string LearningObjectives
) : ICommand<CourseResponse>;

public class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Course title is required")
            .MaximumLength(200)
            .WithMessage("Course title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Course description is required")
            .MaximumLength(2000)
            .WithMessage("Course description cannot exceed 2000 characters");

        RuleFor(x => x.ShortDescription)
            .NotEmpty()
            .WithMessage("Short description is required")
            .MaximumLength(500)
            .WithMessage("Short description cannot exceed 500 characters");

        RuleFor(x => x.InstructorId)
            .GreaterThan(0)
            .WithMessage("Valid instructor is required");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Valid category is required");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Course price must be 0 or greater");

        RuleFor(x => x.DiscountPrice)
            .LessThan(x => x.Price)
            .When(x => x.DiscountPrice.HasValue)
            .WithMessage("Discount price must be less than regular price");

        RuleFor(x => x.DurationHours)
            .GreaterThan(0)
            .WithMessage("Course duration must be greater than 0 hours");

        RuleFor(x => x.Language)
            .NotEmpty()
            .WithMessage("Language is required");

        RuleFor(x => x.LearningObjectives)
            .NotEmpty()
            .WithMessage("Learning objectives are required");
    }
}

public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, CourseResponse>
{
    private readonly ICourseRepository _courseRepository;

    public CreateCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseResponse> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            InstructorId = request.InstructorId,
            CategoryId = request.CategoryId,
            Price = request.Price,
            DiscountPrice = request.DiscountPrice,
            DurationHours = request.DurationHours,
            Level = request.Level,
            Language = request.Language,
            ThumbnailUrl = request.ThumbnailUrl,
            VideoPreviewUrl = request.VideoPreviewUrl,
            IsPublished = request.IsPublished,
            MaxStudents = request.MaxStudents,
            Prerequisites = request.Prerequisites,
            LearningObjectives = request.LearningObjectives
        };

        var createdCourse = await _courseRepository.CreateAsync(course, cancellationToken);

        // TODO: Get instructor and category names from their respective repositories
        return new CourseResponse(
            createdCourse.Id,
            createdCourse.Title,
            createdCourse.Description,
            createdCourse.ShortDescription,
            createdCourse.InstructorId,
            "Instructor Name", // TODO: Load from UserRepository
            createdCourse.CategoryId,
            "Category Name", // TODO: Load from CategoryRepository
            createdCourse.Price,
            createdCourse.DiscountPrice,
            createdCourse.DurationHours,
            createdCourse.Level,
            createdCourse.Language,
            createdCourse.ThumbnailUrl,
            createdCourse.VideoPreviewUrl,
            createdCourse.IsPublished,
            createdCourse.MaxStudents,
            createdCourse.Prerequisites,
            createdCourse.LearningObjectives,
            createdCourse.CreatedAt,
            createdCourse.UpdatedAt
        );
    }
}