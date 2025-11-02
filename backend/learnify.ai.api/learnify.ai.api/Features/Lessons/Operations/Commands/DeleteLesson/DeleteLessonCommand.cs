using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Lessons;

public record DeleteLessonCommand(int Id) : ICommand<bool>;

public class DeleteLessonValidator : AbstractValidator<DeleteLessonCommand>
{
    public DeleteLessonValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Lesson ID must be greater than 0");
    }
}

public class DeleteLessonHandler : IRequestHandler<DeleteLessonCommand, bool>
{
    private readonly ILessonRepository _lessonRepository;

    public DeleteLessonHandler(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<bool> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (lesson == null)
            return false;

        await _lessonRepository.DeleteAsync(lesson, cancellationToken);
        return true;
    }
}
