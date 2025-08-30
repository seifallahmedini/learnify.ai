using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Interfaces;

namespace learnify.ai.api.Features.Courses;

public record DeleteCourseCommand(int Id) : ICommand<bool>;

public class DeleteCourseValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, bool>
{
    private readonly ICourseRepository _courseRepository;

    public DeleteCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<bool> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (course == null)
            return false;

        await _courseRepository.DeleteAsync(course, cancellationToken);
        return true;
    }
}