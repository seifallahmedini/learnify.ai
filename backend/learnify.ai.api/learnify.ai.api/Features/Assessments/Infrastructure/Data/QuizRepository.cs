using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Data.Repositories;
using learnify.ai.api.Features.Assessments.Core.Models;

namespace learnify.ai.api.Features.Assessments.Infrastructure.Data;

public class QuizRepository : BaseRepository<Quiz>, IQuizRepository
{
    public QuizRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Quiz>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(q => q.CourseId == courseId && q.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Quiz>> GetByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(q => q.LessonId == lessonId && q.IsActive, cancellationToken);
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