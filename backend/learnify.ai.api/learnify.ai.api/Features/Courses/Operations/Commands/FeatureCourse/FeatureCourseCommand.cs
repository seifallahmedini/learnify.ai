using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Features.Courses;

public record FeatureCourseCommand(int Id, bool IsFeatured = true) : ICommand<CourseResponse?>;

public class FeatureCourseValidator : AbstractValidator<FeatureCourseCommand>
{
    public FeatureCourseValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class FeatureCourseHandler : IRequestHandler<FeatureCourseCommand, CourseResponse?>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;

    public FeatureCourseHandler(
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository)
    {
        _courseRepository = courseRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<CourseResponse?> Handle(FeatureCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (course == null)
            return null;

        course.IsFeatured = request.IsFeatured;
        var updatedCourse = await _courseRepository.UpdateAsync(course, cancellationToken);

        // Load instructor and category names
        var instructor = await _userRepository.GetByIdAsync(updatedCourse.InstructorId, cancellationToken);
        var category = await _categoryRepository.GetByIdAsync(updatedCourse.CategoryId, cancellationToken);

        return new CourseResponse(
            updatedCourse.Id,
            updatedCourse.Title,
            updatedCourse.Description,
            updatedCourse.ShortDescription,
            updatedCourse.InstructorId,
            instructor?.GetFullName() ?? "Unknown Instructor",
            updatedCourse.CategoryId,
            category?.Name ?? "Unknown Category",
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
