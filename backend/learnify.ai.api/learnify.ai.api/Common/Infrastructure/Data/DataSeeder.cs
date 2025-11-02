using Microsoft.AspNetCore.Identity;
using learnify.ai.api.Common.Enums;
using learnify.ai.api.Common.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace learnify.ai.api.Common.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LearnifyDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        // Ensure roles exist first
        var roles = new[] { "Admin", "Instructor", "Student" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role(roleName));
            }
        }

        // Seed Users first
        if (!context.Users.Any())
        {
            var defaultPassword = "Password@123"; // Default password for seeded users

            var userData = new List<(User User, string Role, string Password)>
            {
                (new User
                {
                    UserName = "john.doe@example.com",
                    Email = "john.doe@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = Gender.Male,
                    Bio = "Experienced software engineer with 10+ years in web development",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    PhoneNumber = "+1234567890",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }, "Instructor", defaultPassword),
                
                (new User
                {
                    UserName = "jane.smith@example.com",
                    Email = "jane.smith@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Gender = Gender.Female,
                    Bio = "Full-stack developer and tech educator",
                    DateOfBirth = new DateTime(1990, 8, 22),
                    PhoneNumber = "+1234567891",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }, "Instructor", defaultPassword),
                
                (new User
                {
                    UserName = "alice.johnson@example.com",
                    Email = "alice.johnson@example.com",
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Gender = Gender.Female,
                    Bio = "Data scientist and machine learning enthusiast",
                    DateOfBirth = new DateTime(1988, 3, 10),
                    PhoneNumber = "+1234567892",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }, "Student", defaultPassword),
                
                (new User
                {
                    UserName = "bob.williams@example.com",
                    Email = "bob.williams@example.com",
                    FirstName = "Bob",
                    LastName = "Williams",
                    Gender = Gender.Male,
                    Bio = "Learning web development",
                    DateOfBirth = new DateTime(1995, 11, 5),
                    PhoneNumber = "+1234567893",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }, "Student", defaultPassword),
                
                (new User
                {
                    UserName = "admin@learnify.com",
                    Email = "admin@learnify.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Gender = Gender.Male,
                    Bio = "System administrator",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    PhoneNumber = "+1234567894",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }, "Admin", defaultPassword)
            };

            // Create users with UserManager and assign roles
            foreach (var (user, roleName, password) in userData)
            {
                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, roleName);
                    }
                }
            }
        }

        // Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Programming",
                    Description = "Learn various programming languages and frameworks",
                    IconUrl = "https://example.com/icons/programming.png",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Web Development",
                    Description = "Frontend and backend web development courses",
                    IconUrl = "https://example.com/icons/web-dev.png",
                    ParentCategoryId = 1, // Programming
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Data Science",
                    Description = "Data analysis, machine learning, and AI courses",
                    IconUrl = "https://example.com/icons/data-science.png",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // Seed Courses (only if categories exist)
        if (!context.Courses.Any() && context.Categories.Any())
        {
            var instructors = await userManager.GetUsersInRoleAsync("Instructor");
            var firstInstructor = instructors.FirstOrDefault();
            var webDevCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Web Development");
            var dataScienceCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Data Science");

            if (firstInstructor != null)
            {
                var courses = new List<Course>
                {
                    new Course
                    {
                        Title = "Complete React Development Course",
                        Description = "Master React from basics to advanced concepts",
                        ShortDescription = "Learn React and build modern web applications",
                        InstructorId = firstInstructor.Id,
                        CategoryId = webDevCategory?.Id ?? 1,
                        Price = 99.99m,
                        DurationHours = 40,
                        Level = CourseLevel.Intermediate,
                        Language = "English",
                        IsPublished = true,
                        LearningObjectives = "Understand React fundamentals, Build reusable components, State management, Routing",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Course
                    {
                        Title = "Python for Data Science",
                        Description = "Learn Python programming for data analysis and visualization",
                        ShortDescription = "Comprehensive Python data science course",
                        InstructorId = firstInstructor.Id,
                        CategoryId = dataScienceCategory?.Id ?? 1,
                        Price = 129.99m,
                        DurationHours = 50,
                        Level = CourseLevel.Beginner,
                        Language = "English",
                        IsPublished = true,
                        LearningObjectives = "Python basics, Data manipulation, Visualization, Statistical analysis",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                await context.Courses.AddRangeAsync(courses);
                await context.SaveChangesAsync();
            }
        }
    }
}
