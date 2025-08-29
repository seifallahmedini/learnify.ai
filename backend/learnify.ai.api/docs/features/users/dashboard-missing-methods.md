# ?? Missing Repository Methods for Dashboard

## Missing Methods to Implement:

### IEnrollmentRepository
- `GetActiveEnrollmentsAsync(int userId, CancellationToken cancellationToken)`
- `GetByCourseIdAsync(int courseId, CancellationToken cancellationToken)`

### IProgressRepository  
- `GetByEnrollmentIdAsync(int enrollmentId, CancellationToken cancellationToken)`

### IPaymentRepository
- `GetRevenueByInstructorAsync(int instructorId, CancellationToken cancellationToken)`
- `GetRevenueByCourseAsync(int courseId, CancellationToken cancellationToken)`

### IQuizRepository
- `GetByCourseIdAsync(int courseId, CancellationToken cancellationToken)`

These methods need to be added to their respective repository interfaces and implementations for the dashboard to function properly.

## Temporary Workaround:
The dashboard implementation includes defensive coding with null checks and empty collections as fallbacks, so it will compile and run but with limited functionality until these methods are implemented.