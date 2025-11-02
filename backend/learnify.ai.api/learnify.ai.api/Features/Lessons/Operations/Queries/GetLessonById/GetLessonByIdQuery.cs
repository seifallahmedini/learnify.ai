using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record GetLessonByIdQuery(int Id) : IQuery<LessonResponse?>;

public class GetLessonByIdValidator : AbstractValidator<GetLessonByIdQuery>
{
    public GetLessonByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");
    }
}

public class GetLessonByIdHandler : IRequestHandler<GetLessonByIdQuery, LessonResponse?>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public GetLessonByIdHandler(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonResponse?> Handle(GetLessonByIdQuery request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (lesson == null)
            return null;

        var course = await _courseRepository.GetByIdAsync(lesson.CourseId, cancellationToken);

        return new LessonResponse(
            lesson.Id,
            lesson.CourseId,
            course?.Title ?? "Unknown Course",
            lesson.Title,
            lesson.Description,
            lesson.Content,
            lesson.VideoUrl,
            lesson.Duration,
            lesson.OrderIndex,
            lesson.IsFree,
            lesson.IsPublished,
            lesson.LearningObjectives,
            lesson.Resources,
            lesson.CreatedAt,
            lesson.UpdatedAt
        );
    }
}
