using FluentValidation;
using MediatR;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Domain.Entities;

namespace learnify.ai.api.Features.Assessments;

public record GetQuizzesQuery(
    int? CourseId = null,
    int? LessonId = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<QuizListResponse>;

public class GetQuizzesValidator : AbstractValidator<GetQuizzesQuery>
{
    public GetQuizzesValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .When(x => x.CourseId.HasValue)
            .WithMessage("Course ID must be greater than 0");

        RuleFor(x => x.LessonId)
            .GreaterThan(0)
            .When(x => x.LessonId.HasValue)
            .WithMessage("Lesson ID must be greater than 0");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetQuizzesHandler : IRequestHandler<GetQuizzesQuery, QuizListResponse>
{
    private readonly IQuizRepository _quizRepository;

    public GetQuizzesHandler(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<QuizListResponse> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
    {
        // Get quizzes based on filters at repository level
        IEnumerable<Quiz> quizzes;

        if (request.CourseId.HasValue)
        {
            quizzes = await _quizRepository.GetByCourseIdAsync(request.CourseId.Value, request.IsActive, cancellationToken);
        }
        else if (request.LessonId.HasValue)
        {
            quizzes = await _quizRepository.GetByLessonIdAsync(request.LessonId.Value, request.IsActive, cancellationToken);
        }
        else
        {
            // For GetAllAsync, apply isActive filter if provided
            if (request.IsActive.HasValue)
            {
                quizzes = await _quizRepository.FindAsync(q => q.IsActive == request.IsActive.Value, cancellationToken);
            }
            else
            {
                quizzes = await _quizRepository.GetAllAsync(cancellationToken);
            }
        }

        var quizzesList = quizzes.ToList();

        // Get total count before pagination
        var totalCount = quizzesList.Count;

        // Apply pagination
        var pagedQuizzes = quizzesList
            .OrderByDescending(q => q.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Build response
        var quizSummaries = new List<QuizSummaryResponse>();
        foreach (var quiz in pagedQuizzes)
        {
            var questionCount = await _quizRepository.GetQuestionCountAsync(quiz.Id, cancellationToken);
            
            quizSummaries.Add(new QuizSummaryResponse(
                quiz.Id,
                quiz.CourseId,
                quiz.LessonId,
                quiz.Title,
                quiz.Description,
                quiz.TimeLimit,
                quiz.PassingScore,
                quiz.IsActive,
                questionCount,
                quiz.CreatedAt
            ));
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new QuizListResponse(
            quizSummaries,
            totalCount,
            request.Page,
            request.PageSize,
            totalPages
        );
    }
}
