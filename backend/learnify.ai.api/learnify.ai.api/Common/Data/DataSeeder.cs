using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Users;

namespace learnify.ai.api.Common.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(LearnifyDbContext context)
    {
        // Seed Users first
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PasswordHash = "hashed_password_here", // In real app, use proper password hashing
                    Role = UserRole.Instructor,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new User
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Instructor,
                    CreatedAt = DateTime.UtcNow.AddDays(-45),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new User
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Email = "alice.johnson@example.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Instructor,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        // Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new()
                {
                    Name = "Programming",
                    Description = "Software development and programming courses",
                    CreatedAt = DateTime.UtcNow.AddDays(-90),
                    UpdatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new()
                {
                    Name = "Web Development",
                    Description = "Frontend and backend web development",
                    CreatedAt = DateTime.UtcNow.AddDays(-85),
                    UpdatedAt = DateTime.UtcNow.AddDays(-85)
                },
                new()
                {
                    Name = "Database",
                    Description = "Database design and management",
                    CreatedAt = DateTime.UtcNow.AddDays(-80),
                    UpdatedAt = DateTime.UtcNow.AddDays(-80)
                }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed Courses
        if (!context.Courses.Any())
        {
            var johnDoe = context.Users.First(u => u.Email == "john.doe@example.com");
            var janeSmith = context.Users.First(u => u.Email == "jane.smith@example.com");
            var aliceJohnson = context.Users.First(u => u.Email == "alice.johnson@example.com");
            
            var programmingCategory = context.Categories.First(c => c.Name == "Programming");
            var webDevCategory = context.Categories.First(c => c.Name == "Web Development");
            var databaseCategory = context.Categories.First(c => c.Name == "Database");

            var courses = new List<Course>
            {
                new()
                {
                    Title = "Introduction to C#",
                    Description = "Learn the basics of C# programming language. This course covers variables, data types, control structures, and object-oriented programming concepts.",
                    ShortDescription = "Learn C# programming fundamentals",
                    InstructorId = johnDoe.Id,
                    CategoryId = programmingCategory.Id,
                    Price = 99.99m,
                    DurationHours = 40,
                    Level = CourseLevel.Beginner,
                    Language = "English",
                    IsPublished = true,
                    LearningObjectives = "Understand C# syntax, OOP concepts, and basic programming patterns",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    Title = "Advanced .NET Core",
                    Description = "Master advanced concepts in .NET Core development including dependency injection, middleware, and API development.",
                    ShortDescription = "Advanced .NET Core development",
                    InstructorId = janeSmith.Id,
                    CategoryId = programmingCategory.Id,
                    Price = 199.99m,
                    DurationHours = 60,
                    Level = CourseLevel.Advanced,
                    Language = "English",
                    IsPublished = true,
                    LearningObjectives = "Master advanced .NET Core concepts and build production-ready applications",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new()
                {
                    Title = "React Fundamentals",
                    Description = "Learn React from scratch including components, state management, hooks, and modern React patterns.",
                    ShortDescription = "Learn React from scratch",
                    InstructorId = aliceJohnson.Id,
                    CategoryId = webDevCategory.Id,
                    Price = 149.99m,
                    DurationHours = 35,
                    Level = CourseLevel.Intermediate,
                    Language = "English",
                    IsPublished = true,
                    LearningObjectives = "Build modern web applications using React and understand component-based architecture",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new()
                {
                    Title = "Database Design with SQL Server",
                    Description = "Comprehensive course on database design principles, SQL Server administration, and query optimization.",
                    ShortDescription = "SQL Server database design and optimization",
                    InstructorId = johnDoe.Id,
                    CategoryId = databaseCategory.Id,
                    Price = 179.99m,
                    DurationHours = 50,
                    Level = CourseLevel.Intermediate,
                    Language = "English",
                    IsPublished = true,
                    LearningObjectives = "Design efficient databases and write optimized SQL queries",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();
        }
    }
}