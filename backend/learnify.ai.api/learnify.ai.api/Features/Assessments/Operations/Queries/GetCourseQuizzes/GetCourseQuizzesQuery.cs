using learnify.ai.api.Features.Courses;

namespace learnify.ai.api.Features.Assessments;

public record GetCourseQuizzesQuery(
    int CourseId
) : IQuery<CourseQuizzesResponse>;

public class GetCourseQuizzesValidator : AbstractValidator<GetCourseQuizzesQuery>
{
    public GetCourseQuizzesValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("Course ID must be greater than 0");
    }
}

public class GetCourseQuizzesHandler : IRequestHandler<GetCourseQuizzesQuery, CourseQuizzesResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;

    public GetCourseQuizzesHandler(
        IQuizRepository quizRepository,
        ICourseRepository courseRepository)
    {
        _quizRepository = quizRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CourseQuizzesResponse> Handle(GetCourseQuizzesQuery request, CancellationToken cancellationToken)
    {
        // Verify course exists
        var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null)
            throw new ArgumentException($"Course with ID {request.CourseId} not found");

        // Get all quizzes for the course (both active and inactive)
        var quizzes = await _quizRepository.GetByCourseIdAsync(request.CourseId, isActive: null, cancellationToken);
        var quizzesList = quizzes.ToList();

        // Build quiz summaries
        var quizSummaries = new List<QuizSummaryResponse>();
        foreach (var quiz in quizzesList.OrderByDescending(q => q.CreatedAt))
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

        // Calculate statistics
        var totalCount = quizzesList.Count;
        var activeQuizzes = quizzesList.Count(q => q.IsActive);
        var inactiveQuizzes = quizzesList.Count(q => !q.IsActive);

        return new CourseQuizzesResponse(
            course.Id,
            course.Title,
            quizSummaries,
            totalCount,
            activeQuizzes,
            inactiveQuizzes
        );
    }
}
