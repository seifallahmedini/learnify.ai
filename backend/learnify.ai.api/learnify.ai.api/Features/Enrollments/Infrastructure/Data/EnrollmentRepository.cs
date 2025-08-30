using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Data.Repositories;

namespace learnify.ai.api.Features.Enrollments;

public class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(LearnifyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Enrollment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(e => e.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(e => e.CourseId == courseId, cancellationToken);
    }

    public async Task<Enrollment?> GetByUserAndCourseAsync(int userId, int courseId, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetByStatusAsync(EnrollmentStatus status, CancellationToken cancellationToken = default)
    {
        return await FindAsync(e => e.Status == status, cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetActiveEnrollmentsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(e => e.UserId == userId && e.Status == EnrollmentStatus.Active, cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetCompletedEnrollmentsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await FindAsync(e => e.UserId == userId && e.Status == EnrollmentStatus.Completed, cancellationToken);
    }

    public async Task<int> GetEnrollmentCountByCourseAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await CountAsync(e => e.CourseId == courseId && 
                                    (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed), 
                               cancellationToken);
    }

    public async Task<bool> IsUserEnrolledAsync(int userId, int courseId, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(e => e.UserId == userId && e.CourseId == courseId && 
                                     (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Completed), 
                                cancellationToken);
    }
}