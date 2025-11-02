using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Infrastructure.Data.Repositories;

namespace learnify.ai.api.Features.Assessments;

public class QuizRepository : BaseRepository<Quiz>, IQuizRepository
{
    public QuizRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Quiz>> GetByCourseIdAsync(int courseId, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        if (isActive.HasValue)
        {
            return await FindAsync(q => q.CourseId == courseId && q.IsActive == isActive.Value, cancellationToken);
        }
        return await FindAsync(q => q.CourseId == courseId, cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetByLessonIdAsync(int lessonId, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        if (isActive.HasValue)
        {
            return await FindAsync(q => q.LessonId == lessonId && q.IsActive == isActive.Value, cancellationToken);
        }
        return await FindAsync(q => q.LessonId == lessonId, cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetActiveQuizzesAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(q => q.CourseId == courseId && q.IsActive, cancellationToken);
    }

    public async Task<int> GetTotalPointsAsync(int quizId, CancellationToken cancellationToken = default)
    {
        var totalPoints = await _context.Questions
            .Where(q => q.QuizId == quizId && q.IsActive)
            .SumAsync(q => q.Points, cancellationToken);
        
        return totalPoints;
    }

    public async Task<int> GetQuestionCountAsync(int quizId, CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .CountAsync(q => q.QuizId == quizId && q.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetQuizzesByInstructorAsync(int instructorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Join(_context.Courses,
                quiz => quiz.CourseId,
                course => course.Id,
                (quiz, course) => new { Quiz = quiz, Course = course })
            .Where(joined => joined.Course.InstructorId == instructorId && joined.Quiz.IsActive)
            .Select(joined => joined.Quiz)
            .ToListAsync(cancellationToken);
    }
}
