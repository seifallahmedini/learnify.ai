# Learnify.ai API - Vertical Slice Architecture

This project implements **Vertical Slice Architecture** for a clean, maintainable, and scalable API design.

## Architecture Overview

Vertical Slice Architecture organizes code by **features** rather than technical layers. Each feature is self-contained and includes everything needed for that specific business capability.

## Project Structure

```
learnify.ai.api/
??? Common/                     # Shared infrastructure
?   ??? Behaviors/              # MediatR pipeline behaviors
?   ??? Controllers/            # Base controllers
?   ??? Data/                   # DbContext and data configurations
?   ?   ??? Configurations/     # Entity configurations
?   ?   ??? LearnifyDbContext.cs # Main DbContext
?   ?   ??? DataSeeder.cs       # Data seeding
?   ??? Interfaces/             # Shared interfaces
?   ??? Middleware/             # Custom middleware
?   ??? Models/                 # Common response models
??? Features/                   # Business features (vertical slices)
?   ??? Courses/                # Course management feature
?   ?   ??? Models/             # Course domain models
?   ?   ??? GetCourses/         # Get all courses slice
?   ?   ??? GetCourse/          # Get single course slice
?   ?   ??? CreateCourse/       # Create course slice
?   ?   ??? CoursesController.cs
?   ??? Users/                  # User management feature
?       ??? Models/             # User domain models
?       ??? GetUsers/           # Get all users slice
?       ??? CreateUser/         # Create user slice
?       ??? UsersController.cs
??? Program.cs                  # Application entry point
```

## Key Principles

### 1. Feature-Based Organization
- Each feature is a complete vertical slice
- Contains all layers needed for that feature
- Minimizes cross-feature dependencies

### 2. CQRS with MediatR
- **Commands**: Write operations that change state
- **Queries**: Read operations that return data
- Clear separation of concerns

### 3. Database Integration
- **DbContext Location**: Placed in `Common/Data` as shared infrastructure
- **Entity Configurations**: Separate configuration files for each entity
- **Data Access**: Injected into handlers via dependency injection
- **Development**: Uses InMemory database for easy testing
- **Production**: Can be easily switched to SQL Server

### 4. Validation Pipeline
- FluentValidation for request validation
- Automatic validation through MediatR pipeline
- Consistent error handling

### 5. Consistent API Responses
- Standardized `ApiResponse<T>` wrapper
- Success/error states with messages
- Global exception handling

## Database Architecture in Vertical Slices

### DbContext Placement
The `LearnifyDbContext` is placed in `Common/Data` because:
- It's shared infrastructure used across multiple features
- Maintains separation of concerns
- Allows for centralized configuration
- Supports dependency injection patterns

### Entity Configuration
Each entity has its own configuration file:
```csharp
// Common/Data/Configurations/CourseConfiguration.cs
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        // Entity configuration here
    }
}
```

### Data Access in Handlers
Handlers receive DbContext via dependency injection:
```csharp
public class GetCoursesHandler : IRequestHandler<GetCoursesQuery, List<Course>>
{
    private readonly LearnifyDbContext _context;

    public GetCoursesHandler(LearnifyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Course>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Courses.ToListAsync(cancellationToken);
    }
}
```

## Alternative Database Patterns

### Option 1: Feature-Specific Repositories (Alternative)
If you prefer more isolation, you can create feature-specific repositories:

```
Features/Courses/
??? Data/
?   ??? ICourseRepository.cs
?   ??? CourseRepository.cs
??? ...other files
```

### Option 2: Database Per Feature (Microservices)
For ultimate isolation, each feature could have its own database:
- Separate DbContext per feature
- Independent database schemas
- Event-driven communication between features

## Adding a New Feature with Database

To add a new feature (e.g., "Enrollments"):

1. **Create the domain model:**
   ```csharp
   public class Enrollment
   {
       public int Id { get; set; }
       public int UserId { get; set; }
       public int CourseId { get; set; }
       // ... other properties
   }
   ```

2. **Add entity configuration:**
   ```csharp
   public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
   {
       // Configuration here
   }
   ```

3. **Add DbSet to DbContext:**
   ```csharp
   public DbSet<Enrollment> Enrollments { get; set; }
   ```

4. **Implement handlers with data access:**
   ```csharp
   public class GetEnrollmentsHandler : IRequestHandler<GetEnrollmentsQuery, List<Enrollment>>
   {
       private readonly LearnifyDbContext _context;
       // Implementation
   }
   ```

## Database Configuration

### Development (InMemory)
Currently configured for easy development and testing:
```csharp
options.UseInMemoryDatabase("LearnifyDb");
```

### Production (SQL Server)
To switch to SQL Server:

1. **Update appsettings.json:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=...;Database=LearnifyDb;..."
     }
   }
   ```

2. **Update Program.cs:**
   ```csharp
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
   ```

3. **Create and run migrations:**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Benefits

- **Maintainability**: Features are isolated and easy to modify
- **Scalability**: New features don't affect existing ones
- **Team Productivity**: Different teams can work on different features
- **Testing**: Each slice can be tested independently
- **Deployment**: Features can be deployed incrementally
- **Data Isolation**: Clear boundaries for data access patterns

## Technologies Used

- **.NET 8**: Latest .NET framework
- **Entity Framework Core**: ORM for database access
- **MediatR**: CQRS and mediator pattern implementation
- **FluentValidation**: Fluent validation library
- **Swagger**: API documentation

## Getting Started

1. Run the application:
   ```bash
   dotnet run
   ```

2. Navigate to Swagger UI:
   ```
   https://localhost:{port}/swagger
   ```

3. Test the endpoints:
   - `GET /api/courses` - Get all courses
   - `GET /api/courses/{id}` - Get course by ID
   - `POST /api/courses` - Create a new course
   - `GET /api/users` - Get all users
   - `POST /api/users` - Create a new user

The application will automatically seed sample data in development mode.

## Example Request/Response

### Create Course Request:
```json
POST /api/courses
{
    "title": "React Fundamentals",
    "description": "Learn React from scratch",
    "instructor": "Alice Johnson",
    "price": 149.99,
    "durationHours": 25
}
```

### Response:
```json
{
    "success": true,
    "data": {
        "id": 1234,
        "title": "React Fundamentals",
        "description": "Learn React from scratch",
        "instructor": "Alice Johnson",
        "price": 149.99,
        "durationHours": 25,
        "createdAt": "2025-01-27T10:00:00Z",
        "updatedAt": "2025-01-27T10:00:00Z"
    },
    "message": "Course created successfully"
}