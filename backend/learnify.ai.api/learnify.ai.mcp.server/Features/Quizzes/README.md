# ?? Quiz Management MCP Feature

## Overview

The Quiz Management MCP feature provides comprehensive quiz and assessment management capabilities for the Learnify.ai platform through a Model Context Protocol (MCP) server. This feature exposes all quiz-related API endpoints as structured tools that AI assistants can use to manage quizzes, questions, quiz attempts, and assessment analytics.

## Architecture

### Vertical Slice Structure

```
Features/Quizzes/
??? Models/
?   ??? QuizModels.cs            # Quiz domain models and DTOs
??? Services/
?   ??? QuizApiService.cs        # MCP tools with API integration
??? QuizFeature.cs              # Feature registration
```

### Key Components

- **QuizApiService**: MCP tool provider with `[McpServerToolType]` attribute
- **QuizModels**: Domain models, request/response DTOs, and assessment enums
- **QuizFeature**: Dependency injection registration

## Available MCP Tools (18 Total)

### ?? **Quiz CRUD Operations**

#### `GetQuizzesAsync`
**Description**: Get all quizzes with optional filtering and pagination  
**Parameters**:
- `courseId` (optional): Filter by course ID
- `lessonId` (optional): Filter by lesson ID
- `isActive` (optional): Filter by active status
- `searchTerm` (optional): Search in title/description
- `page`/`pageSize`: Pagination (default: page=1, pageSize=10)

#### `GetQuizAsync`
**Description**: Get quiz details by ID  
**Parameters**: 
- `quizId`: The quiz ID

#### `CreateQuizAsync`
**Description**: Create a new quiz  
**Required Parameters**:
- `title`: Quiz title
- `description`: Quiz description
- `passingScore`: Passing score (0-100)

**Optional Parameters**:
- `courseId`: Associated course (optional)
- `lessonId`: Associated lesson (optional)
- `timeLimit`: Time limit in minutes
- `maxAttempts`: Maximum attempts (default: 3)
- `isActive`: Active status (default: false)

#### `UpdateQuizAsync`
**Description**: Update existing quiz details  
**Parameters**:
- `quizId`: The quiz ID to update
- All quiz fields (optional): title, description, timeLimit, passingScore, maxAttempts, isActive

#### `DeleteQuizAsync`
**Description**: Delete a quiz permanently  
**Parameters**:
- `quizId`: The quiz ID to delete

### ?? **Quiz Activation Operations**

#### `ActivateQuizAsync`
**Description**: Activate a quiz to make it available to students  
**Parameters**:
- `quizId`: The quiz ID to activate

#### `DeactivateQuizAsync`
**Description**: Deactivate a quiz to make it unavailable to students  
**Parameters**:
- `quizId`: The quiz ID to deactivate

### ?? **Course and Lesson Integration**

#### `GetCourseQuizzesAsync`
**Description**: Get all quizzes for a specific course  
**Parameters**:
- `courseId`: The course ID

#### `GetLessonQuizzesAsync`
**Description**: Get all quizzes for a specific lesson  
**Parameters**:
- `lessonId`: The lesson ID

### ? **Question Management**

#### `GetQuizQuestionsAsync`
**Description**: Get all questions for a specific quiz  
**Parameters**:
- `quizId`: The quiz ID

#### `AddQuestionToQuizAsync`
**Description**: Add a new question to a quiz  
**Parameters**:
- `quizId`: The quiz ID
- `questionText`: The question text
- `questionType`: Question type (1=MultipleChoice, 2=TrueFalse, 3=ShortAnswer, 4=Essay, 5=FillInTheBlank)
- `points`: Points for this question (default: 1)
- `orderIndex`: Order index (optional)

### ?? **Quiz Attempts Management**

#### `StartQuizAttemptAsync`
**Description**: Start a quiz attempt for a specific user  
**Parameters**:
- `quizId`: The quiz ID
- `userId`: The user ID

#### `GetQuizAttemptsAsync`
**Description**: Get all attempts for a specific quiz  
**Parameters**:
- `quizId`: The quiz ID
- `page`/`pageSize`: Pagination (default: page=1, pageSize=10)

#### `GetQuizStatsAsync`
**Description**: Get comprehensive statistics for a quiz  
**Parameters**:
- `quizId`: The quiz ID

**Returns**: Attempt counts, scores, pass rates, completion times, etc.

### ?? **Utility Operations**

#### `CheckQuizExistsAsync`
**Description**: Check if a quiz exists  
**Parameters**:
- `quizId`: The quiz ID to check

#### `GetQuizSummaryAsync`
**Description**: Get quiz summary (basic information only)  
**Parameters**:
- `quizId`: The quiz ID

## Data Models

### Quiz Model
```csharp
public record QuizModel(
    int Id,
    int? CourseId,
    string? CourseTitle,
    int? LessonId,
    string? LessonTitle,
    string Title,
    string Description,
    int? TimeLimit,
    decimal PassingScore,
    int MaxAttempts,
    bool IsActive,
    int QuestionCount,
    int TotalPoints,
    int AttemptCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

### Question Types
```csharp
public enum QuestionType
{
    MultipleChoice = 1,    // Multiple choice with single correct answer
    TrueFalse = 2,        // True/False questions
    ShortAnswer = 3,      // Short text answers
    Essay = 4,            // Long-form essay responses
    FillInTheBlank = 5    // Fill-in-the-blank questions
}
```

### Quiz Statistics Model
```csharp
public record QuizStatsModel(
    int QuizId,
    string QuizTitle,
    int TotalAttempts,
    int CompletedAttempts,
    int PassedAttempts,
    decimal AverageScore,
    decimal PassRate,
    int UniqueParticipants,
    TimeSpan AverageCompletionTime,
    DateTime LastAttempt
);
```

### Response Formats

#### Success Response
```json
{
  "success": true,
  "data": { /* quiz data */ },
  "message": "Operation completed successfully"
}
```

#### Error Response
```json
{
  "success": false,
  "message": "Detailed error message"
}
```

## Usage Examples

### Basic Quiz Operations

**Get all quizzes:**
```
Tool: GetQuizzesAsync
Parameters: { "page": 1, "pageSize": 10 }
```

**Get active quizzes for a course:**
```
Tool: GetQuizzesAsync
Parameters: { "courseId": 1, "isActive": true }
```

**Create a new quiz:**
```
Tool: CreateQuizAsync
Parameters: {
  "title": "JavaScript Fundamentals Quiz",
  "description": "Test your knowledge of JavaScript basics",
  "passingScore": 70,
  "courseId": 1,
  "timeLimit": 30,
  "maxAttempts": 3,
  "isActive": false
}
```

**Update quiz settings:**
```
Tool: UpdateQuizAsync
Parameters: {
  "quizId": 1,
  "passingScore": 75,
  "timeLimit": 45,
  "isActive": true
}
```

### Quiz Management

**Activate a quiz:**
```
Tool: ActivateQuizAsync
Parameters: { "quizId": 1 }
```

**Get course quizzes:**
```
Tool: GetCourseQuizzesAsync
Parameters: { "courseId": 1 }
```

**Get lesson quizzes:**
```
Tool: GetLessonQuizzesAsync
Parameters: { "lessonId": 5 }
```

### Question Management

**Get quiz questions:**
```
Tool: GetQuizQuestionsAsync
Parameters: { "quizId": 1 }
```

**Add multiple choice question:**
```
Tool: AddQuestionToQuizAsync
Parameters: {
  "quizId": 1,
  "questionText": "What is the correct syntax for creating a variable in JavaScript?",
  "questionType": 1,
  "points": 2
}
```

**Add true/false question:**
```
Tool: AddQuestionToQuizAsync
Parameters: {
  "quizId": 1,
  "questionText": "JavaScript is a compiled language.",
  "questionType": 2,
  "points": 1
}
```

### Assessment Management

**Start quiz attempt:**
```
Tool: StartQuizAttemptAsync
Parameters: {
  "quizId": 1,
  "userId": 10
}
```

**Get quiz attempts:**
```
Tool: GetQuizAttemptsAsync
Parameters: {
  "quizId": 1,
  "page": 1,
  "pageSize": 20
}
```

**Get quiz statistics:**
```
Tool: GetQuizStatsAsync
Parameters: { "quizId": 1 }
```

### Search and Discovery

**Search quizzes:**
```
Tool: GetQuizzesAsync
Parameters: {
  "searchTerm": "javascript",
  "isActive": true,
  "page": 1,
  "pageSize": 15
}
```

**Find quizzes by lesson:**
```
Tool: GetQuizzesAsync
Parameters: {
  "lessonId": 3,
  "isActive": true
}
```

## API Endpoint Mapping

The MCP tools map directly to the Learnify.ai API endpoints:

| MCP Tool | HTTP Method | API Endpoint |
|----------|-------------|--------------|
| `GetQuizzesAsync` | GET | `/api/quizzes` |
| `GetQuizAsync` | GET | `/api/quizzes/{id}` |
| `CreateQuizAsync` | POST | `/api/quizzes` |
| `UpdateQuizAsync` | PUT | `/api/quizzes/{id}` |
| `DeleteQuizAsync` | DELETE | `/api/quizzes/{id}` |
| `ActivateQuizAsync` | PUT | `/api/quizzes/{id}/activate` |
| `DeactivateQuizAsync` | PUT | `/api/quizzes/{id}/deactivate` |
| `GetCourseQuizzesAsync` | GET | `/api/quizzes/courses/{courseId}/quizzes` |
| `GetLessonQuizzesAsync` | GET | `/api/quizzes/lessons/{lessonId}/quizzes` |
| `GetQuizQuestionsAsync` | GET | `/api/quizzes/{id}/questions` |
| `AddQuestionToQuizAsync` | POST | `/api/quizzes/{id}/questions` |
| `StartQuizAttemptAsync` | POST | `/api/quizzes/{id}/start` |
| `GetQuizAttemptsAsync` | GET | `/api/quizzes/{id}/attempts` |
| `GetQuizStatsAsync` | GET | `/api/quizzes/{id}/stats` |

## Integration

### Prerequisites
- Learnify.ai API running on `http://localhost:5271`
- .NET 8 SDK
- Quiz feature registered in `Program.cs`

### Configuration
```csharp
// Program.cs
builder.Services.AddQuizFeature();
```

### Error Handling
All tools implement comprehensive error handling:
- Input validation
- API communication errors
- Not found scenarios
- Quiz state validation (active/inactive)
- Attempt limits and permissions
- Permission/authorization errors

## Assessment Best Practices

### Quiz Design
- **Clear Instructions**: Provide detailed quiz descriptions
- **Appropriate Timing**: Set realistic time limits
- **Fair Scoring**: Set reasonable passing scores (60-80%)
- **Multiple Attempts**: Allow 2-3 attempts for learning

### Question Management
- **Varied Question Types**: Mix multiple choice, true/false, and short answer
- **Point Distribution**: Assign points based on question difficulty
- **Logical Order**: Arrange questions from simple to complex
- **Clear Language**: Write unambiguous question text

### Assessment Analytics
- **Monitor Performance**: Regular review of quiz statistics
- **Identify Patterns**: Look for consistently failed questions
- **Adjust Difficulty**: Modify based on pass rates
- **Student Feedback**: Consider completion times and attempt patterns

### Security Considerations
- **Activate When Ready**: Only activate completed quizzes
- **Time Limits**: Use appropriate time constraints
- **Attempt Limits**: Prevent unlimited retries
- **Question Pools**: Randomize questions when possible

## Testing Commands

Test the quiz MCP tools with Claude:

### **Basic Operations:**
1. **"What quiz management tools are available?"**
2. **"Get all active quizzes"**
3. **"Create a new quiz for JavaScript fundamentals"**
4. **"Get quizzes for course 1"**

### **Question Management:**
1. **"Get questions for quiz 1"**
2. **"Add a multiple choice question about variables"**
3. **"Add a true/false question about JavaScript compilation"**

### **Assessment Operations:**
1. **"Start a quiz attempt for user 5 on quiz 1"**
2. **"Get all attempts for quiz 1"**
3. **"Get statistics for quiz 2"**
4. **"Activate quiz 3"**

### **Advanced Operations:**
1. **"Create a comprehensive quiz with 10 questions"**
2. **"Find all quizzes with low pass rates"**
3. **"Get performance analytics across all course quizzes"**

## Quiz Workflow Example

### **Complete Quiz Creation Process:**
1. **Create Quiz**: Use `CreateQuizAsync` with basic settings
2. **Add Questions**: Use `AddQuestionToQuizAsync` multiple times
3. **Review Content**: Use `GetQuizQuestionsAsync` to verify
4. **Activate Quiz**: Use `ActivateQuizAsync` when ready
5. **Monitor Performance**: Use `GetQuizStatsAsync` for analytics

### **Student Assessment Workflow:**
1. **Start Attempt**: Use `StartQuizAttemptAsync`
2. **Monitor Progress**: Track through attempt system
3. **Review Results**: Use `GetQuizAttemptsAsync`
4. **Analyze Performance**: Use `GetQuizStatsAsync`

## Integration with Other Features

### **Course Integration:**
- **Course Quizzes**: `GetCourseQuizzesAsync` shows all assessments in a course
- **Course Analytics**: Combined with course statistics for comprehensive insights

### **Lesson Integration:**
- **Lesson Quizzes**: `GetLessonQuizzesAsync` shows lesson-specific assessments
- **Learning Flow**: Quizzes can be used to gate lesson progression

### **Category Integration:**
- **Category Analytics**: Quiz performance can be analyzed by category
- **Subject Assessment**: Track learning outcomes across subject areas

## Performance Monitoring

### **Key Metrics to Track:**
- **Pass Rate**: Percentage of students passing quizzes
- **Average Score**: Overall performance across attempts
- **Completion Time**: How long students take on average
- **Question Performance**: Which questions are most/least difficult
- **Retake Patterns**: How many attempts students typically need

### **Analytics Tools:**
- **Quiz Statistics**: Comprehensive performance data
- **Attempt Analysis**: Individual and aggregate attempt data
- **Question Analytics**: Performance by question type and difficulty
- **Trend Analysis**: Performance over time

The Quiz MCP feature provides comprehensive assessment management capabilities while maintaining consistency with the lesson, course, and category features! ??