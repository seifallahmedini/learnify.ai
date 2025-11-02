using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Assessments;

public record GetQuizByIdQuery(
    int Id
) : IQuery<QuizResponse?>;

public class GetQuizByIdValidator : AbstractValidator<GetQuizByIdQuery>
{
    public GetQuizByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Quiz ID must be greater than 0");
    }
}

public class GetQuizByIdHandler : IRequestHandler<GetQuizByIdQuery, QuizResponse?>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public GetQuizByIdHandler(
        IQuizRepository quizRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        _quizRepository = quizRepository;
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<QuizResponse?> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetByIdAsync(request.Id, cancellationToken);
        if (quiz == null)
            return null;

        // Load related data
        var course = await _courseRepository.GetByIdAsync(quiz.CourseId, cancellationToken);
        string? lessonTitle = null;
        if (quiz.LessonId.HasValue)
        {
            var lesson = await _lessonRepository.GetByIdAsync(quiz.LessonId.Value, cancellationToken);
            lessonTitle = lesson?.Title;
        }

        var questionCount = await _quizRepository.GetQuestionCountAsync(quiz.Id, cancellationToken);
        var totalPoints = await _quizRepository.GetTotalPointsAsync(quiz.Id, cancellationToken);

        return new QuizResponse(
            quiz.Id,
            quiz.CourseId,
            course?.Title ?? "Unknown Course",
            quiz.LessonId,
            lessonTitle,
            quiz.Title,
            quiz.Description,
            quiz.TimeLimit,
            quiz.PassingScore,
            quiz.MaxAttempts,
            quiz.IsActive,
            questionCount,
            totalPoints,
            quiz.CreatedAt,
            quiz.UpdatedAt
        );
    }
}
