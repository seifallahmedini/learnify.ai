# Learnify.ai API - Class Diagram and Domain Model

## Class Diagram (PlantUML)

```plantuml
@startuml Learnify_API_Class_Diagram

!define ENTITY class
!define VALUE_OBJECT class
!define ENUM enum

package "User Management" {
    ENTITY User {
        +Id: int
        +FirstName: string
        +LastName: string
        +Email: string
        +PasswordHash: string
        +Role: UserRole
        +IsActive: bool
        +ProfilePicture: string?
        +Bio: string?
        +DateOfBirth: DateTime?
        +PhoneNumber: string?
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
        --
        +GetFullName(): string
        +IsInstructor(): bool
        +IsStudent(): bool
    }

    ENUM UserRole {
        Student
        Instructor
        Admin
    }
}

package "Course Management" {
    ENTITY Course {
        +Id: int
        +Title: string
        +Description: string
        +ShortDescription: string
        +InstructorId: int
        +CategoryId: int
        +Price: decimal
        +DiscountPrice: decimal?
        +DurationHours: int
        +Level: CourseLevel
        +Language: string
        +ThumbnailUrl: string?
        +VideoPreviewUrl: string?
        +IsPublished: bool
        +MaxStudents: int?
        +Prerequisites: string?
        +LearningObjectives: string
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
        --
        +GetEffectivePrice(): decimal
        +IsDiscounted(): bool
        +CanEnroll(): bool
    }

    ENTITY Category {
        +Id: int
        +Name: string
        +Description: string
        +IconUrl: string?
        +ParentCategoryId: int?
        +IsActive: bool
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }

    ENTITY Lesson {
        +Id: int
        +CourseId: int
        +Title: string
        +Description: string
        +Content: string
        +VideoUrl: string?
        +Duration: int
        +OrderIndex: int
        +IsFree: bool
        +IsPublished: bool
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }

    ENUM CourseLevel {
        Beginner
        Intermediate
        Advanced
        Expert
    }
}

package "Enrollment Management" {
    ENTITY Enrollment {
        +Id: int
        +UserId: int
        +CourseId: int
        +EnrollmentDate: DateTime
        +CompletionDate: DateTime?
        +Progress: decimal
        +Status: EnrollmentStatus
        +PaymentId: int?
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
        --
        +IsCompleted(): bool
        +GetProgressPercentage(): int
    }

    ENTITY Progress {
        +Id: int
        +EnrollmentId: int
        +LessonId: int
        +IsCompleted: bool
        +CompletionDate: DateTime?
        +TimeSpent: int
        +LastAccessDate: DateTime
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }

    ENUM EnrollmentStatus {
        Active
        Completed
        Dropped
        Suspended
    }
}

package "Assessment Management" {
    ENTITY Quiz {
        +Id: int
        +CourseId: int
        +LessonId: int?
        +Title: string
        +Description: string
        +TimeLimit: int?
        +PassingScore: int
        +MaxAttempts: int
        +IsActive: bool
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }

    ENTITY Question {
        +Id: int
        +QuizId: int
        +QuestionText: string
        +QuestionType: QuestionType
        +Points: int
        +OrderIndex: int
        +IsActive: bool
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }

    ENTITY Answer {
        +Id: int
        +QuestionId: int
        +AnswerText: string
        +IsCorrect: bool
        +OrderIndex: int
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }

    ENTITY QuizAttempt {
        +Id: int
        +QuizId: int
        +UserId: int
        +Score: int
        +MaxScore: int
        +StartedAt: DateTime
        +CompletedAt: DateTime?
        +TimeSpent: int
        +IsPassed: bool
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }

    ENUM QuestionType {
        MultipleChoice
        TrueFalse
        ShortAnswer
        Essay
    }
}

package "Payment Management" {
    ENTITY Payment {
        +Id: int
        +UserId: int
        +CourseId: int
        +Amount: decimal
        +Currency: string
        +PaymentMethod: PaymentMethod
        +TransactionId: string
        +Status: PaymentStatus
        +PaymentDate: DateTime
        +RefundDate: DateTime?
        +RefundAmount: decimal?
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }

    ENUM PaymentMethod {
        CreditCard
        PayPal
        BankTransfer
        Cryptocurrency
    }

    ENUM PaymentStatus {
        Pending
        Completed
        Failed
        Refunded
        Cancelled
    }
}

package "Review Management" {
    ENTITY Review {
        +Id: int
        +CourseId: int
        +UserId: int
        +Rating: int
        +Comment: string?
        +IsApproved: bool
        +CreatedAt: DateTime
        +UpdatedAt: DateTime
    }
}

' Relationships
User ||--o{ Enrollment : "enrolls in"
User ||--o{ Course : "instructs"
User ||--o{ Review : "writes"
User ||--o{ QuizAttempt : "attempts"
User ||--o{ Payment : "makes"

Course ||--o{ Enrollment : "has"
Course ||--o{ Lesson : "contains"
Course ||--o{ Quiz : "includes"
Course ||--o{ Review : "receives"
Course }o--|| Category : "belongs to"
Course }o--|| User : "taught by"

Category ||--o{ Category : "parent/child"

Enrollment ||--o{ Progress : "tracks"
Enrollment ||--o{ Payment : "paid by"

Lesson ||--o{ Progress : "tracked in"
Lesson ||--o{ Quiz : "may have"

Quiz ||--o{ Question : "contains"
Quiz ||--o{ QuizAttempt : "attempted as"

Question ||--o{ Answer : "has options"

@enduml
```

## Domain Model Overview

### Core Entities

1. **User**: Represents all users (students, instructors, admins)
2. **Course**: Main learning content with metadata
3. **Category**: Course categorization and organization
4. **Lesson**: Individual learning units within courses
5. **Enrollment**: User-course relationship tracking
6. **Progress**: Detailed progress tracking per lesson
7. **Quiz/Question/Answer**: Assessment system
8. **QuizAttempt**: User quiz attempt tracking
9. **Payment**: Financial transactions
10. **Review**: Course feedback and ratings

### Key Relationships

- **User ? Course**: Many-to-many through Enrollment
- **User ? Course**: One-to-many (Instructor relationship)
- **Course ? Lesson**: One-to-many composition
- **Course ? Category**: Many-to-one
- **Enrollment ? Progress**: One-to-many
- **Quiz ? Question ? Answer**: Hierarchical structure

### Business Rules

1. **Enrollment**: Users can enroll in multiple courses
2. **Progress**: Tracked per lesson per enrollment
3. **Payments**: Required for paid courses
4. **Reviews**: Only enrolled users can review
5. **Quizzes**: Can be course-level or lesson-specific
6. **Categories**: Support hierarchical structure

## Implementation Strategy

1. **Phase 1**: Extend existing User and Course models
2. **Phase 2**: Add core relationship entities (Enrollment, Category)
3. **Phase 3**: Implement assessment system
4. **Phase 4**: Add payment and review systems

This diagram provides a comprehensive foundation for a learning management system that can scale with your business needs.