# ?? Course Management MCP Feature

## Overview

The Course Management MCP feature provides comprehensive course management capabilities for the Learnify.ai platform through a Model Context Protocol (MCP) server. This feature exposes all course-related API endpoints as structured tools that AI assistants can use to manage courses, enrollments, and course metadata.

## Architecture

### Vertical Slice Structure

```
Features/Courses/
??? Models/
?   ??? CourseModels.cs          # Course domain models and DTOs
??? Services/
?   ??? CourseApiService.cs      # MCP tools with API integration
??? CourseFeature.cs            # Feature registration
```

### Key Components

- **CourseApiService**: MCP tool provider with `[McpServerToolType]` attribute
- **CourseModels**: Domain models, request/response DTOs, and enums
- **CourseFeature**: Dependency injection registration

## Available MCP Tools (15 Total)

### ?? **Course CRUD Operations**

#### `GetCoursesAsync`
**Description**: Get all courses with optional filtering and pagination  
**Parameters**:
- `categoryId` (optional): Filter by category ID
- `instructorId` (optional): Filter by instructor ID  
- `level` (optional): Course level (1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert)
- `isPublished` (optional): Filter by published status
- `isFeatured` (optional): Filter by featured status
- `minPrice`/`maxPrice` (optional): Price range filter
- `searchTerm` (optional): Search in title/description
- `page`/`pageSize`: Pagination (default: page=1, pageSize=10)

#### `GetCourseAsync`
**Description**: Get course details by ID  
**Parameters**: 
- `courseId`: The course ID

#### `CreateCourseAsync`
**Description**: Create a new course  
**Required Parameters**:
- `title`: Course title
- `description`: Course description
- `instructorId`: Instructor ID
- `categoryId`: Category ID
- `price`: Course price
- `durationHours`: Duration in hours
- `level`: Course level (1-4)
- `language`: Course language

**Optional Parameters**:
- `shortDescription`: Brief description
- `discountPrice`: Discounted price
- `thumbnailUrl`: Course thumbnail
- `videoPreviewUrl`: Preview video
- `isPublished`: Publish status (default: false)
- `isFeatured`: Featured status (default: false)
- `maxStudents`: Maximum enrollments
- `prerequisites`: Course prerequisites
- `learningObjectives`: Learning goals

#### `UpdateCourseAsync`
**Description**: Update existing course details  
**Parameters**:
- `courseId`: The course ID to update
- All course fields (optional): Same as CreateCourse but all optional

#### `DeleteCourseAsync`
**Description**: Delete a course permanently  
**Parameters**:
- `courseId`: The course ID to delete

### ?? **Publishing Operations**

#### `PublishCourseAsync`
**Description**: Publish a course to make it visible to students  
**Parameters**:
- `courseId`: The course ID to publish

#### `UnpublishCourseAsync`
**Description**: Unpublish a course to hide it from students  
**Parameters**:
- `courseId`: The course ID to unpublish

#### `FeatureCourseAsync`
**Description**: Feature a course to highlight it  
**Parameters**:
- `courseId`: The course ID to feature

#### `UnfeatureCourseAsync`
**Description**: Unfeature a course to remove highlighting  
**Parameters**:
- `courseId`: The course ID to unfeature

### ?? **Enrollment Operations**

#### `GetCourseEnrollmentsAsync`
**Description**: Get all enrollments for a specific course  
**Parameters**:
- `courseId`: The course ID
- `page`/`pageSize`: Pagination (default: page=1, pageSize=10)

#### `GetCourseStatsAsync`
**Description**: Get comprehensive statistics for a course  
**Parameters**:
- `courseId`: The course ID

**Returns**: Enrollment counts, ratings, revenue, lesson count, etc.

### ?? **Utility Operations**

#### `CheckCourseExistsAsync`
**Description**: Check if a course exists  
**Parameters**:
- `courseId`: The course ID to check

#### `GetCourseSummaryAsync`
**Description**: Get course summary (basic information only)  
**Parameters**:
- `courseId`: The course ID

## Data Models

### Course Model
```csharp
public record CourseModel(
    int Id,
    string Title,
    string Description,
    string? ShortDescription,
    int InstructorId,
    string InstructorName,
    int CategoryId,
    string CategoryName,
    decimal Price,
    decimal? DiscountPrice,
    int DurationHours,
    CourseLevel Level,
    string Language,
    string? ThumbnailUrl,
    string? VideoPreviewUrl,
    bool IsPublished,
    bool IsFeatured,
    int? MaxStudents,
    string? Prerequisites,
    string? LearningObjectives,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

### Course Levels
```csharp
public enum CourseLevel
{
    Beginner = 1,      // Entry-level courses
    Intermediate = 2,  // Moderate difficulty
    Advanced = 3,      // High difficulty
    Expert = 4         // Expert-level courses
}
```

### Response Formats

#### Success Response
```json
{
  "success": true,
  "data": { /* course data */ },
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

### Basic Course Operations

**Get all courses:**
```
Tool: GetCoursesAsync
Parameters: { "page": 1, "pageSize": 10 }
```

**Get courses by category:**
```
Tool: GetCoursesAsync
Parameters: { "categoryId": 2, "isPublished": true }
```

**Get featured courses:**
```
Tool: GetCoursesAsync
Parameters: { "isFeatured": true, "isPublished": true }
```

**Create a new course:**
```
Tool: CreateCourseAsync
Parameters: {
  "title": "Introduction to React",
  "description": "Learn React from scratch",
  "instructorId": 1,
  "categoryId": 2,
  "price": 99.99,
  "durationHours": 20,
  "level": 1,
  "language": "English",
  "isPublished": false
}
```

### Course Management

**Update course pricing:**
```
Tool: UpdateCourseAsync
Parameters: {
  "courseId": 1,
  "price": 79.99,
  "discountPrice": 59.99
}
```

**Publish a course:**
```
Tool: PublishCourseAsync
Parameters: { "courseId": 1 }
```

**Feature a course:**
```
Tool: FeatureCourseAsync
Parameters: { "courseId": 1 }
```

### Analytics and Monitoring

**Get course statistics:**
```
Tool: GetCourseStatsAsync
Parameters: { "courseId": 1 }
```

**Get course enrollments:**
```
Tool: GetCourseEnrollmentsAsync
Parameters: { "courseId": 1, "page": 1, "pageSize": 20 }
```

### Search and Filtering

**Search courses:**
```
Tool: GetCoursesAsync
Parameters: {
  "searchTerm": "javascript",
  "level": 1,
  "maxPrice": 100,
  "isPublished": true
}
```

**Get instructor's courses:**
```
Tool: GetCoursesAsync
Parameters: {
  "instructorId": 5,
  "isPublished": true
}
```

## API Endpoint Mapping

The MCP tools map directly to the Learnify.ai API endpoints:

| MCP Tool | HTTP Method | API Endpoint |
|----------|-------------|--------------|
| `GetCoursesAsync` | GET | `/api/courses` |
| `GetCourseAsync` | GET | `/api/courses/{id}` |
| `CreateCourseAsync` | POST | `/api/courses` |
| `UpdateCourseAsync` | PUT | `/api/courses/{id}` |
| `DeleteCourseAsync` | DELETE | `/api/courses/{id}` |
| `PublishCourseAsync` | PUT | `/api/courses/{id}/publish` |
| `UnpublishCourseAsync` | PUT | `/api/courses/{id}/unpublish` |
| `FeatureCourseAsync` | PUT | `/api/courses/{id}/feature` |
| `UnfeatureCourseAsync` | PUT | `/api/courses/{id}/unfeature` |
| `GetCourseEnrollmentsAsync` | GET | `/api/courses/{id}/enrollments` |
| `GetCourseStatsAsync` | GET | `/api/courses/{id}/stats` |

## Integration

### Prerequisites
- Learnify.ai API running on `http://localhost:5271`
- .NET 8 SDK
- Course feature registered in `Program.cs`

### Configuration
```csharp
// Program.cs
builder.Services.AddCourseFeature();
```

### Error Handling
All tools implement comprehensive error handling:
- Input validation
- API communication errors
- Not found scenarios
- Permission/authorization errors

## Testing Commands

Test the course MCP tools with Claude:

1. **"Get all published courses"**
2. **"Create a new beginner course for JavaScript"**
3. **"Get statistics for course 1"**
4. **"Find all courses by instructor 2"**
5. **"Publish course 3"**
6. **"Get featured courses under $50"**

## Best Practices

### Course Creation
- Always validate required fields
- Set appropriate difficulty levels
- Include comprehensive descriptions
- Set realistic pricing and duration

### Course Management
- Publish courses only when complete
- Feature high-quality courses
- Monitor enrollment statistics
- Regular content updates

### Search and Discovery
- Use meaningful search terms
- Apply appropriate filters
- Leverage pagination for large datasets
- Monitor popular categories and levels

The Course MCP feature provides comprehensive course management capabilities while maintaining the same patterns and standards as the Lesson feature! ??