# DbContext Placement in Vertical Slice Architecture

## Summary

In your vertical slice architecture, the **DbContext is placed in `Common/Data`** as shared infrastructure. This is the **recommended approach** for most applications.

## Why This Placement?

### ? Shared Infrastructure Approach (Current Implementation)

**Location**: `Common/Data/LearnifyDbContext.cs`

**Pros:**
- Single source of truth for database configuration
- Centralized entity relationship management
- Easier transaction management across features
- Simplified dependency injection setup
- Better for applications with related entities
- Standard EF Core patterns

**Cons:**
- Potential coupling between features through shared context
- All features must use the same database technology

## Architecture Details

```
learnify.ai.api/
??? Common/
?   ??? Data/
?       ??? LearnifyDbContext.cs          # Main DbContext
?       ??? DataSeeder.cs                 # Data seeding
?       ??? Configurations/
?           ??? CourseConfiguration.cs    # Course entity config
?           ??? UserConfiguration.cs      # User entity config
??? Features/
    ??? Courses/
    ?   ??? GetCourses/
    ?   ?   ??? GetCoursesQuery.cs        # Injects LearnifyDbContext
    ?   ??? CreateCourse/
    ?       ??? CreateCourseCommand.cs    # Injects LearnifyDbContext
    ??? Users/
        ??? ...similar structure
```

## How It Works

### 1. DbContext Registration
```csharp
// Program.cs
builder.Services.AddDbContext<LearnifyDbContext>(options =>
{
    options.UseInMemoryDatabase("LearnifyDb");
});
```

### 2. Handler Implementation
```csharp
// Any feature handler
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

### 3. Entity Configuration
```csharp
// Common/Data/Configurations/CourseConfiguration.cs
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(c => c.Id);
        // ... other configurations
    }
}
```

## Alternative Approaches

### Option A: Repository Pattern per Feature
```
Features/Courses/
??? Data/
?   ??? ICourseRepository.cs
?   ??? CourseRepository.cs      # Wraps DbContext access
??? GetCourses/
    ??? GetCoursesQuery.cs       # Injects ICourseRepository
```

**When to use:** When you want additional abstraction over data access.

### Option B: Feature-Specific DbContexts
```
Features/Courses/
??? Data/
?   ??? CourseDbContext.cs       # Course-specific context
??? ...

Features/Users/
??? Data/
?   ??? UserDbContext.cs         # User-specific context
??? ...
```

**When to use:** For microservices architecture or when features are completely independent.

### Option C: Shared DbContext with Feature Interfaces
```
Common/Data/
??? LearnifyDbContext.cs         # Shared context

Features/Courses/
??? Data/
?   ??? ICourseDataAccess.cs     # Feature-specific interface
??? ...
```

**When to use:** When you want the benefits of shared context but with feature-specific abstractions.

## Decision Matrix

| Approach | Complexity | Coupling | Testability | Transactions | Best For |
|----------|------------|----------|-------------|--------------|----------|
| **Shared DbContext** (Current) | Low | Medium | Good | Easy | Most apps |
| Repository per Feature | Medium | Low | Excellent | Medium | Clean Architecture |
| DbContext per Feature | High | Very Low | Good | Hard | Microservices |
| Shared + Interfaces | Medium | Low | Excellent | Easy | Large teams |

## Current Implementation Benefits

1. **Simplicity**: Easy to understand and maintain
2. **EF Core Best Practices**: Follows standard patterns
3. **Cross-Feature Queries**: Can easily query across entities
4. **Transaction Support**: Easy to manage transactions across features
5. **Rapid Development**: Quick to implement new features

## When to Consider Alternatives

- **Large teams**: Multiple teams working on different features
- **Microservices**: Features need complete independence
- **Different data stores**: Some features need different databases
- **High coupling concerns**: Features becoming too interdependent

## Migration Path

If you need to move to a different approach later:

1. **To Repository Pattern**: Wrap current DbContext usage in repositories
2. **To Feature DbContexts**: Extract entity configurations and create separate contexts
3. **To Microservices**: Split along feature boundaries with event communication

## Conclusion

The current **shared DbContext approach** is the right choice for your learning platform because:
- Features are related (Users take Courses)
- Team size is manageable
- Rapid development is priority
- Standard patterns are preferred

This approach can easily evolve as your application grows and requirements change.