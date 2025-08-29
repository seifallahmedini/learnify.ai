using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses.Contracts.Responses;
using learnify.ai.api.Features.Courses.Infrastructure.Data;
using learnify.ai.api.Features.Courses.Core.Models;

namespace learnify.ai.api.Features.Courses.Operations.Commands.UpdateCourse;

public record UpdateCourseCommand(
    int Id,
    string? Title = null,
    string? Description = null,
    string? ShortDescription = null,
    int? CategoryId = null,
    decimal? Price = null,
    decimal? DiscountPrice = null,
    int? DurationHours = null,
    CourseLevel? Level = null,
    string? Language = null,
    string? ThumbnailUrl = null,
    string? VideoPreviewUrl = null,
    bool? IsPublished = null,
    bool? IsFeatured = null,
    int? MaxStudents = null,
    string? Prerequisites = null,
    string? LearningObjectives = null
) : ICommand<CourseResponse?>;

public class UpdateCourseValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Course title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Course description cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500)
            .WithMessage("Short description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ShortDescription));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue)
            .WithMessage("Valid category is required");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Price.HasValue)
            .WithMessage("Course price must be 0 or greater");

        RuleFor(x => x.DiscountPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DiscountPrice.HasValue)
            .WithMessage("Discount price must be 0 or greater");

        RuleFor(x => x.DurationHours)
            .GreaterThan(0)
            .When(x => x.DurationHours.HasValue)
            .WithMessage("Course duration must be greater than 0 hours");
    }
}

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, CourseResponse?>
{
    private readonly ICourseRepository _courseRepository;

    public UpdateCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseResponse?> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (course == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.Title))
            course.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Description))
            course.Description = request.Description;

        if (!string.IsNullOrEmpty(request.ShortDescription))
            course.ShortDescription = request.ShortDescription;

        if (request.CategoryId.HasValue)
            course.CategoryId = request.CategoryId.Value;

        if (request.Price.HasValue)
            course.Price = request.Price.Value;

        if (request.DiscountPrice.HasValue)
            course.DiscountPrice = request.DiscountPrice.Value;

        if (request.DurationHours.HasValue)
            course.DurationHours = request.DurationHours.Value;

        if (request.Level.HasValue)
            course.Level = request.Level.Value;

        if (!string.IsNullOrEmpty(request.Language))
            course.Language = request.Language;

        if (request.ThumbnailUrl != null)
            course.ThumbnailUrl = request.ThumbnailUrl;

        if (request.VideoPreviewUrl != null)
            course.VideoPreviewUrl = request.VideoPreviewUrl;

        if (request.IsPublished.HasValue)
            course.IsPublished = request.IsPublished.Value;

        if (request.IsFeatured.HasValue)
            course.IsFeatured = request.IsFeatured.Value;

        if (request.MaxStudents.HasValue)
            course.MaxStudents = request.MaxStudents.Value;

        if (request.Prerequisites != null)
            course.Prerequisites = request.Prerequisites;

        if (!string.IsNullOrEmpty(request.LearningObjectives))
            course.LearningObjectives = request.LearningObjectives;

        var updatedCourse = await _courseRepository.UpdateAsync(course, cancellationToken);

        // TODO: Get instructor and category names from their respective repositories
        return new CourseResponse(
            updatedCourse.Id,
            updatedCourse.Title,
            updatedCourse.Description,
            updatedCourse.ShortDescription,
            updatedCourse.InstructorId,
            "Instructor Name", // TODO: Load from UserRepository
            updatedCourse.CategoryId,
            "Category Name", // TODO: Load from CategoryRepository
            updatedCourse.Price,
            updatedCourse.DiscountPrice,
            updatedCourse.DurationHours,
            updatedCourse.Level,
            updatedCourse.Language,
            updatedCourse.ThumbnailUrl,
            updatedCourse.VideoPreviewUrl,
            updatedCourse.IsPublished,
            updatedCourse.IsFeatured,
            updatedCourse.MaxStudents,
            updatedCourse.Prerequisites,
            updatedCourse.LearningObjectives,
            updatedCourse.CreatedAt,
            updatedCourse.UpdatedAt
        );
    }
}