using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses.Contracts.Responses;
using learnify.ai.api.Features.Courses.Infrastructure.Data;

namespace learnify.ai.api.Features.Courses.Operations.Queries.GetCourseById;

public record GetCourseByIdQuery(int Id) : IQuery<CourseResponse?>;

public class GetCourseByIdValidator : AbstractValidator<GetCourseByIdQuery>
{
    public GetCourseByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, CourseResponse?>
{
    private readonly ICourseRepository _courseRepository;

    public GetCourseByIdHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseResponse?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (course == null)
            return null;

        // TODO: Get instructor and category names from their respective repositories
        return new CourseResponse(
            course.Id,
            course.Title,
            course.Description,
            course.ShortDescription,
            course.InstructorId,
            "Instructor Name", // TODO: Load from UserRepository
            course.CategoryId,
            "Category Name", // TODO: Load from CategoryRepository
            course.Price,
            course.DiscountPrice,
            course.DurationHours,
            course.Level,
            course.Language,
            course.ThumbnailUrl,
            course.VideoPreviewUrl,
            course.IsPublished,
            course.MaxStudents,
            course.Prerequisites,
            course.LearningObjectives,
            course.CreatedAt,
            course.UpdatedAt
        );
    }
}