# Repository Architecture Summary

## ??? Complete Repository Structure

```
learnify.ai.api/
??? Common/
?   ??? Data/
?   ?   ??? LearnifyDbContext.cs
?   ?   ??? Repositories/
?   ?       ??? IBaseRepository.cs         # Generic base interface
?   ?       ??? BaseRepository.cs          # Generic base implementation
?   ??? Extensions/
?       ??? ServiceCollectionExtensions.cs # DI Registration
??? Features/
    ??? Users/
    ?   ??? Data/
    ?   ?   ??? IUserRepository.cs         ? Created
    ?   ?   ??? UserRepository.cs          ? Created
    ?   ??? GetUsers/
    ?   ?   ??? GetUsersQuery.cs           ? Updated to use repository
    ?   ??? Models/
    ?       ??? User.cs
    ??? Courses/
    ?   ??? Data/
    ?   ?   ??? ICourseRepository.cs       ? Created
    ?   ?   ??? CourseRepository.cs        ? Created
    ?   ?   ??? ICategoryRepository.cs     ? Created
    ?   ?   ??? CategoryRepository.cs      ? Created
    ?   ?   ??? ILessonRepository.cs       ? Created
    ?   ?   ??? LessonRepository.cs        ? Created
    ?   ??? GetCourse/
    ?   ?   ??? GetCourseQuery.cs          ? Updated to use repository
    ?   ??? Models/
    ?       ??? Course.cs
    ?       ??? Category.cs
    ?       ??? Lesson.cs
    ??? Enrollments/
    ?   ??? Data/
    ?   ?   ??? IEnrollmentRepository.cs   ? Created
    ?   ?   ??? EnrollmentRepository.cs    ? Created
    ?   ?   ??? IProgressRepository.cs     ? Created
    ?   ?   ??? ProgressRepository.cs      ? Created
    ?   ??? Models/
    ?       ??? Enrollment.cs
    ?       ??? Progress.cs
    ??? Assessments/
    ?   ??? Data/
    ?   ?   ??? IQuizRepository.cs         ? Created
    ?   ?   ??? QuizRepository.cs          ? Created
    ?   ?   ??? IQuestionRepository.cs     ? Created
    ?   ?   ??? QuestionRepository.cs      ? Created
    ?   ?   ??? IAnswerRepository.cs       ? Created
    ?   ?   ??? AnswerRepository.cs        ? Created
    ?   ?   ??? IQuizAttemptRepository.cs  ? Created
    ?   ?   ??? QuizAttemptRepository.cs   ? Created
    ?   ??? Models/
    ?       ??? Quiz.cs
    ?       ??? Question.cs
    ?       ??? Answer.cs
    ?       ??? QuizAttempt.cs
    ??? Payments/
    ?   ??? Data/
    ?   ?   ??? IPaymentRepository.cs      ? Created
    ?   ?   ??? PaymentRepository.cs       ? Created
    ?   ??? Models/
    ?       ??? Payment.cs
    ??? Reviews/
        ??? Data/
        ?   ??? IReviewRepository.cs       ? Created
        ?   ??? ReviewRepository.cs        ? Created
        ??? Models/
            ??? Review.cs
```

## ?? Repository Features by Domain

### ?? **User Management**
- **IUserRepository / UserRepository**
  - Basic CRUD operations (inherited from BaseRepository)
  - `GetByEmailAsync()` - Find user by email
  - `GetInstructorsAsync()` - Get all instructors
  - `GetStudentsAsync()` - Get all students
  - `GetActiveUsersAsync()` - Get active users only
  - `EmailExistsAsync()` - Check email uniqueness

### ?? **Course Management**

#### **ICourseRepository / CourseRepository**
- Basic CRUD operations
- `GetByInstructorIdAsync()` - Courses by instructor
- `GetByCategoryIdAsync()` - Courses by category
- `GetPublishedCoursesAsync()` - Published courses with pagination
- `GetEnrollmentCountAsync()` - Count enrolled students
- `GetAverageRatingAsync()` - Course average rating
- `GetTotalStudentsAsync()` - Total student count

#### **ICategoryRepository / CategoryRepository**
- Basic CRUD operations
- `GetRootCategoriesAsync()` - Top-level categories
- `GetSubCategoriesAsync()` - Child categories
- `GetActiveCategoriesAsync()` - Active categories only
- `HasSubCategoriesAsync()` - Check if category has children
- `GetCourseCountAsync()` - Count courses in category
- `GetCategoryHierarchyAsync()` - Get parent chain

#### **ILessonRepository / LessonRepository**
- Basic CRUD operations
- `GetByCourseIdAsync()` - Lessons by course (ordered)
- `GetPublishedLessonsAsync()` - Published lessons only
- `GetFreeLessonsAsync()` - Free preview lessons
- `GetNextLessonAsync()` / `GetPreviousLessonAsync()` - Navigation
- `GetTotalDurationAsync()` - Course total duration
- `GetLessonCountAsync()` - Count lessons in course
- `GetMaxOrderIndexAsync()` - For ordering new lessons

### ?? **Enrollment System**

#### **IEnrollmentRepository / EnrollmentRepository**
- Basic CRUD operations
- `GetByUserIdAsync()` - User's enrollments
- `GetByCourseIdAsync()` - Course enrollments
- `GetByUserAndCourseAsync()` - Specific enrollment
- `GetByStatusAsync()` - Filter by status
- `GetActiveEnrollmentsAsync()` / `GetCompletedEnrollmentsAsync()`
- `GetEnrollmentCountByCourseAsync()` - Course popularity
- `IsUserEnrolledAsync()` - Check enrollment status

#### **IProgressRepository / ProgressRepository**
- Basic CRUD operations
- `GetByEnrollmentIdAsync()` - All lesson progress
- `GetByLessonIdAsync()` - Progress across users
- `GetByEnrollmentAndLessonAsync()` - Specific progress
- `GetCompletedLessonsAsync()` / `GetIncompleteLessonsAsync()`
- `GetCompletedLessonsCountAsync()` - Count completed
- `GetTotalTimeSpentAsync()` - Time tracking
- `GetProgressPercentageAsync()` - Calculate completion %

### ?? **Assessment System**

#### **IQuizRepository / QuizRepository**
- Basic CRUD operations
- `GetByCourseIdAsync()` / `GetByLessonIdAsync()` - Quiz location
- `GetActiveQuizzesAsync()` - Active quizzes only
- `GetTotalPointsAsync()` - Quiz total points
- `GetQuestionCountAsync()` - Count questions
- `GetQuizzesByInstructorAsync()` - Instructor's quizzes

#### **IQuestionRepository / QuestionRepository**
- Basic CRUD operations
- `GetByQuizIdAsync()` - Quiz questions (ordered)
- `GetActiveQuestionsAsync()` - Active questions only
- `GetByOrderIndexAsync()` - Specific question by order
- `GetMaxOrderIndexAsync()` - For ordering new questions
- `GetByTypeAsync()` - Filter by question type
- `GetTotalPointsAsync()` - Calculate quiz points

#### **IAnswerRepository / AnswerRepository**
- Basic CRUD operations
- `GetByQuestionIdAsync()` - Question answers (ordered)
- `GetCorrectAnswersAsync()` / `GetCorrectAnswerAsync()` - Correct answers
- `GetByOrderIndexAsync()` - Specific answer by order
- `GetMaxOrderIndexAsync()` - For ordering new answers
- `HasCorrectAnswerAsync()` - Validation check

#### **IQuizAttemptRepository / QuizAttemptRepository**
- Basic CRUD operations
- `GetByQuizIdAsync()` / `GetByUserIdAsync()` - Attempts by entity
- `GetByUserAndQuizAsync()` - User's quiz attempts
- `GetLatestAttemptAsync()` / `GetBestAttemptAsync()` - Specific attempts
- `GetAttemptCountAsync()` - Count attempts (for limits)
- `GetPassedAttemptsAsync()` / `GetFailedAttemptsAsync()` - By result
- `GetInProgressAttemptsAsync()` - Incomplete attempts
- `GetAverageScoreAsync()` - Quiz difficulty analysis

### ?? **Business Operations**

#### **IPaymentRepository / PaymentRepository**
- Basic CRUD operations
- `GetByUserIdAsync()` / `GetByCourseIdAsync()` - Payments by entity
- `GetByTransactionIdAsync()` - Find by transaction ID
- `GetByStatusAsync()` - Filter by payment status
- `GetCompletedPaymentsAsync()` / `GetRefundedPaymentsAsync()`
- `GetTotalRevenueAsync()` - Platform revenue
- `GetRevenueByInstructorAsync()` / `GetRevenueByCourseAsync()`
- `HasUserPurchasedCourseAsync()` - Access control

#### **IReviewRepository / ReviewRepository**
- Basic CRUD operations
- `GetByCourseIdAsync()` / `GetByUserIdAsync()` - Reviews by entity
- `GetByUserAndCourseAsync()` - Specific review
- `GetApprovedReviewsAsync()` - Public reviews
- `GetPendingReviewsAsync()` - Moderation queue
- `GetAverageRatingAsync()` / `GetReviewCountAsync()` - Course metrics
- `GetReviewsByRatingAsync()` - Filter by rating
- `HasUserReviewedCourseAsync()` - Prevent duplicates

## ?? **Usage Examples**

### **In Command/Query Handlers:**

```csharp
// Using specific repository methods
public class GetCourseHandler : IRequestHandler<GetCourseQuery, Course?>
{
    private readonly ICourseRepository _courseRepository;
    
    public GetCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    
    public async Task<Course?> Handle(GetCourseQuery request, CancellationToken cancellationToken)
    {
        return await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}

// Using multiple repositories
public class EnrollUserHandler : IRequestHandler<EnrollUserCommand, Enrollment>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IPaymentRepository _paymentRepository;
    
    // ... implementation
}
```

### **In Business Logic:**

```csharp
// Check if user can enroll
var isAlreadyEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(userId, courseId);
var hasPurchased = await _paymentRepository.HasUserPurchasedCourseAsync(userId, courseId);
var enrollmentCount = await _courseRepository.GetEnrollmentCountAsync(courseId);

if (!isAlreadyEnrolled && (hasPurchased || course.Price == 0))
{
    // Allow enrollment
}
```

## ?? **Benefits of This Architecture**

1. **?? Clean Separation**: Each feature has its own data access layer
2. **?? Reusable Base**: Common CRUD operations inherited from BaseRepository
3. **?? Domain-Specific**: Feature-specific methods for business logic
4. **?? Testable**: Easy to mock repositories for unit testing
5. **? Performance**: Direct control over queries and optimizations
6. **??? Maintainable**: Clear organization following vertical slice architecture
7. **?? Scalable**: Easy to add new repositories as features grow

## ?? **Next Steps**

1. **Register in DI**: Add `services.AddRepositories()` to Program.cs
2. **Update Handlers**: Refactor existing handlers to use repositories
3. **Add Caching**: Implement caching in repositories for performance
4. **Add Logging**: Add structured logging in repository methods
5. **Unit Tests**: Create comprehensive unit tests for each repository

All repositories are now created and ready to use! ??