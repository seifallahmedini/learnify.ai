using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record GetLessonResourcesQuery(int Id) : IQuery<LessonResourcesResponse>;

public record LessonResourcesResponse(
    int LessonId,
    string LessonTitle,
    IEnumerable<LessonResource> Resources,
    int ResourceCount
);

public record LessonResource(
    int Id,
    string Name,
    string Type,
    string Url,
    long Size,
    DateTime UploadedAt
);

public class GetLessonResourcesValidator : AbstractValidator<GetLessonResourcesQuery>
{
    public GetLessonResourcesValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");
    }
}

public class GetLessonResourcesHandler : IRequestHandler<GetLessonResourcesQuery, LessonResourcesResponse>
{
    private readonly ILessonRepository _lessonRepository;

    public GetLessonResourcesHandler(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonResourcesResponse> Handle(GetLessonResourcesQuery request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (lesson == null)
            throw new ArgumentException($"Lesson with ID {request.Id} not found");

        // TODO: Implement proper resource management when resource entities are created
        // For now, return a mock response
        var resources = new List<LessonResource>
        {
            new LessonResource(
                1,
                "Lesson Notes.pdf",
                "PDF",
                "/resources/lesson-notes.pdf",
                1024000,
                DateTime.UtcNow.AddDays(-5)
            ),
            new LessonResource(
                2,
                "Code Examples.zip",
                "ZIP",
                "/resources/code-examples.zip",
                2048000,
                DateTime.UtcNow.AddDays(-3)
            )
        };

        return new LessonResourcesResponse(
            lesson.Id,
            lesson.Title,
            resources,
            resources.Count
        );
    }
}