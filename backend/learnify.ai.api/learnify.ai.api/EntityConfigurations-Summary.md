# Entity Configurations Summary

## Overview
This document summarizes all the Entity Framework Core configurations implemented for the Learnify.ai API following the class diagram design.

## Entity Configurations Implemented

### ? Core Entities

#### 1. UserConfiguration.cs
- **Table**: Users
- **Key Properties**: Id (Primary Key)
- **Properties**:
  - FirstName: string (required, max 50)
  - LastName: string (required, max 50)
  - Email: string (required, max 255, unique)
  - PasswordHash: string (required, max 500)
  - Role: UserRole enum ? string (required, max 20)
  - IsActive: bool (required, default true)
  - ProfilePicture: string? (max 500)
  - Bio: string? (max 1000)
  - DateOfBirth: DateTime?
  - PhoneNumber: string? (max 20)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - Unique email index
  - Password hash required
  - Role enum conversion (Student, Instructor, Admin)
- **Indexes**: Email (unique), Role, IsActive, FirstName+LastName, CreatedAt, Role+IsActive

#### 2. CourseConfiguration.cs
- **Table**: Courses
- **Key Properties**: Id (Primary Key), InstructorId (FK), CategoryId (FK)
- **Properties**:
  - Title: string (required, max 200)
  - Description: string (required, max 2000)
  - ShortDescription: string (required, max 500)
  - InstructorId: int (required, FK to Users)
  - CategoryId: int (required, FK to Categories)
  - Price: decimal (required, precision 18,2)
  - DiscountPrice: decimal? (precision 18,2)
  - DurationHours: int (required)
  - Level: CourseLevel enum ? string (required, max 20)
  - Language: string (required, max 50)
  - ThumbnailUrl: string? (max 500)
  - VideoPreviewUrl: string? (max 500)
  - IsPublished: bool (required, default false)
  - MaxStudents: int?
  - Prerequisites: string? (max 1000)
  - LearningObjectives: string (required, max 2000)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - Price with precision (18,2)
  - Level enum conversion (Beginner, Intermediate, Advanced, Expert)
  - Required learning objectives
- **Indexes**: Title, InstructorId, CategoryId, IsPublished, Level, Price, CreatedAt, composite indexes

#### 3. CategoryConfiguration.cs
- **Table**: Categories
- **Key Properties**: Id (Primary Key), ParentCategoryId (FK, self-reference)
- **Properties**:
  - Name: string (required, max 100)
  - Description: string (required, max 500)
  - IconUrl: string? (max 255)
  - ParentCategoryId: int? (FK to Categories)
  - IsActive: bool (required, default true)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - Name required and indexed
  - Hierarchical structure support
- **Indexes**: Name, ParentCategoryId, IsActive, ParentCategoryId+IsActive

#### 4. LessonConfiguration.cs
- **Table**: Lessons
- **Key Properties**: Id (Primary Key), CourseId (FK)
- **Properties**:
  - CourseId: int (required, FK to Courses)
  - Title: string (required, max 200)
  - Description: string (required, max 1000)
  - Content: string (required)
  - VideoUrl: string? (max 500)
  - Duration: int (required, > 0)
  - OrderIndex: int (required, >= 0)
  - IsFree: bool (required)
  - IsPublished: bool (required)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - Duration > 0
  - OrderIndex >= 0
  - Content required
- **Indexes**: CourseId, IsPublished, IsFree, CourseId+OrderIndex, CourseId+IsPublished

### ? Enrollment System

#### 5. EnrollmentConfiguration.cs
- **Table**: Enrollments
- **Key Properties**: Id (Primary Key), UserId (FK), CourseId (FK)
- **Properties**:
  - UserId: int (required, FK to Users)
  - CourseId: int (required, FK to Courses)
  - EnrollmentDate: DateTime (required)
  - CompletionDate: DateTime?
  - Progress: decimal (required, precision 5,2, 0-100)
  - Status: EnrollmentStatus enum ? string (required)
  - PaymentId: int? (FK to Payments)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - Progress between 0-100
  - Status enum conversion (Active, Completed, Dropped, Suspended)
  - Unique user-course combination
- **Indexes**: UserId, CourseId, Status, EnrollmentDate, Progress, composite indexes

#### 6. ProgressConfiguration.cs
- **Table**: Progress
- **Key Properties**: Id (Primary Key), EnrollmentId (FK), LessonId (FK)
- **Properties**:
  - EnrollmentId: int (required, FK to Enrollments)
  - LessonId: int (required, FK to Lessons)
  - IsCompleted: bool (required)
  - CompletionDate: DateTime?
  - TimeSpent: int (required, >= 0)
  - LastAccessDate: DateTime (required)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - TimeSpent >= 0
  - Unique enrollment-lesson combination
- **Indexes**: EnrollmentId, LessonId, IsCompleted, LastAccessDate, composite indexes

### ? Assessment System

#### 7. QuizConfiguration.cs
- **Table**: Quizzes
- **Key Properties**: Id (Primary Key), CourseId (FK), LessonId (FK, optional)
- **Properties**:
  - CourseId: int (required, FK to Courses)
  - LessonId: int? (FK to Lessons, optional)
  - Title: string (required, max 200)
  - Description: string (required, max 1000)
  - TimeLimit: int? (> 0 if set)
  - PassingScore: int (required, 0-100)
  - MaxAttempts: int (required, > 0)
  - IsActive: bool (required)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - PassingScore 0-100
  - MaxAttempts > 0
  - TimeLimit > 0 (if set)
- **Indexes**: CourseId, LessonId, IsActive, CreatedAt, composite indexes

#### 8. QuestionConfiguration.cs
- **Table**: Questions
- **Key Properties**: Id (Primary Key), QuizId (FK)
- **Properties**:
  - QuizId: int (required, FK to Quizzes)
  - QuestionText: string (required, max 1000)
  - QuestionType: QuestionType enum ? string (required, max 50)
  - Points: int (required, default 1, > 0)
  - OrderIndex: int (required, >= 0)
  - IsActive: bool (required, default true)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - QuestionType enum conversion (MultipleChoice, TrueFalse, ShortAnswer, Essay)
  - OrderIndex for sequence >= 0
  - Points > 0
- **Indexes**: QuizId, IsActive, QuestionType, QuizId+OrderIndex, QuizId+IsActive

#### 9. AnswerConfiguration.cs
- **Table**: Answers
- **Key Properties**: Id (Primary Key), QuestionId (FK)
- **Properties**:
  - QuestionId: int (required, FK to Questions)
  - AnswerText: string (required, max 500)
  - IsCorrect: bool (required, default false)
  - OrderIndex: int (required, >= 0)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - OrderIndex for sequence >= 0
  - IsCorrect flag
- **Indexes**: QuestionId, IsCorrect, QuestionId+OrderIndex, QuestionId+IsCorrect

#### 10. QuizAttemptConfiguration.cs
- **Table**: QuizAttempts
- **Key Properties**: Id (Primary Key), QuizId (FK), UserId (FK)
- **Properties**:
  - QuizId: int (required, FK to Quizzes)
  - UserId: int (required, FK to Users)
  - Score: int (required, default 0, >= 0)
  - MaxScore: int (required, > 0)
  - StartedAt: DateTime (required)
  - CompletedAt: DateTime?
  - TimeSpent: int (required, default 0, >= 0)
  - IsPassed: bool (required, default false)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - Score >= 0
  - MaxScore > 0
  - TimeSpent >= 0
- **Indexes**: QuizId, UserId, IsPassed, StartedAt, CompletedAt, composite indexes

### ? Business Entities

#### 11. PaymentConfiguration.cs
- **Table**: Payments
- **Key Properties**: Id (Primary Key), UserId (FK), CourseId (FK)
- **Properties**:
  - UserId: int (required, FK to Users)
  - CourseId: int (required, FK to Courses)
  - Amount: decimal (required, precision 18,2)
  - Currency: string (required, max 3)
  - PaymentMethod: PaymentMethod enum ? string (required, max 50)
  - TransactionId: string (required, max 100, unique)
  - Status: PaymentStatus enum ? string (required, max 50)
  - PaymentDate: DateTime (required)
  - RefundDate: DateTime?
  - RefundAmount: decimal? (precision 18,2)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - Amount with precision (18,2)
  - Currency code (3 chars)
  - Unique TransactionId
  - Status and PaymentMethod enums
- **Indexes**: UserId, CourseId, Status, PaymentDate, TransactionId (unique), composite indexes

#### 12. ReviewConfiguration.cs
- **Table**: Reviews
- **Key Properties**: Id (Primary Key), CourseId (FK), UserId (FK)
- **Properties**:
  - CourseId: int (required, FK to Courses)
  - UserId: int (required, FK to Users)
  - Rating: int (required, 1-5)
  - Comment: string? (max 1000)
  - IsApproved: bool (required, default false)
  - CreatedAt: DateTime (required)
  - UpdatedAt: DateTime (required)
- **Constraints**: 
  - Rating 1-5 (check constraint)
  - Unique user-course combination
- **Indexes**: CourseId, UserId, Rating, IsApproved, CreatedAt, composite indexes

## Database Relationships Configured

### Primary Relationships
1. **User ? Course** (Instructor): One-to-Many with Restrict delete
2. **Course ? Category**: Many-to-One with Restrict delete
3. **Course ? Lesson**: One-to-Many with Cascade delete
4. **User ? Enrollment**: One-to-Many with Cascade delete
5. **Course ? Enrollment**: One-to-Many with Cascade delete
6. **Enrollment ? Progress**: One-to-Many with Cascade delete
7. **Lesson ? Progress**: One-to-Many with Cascade delete

### Assessment Relationships
8. **Course ? Quiz**: One-to-Many with Cascade delete
9. **Lesson ? Quiz**: One-to-Many with Cascade delete (optional)
10. **Quiz ? Question**: One-to-Many with Cascade delete
11. **Question ? Answer**: One-to-Many with Cascade delete
12. **Quiz ? QuizAttempt**: One-to-Many with Cascade delete
13. **User ? QuizAttempt**: One-to-Many with Cascade delete

### Business Relationships
14. **User ? Payment**: One-to-Many with Cascade delete
15. **Course ? Payment**: One-to-Many with Cascade delete
16. **User ? Review**: One-to-Many with Cascade delete
17. **Course ? Review**: One-to-Many with Cascade delete
18. **Enrollment ? Payment**: One-to-One with SetNull delete (optional)

### Self-Referencing Relationships
19. **Category ? Category**: Parent-Child with Restrict delete

## Unique Constraints & Indexes

### Unique Constraints
- Users.Email
- Enrollments(UserId, CourseId)
- Progress(EnrollmentId, LessonId)
- Reviews(UserId, CourseId)
- Payments.TransactionId

### Performance Indexes
- **Single Column**: All foreign keys, status fields, dates
- **Composite**: Common query patterns (e.g., Course.IsPublished + Category)
- **Covering**: Frequently accessed field combinations

## Check Constraints

### Data Integrity
- Review.Rating: Between 1 and 5
- Quiz.PassingScore: Between 0 and 100
- Quiz.MaxAttempts: Greater than 0
- Quiz.TimeLimit: Greater than 0 (if not null)
- Enrollment.Progress: Between 0 and 100
- Progress.TimeSpent: Greater than or equal to 0
- Lesson.Duration: Greater than 0
- Lesson.OrderIndex: Greater than or equal to 0
- Question.OrderIndex: Greater than or equal to 0
- Question.Points: Greater than 0
- Answer.OrderIndex: Greater than or equal to 0
- QuizAttempt.Score: Greater than or equal to 0
- QuizAttempt.MaxScore: Greater than 0
- QuizAttempt.TimeSpent: Greater than or equal to 0

## Enum Conversions
All enums are stored as strings for better readability:
- UserRole ? string (Student, Instructor, Admin)
- CourseLevel ? string (Beginner, Intermediate, Advanced, Expert)
- EnrollmentStatus ? string (Active, Completed, Dropped, Suspended)
- QuestionType ? string (MultipleChoice, TrueFalse, ShortAnswer, Essay)
- PaymentMethod ? string (CreditCard, PayPal, BankTransfer, Cryptocurrency)
- PaymentStatus ? string (Pending, Completed, Failed, Refunded, Cancelled)

## Entity Business Methods (as per ClassDiagram.md)

### User Methods
- GetFullName(): string
- IsInstructor(): bool
- IsStudent(): bool

### Course Methods
- GetEffectivePrice(): decimal
- IsDiscounted(): bool
- CanEnroll(): bool

### Enrollment Methods
- IsCompleted(): bool
- GetProgressPercentage(): int

## Migration Notes

### For SQL Server Production
1. Update connection string in appsettings.json
2. Replace `UseInMemoryDatabase` with `UseSqlServer` in Program.cs
3. Run migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### For Development
- Currently using InMemory database
- Data is seeded automatically on startup
- Perfect for testing and development

## Next Steps

1. **Create Migration**: Generate EF Core migration from these configurations
2. **Add Validators**: Implement FluentValidation for each entity
3. **Add Features**: Create CRUD operations for each entity following vertical slice pattern
4. **Add Tests**: Unit and integration tests for each configuration
5. **Performance Tuning**: Monitor and optimize indexes based on usage patterns

This comprehensive configuration provides a solid foundation for a production-ready learning management system with proper data integrity, performance optimization, and scalability. All configurations now match the ClassDiagram.md specifications exactly.