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
                // Instructors (5 users)
                new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PasswordHash = "hashed_password_here", // In real app, use proper password hashing
                    Role = UserRole.Instructor,
                    Bio = "Experienced software engineer with 10+ years in .NET development. Passionate about teaching and sharing knowledge.",
                    PhoneNumber = "+1-555-0101",
                    DateOfBirth = new DateTime(1985, 3, 15),
                    ProfilePicture = "https://randomuser.me/api/portraits/men/1.jpg",
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
                    Bio = "Full-stack developer and tech educator. Specializes in modern web technologies and React development.",
                    PhoneNumber = "+1-555-0102",
                    DateOfBirth = new DateTime(1988, 7, 22),
                    ProfilePicture = "https://randomuser.me/api/portraits/women/2.jpg",
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
                    Bio = "Database architect and SQL expert. 15+ years experience in database design and optimization.",
                    PhoneNumber = "+1-555-0103",
                    DateOfBirth = new DateTime(1982, 11, 8),
                    ProfilePicture = "https://randomuser.me/api/portraits/women/3.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new User
                {
                    FirstName = "Michael",
                    LastName = "Brown",
                    Email = "michael.brown@example.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Instructor,
                    Bio = "DevOps engineer and cloud solutions architect. Expert in Azure, AWS, and containerization technologies.",
                    PhoneNumber = "+1-555-0104",
                    DateOfBirth = new DateTime(1987, 5, 12),
                    ProfilePicture = "https://randomuser.me/api/portraits/men/4.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new User
                {
                    FirstName = "Sarah",
                    LastName = "Wilson",
                    Email = "sarah.wilson@example.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Instructor,
                    Bio = "Mobile app developer and UI/UX designer. Passionate about creating intuitive user experiences.",
                    PhoneNumber = "+1-555-0105",
                    DateOfBirth = new DateTime(1990, 9, 3),
                    ProfilePicture = "https://randomuser.me/api/portraits/women/5.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },

                // Students (9 users)
                new User
                {
                    FirstName = "David",
                    LastName = "Garcia",
                    Email = "david.garcia@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "Computer science student passionate about learning new technologies and building innovative projects.",
                    PhoneNumber = "+1-555-0201",
                    DateOfBirth = new DateTime(1998, 4, 18),
                    ProfilePicture = "https://randomuser.me/api/portraits/men/6.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-35),
                    UpdatedAt = DateTime.UtcNow.AddDays(-7)
                },
                new User
                {
                    FirstName = "Emily",
                    LastName = "Davis",
                    Email = "emily.davis@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "Career changer transitioning from marketing to software development. Eager to learn and grow.",
                    PhoneNumber = "+1-555-0202",
                    DateOfBirth = new DateTime(1992, 12, 25),
                    ProfilePicture = "https://randomuser.me/api/portraits/women/7.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-28),
                    UpdatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new User
                {
                    FirstName = "Robert",
                    LastName = "Miller",
                    Email = "robert.miller@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "Self-taught programmer looking to formalize knowledge and learn industry best practices.",
                    PhoneNumber = "+1-555-0203",
                    DateOfBirth = new DateTime(1995, 6, 14),
                    ProfilePicture = "https://randomuser.me/api/portraits/men/8.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-22),
                    UpdatedAt = DateTime.UtcNow.AddDays(-6)
                },
                new User
                {
                    FirstName = "Lisa",
                    LastName = "Anderson",
                    Email = "lisa.anderson@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "Recent graduate seeking to enhance programming skills and start a career in tech.",
                    PhoneNumber = "+1-555-0204",
                    DateOfBirth = new DateTime(1999, 8, 7),
                    ProfilePicture = "https://randomuser.me/api/portraits/women/9.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new User
                {
                    FirstName = "James",
                    LastName = "Taylor",
                    Email = "james.taylor@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "Working professional looking to upskill and advance career in software development.",
                    PhoneNumber = "+1-555-0205",
                    DateOfBirth = new DateTime(1991, 1, 30),
                    ProfilePicture = "https://randomuser.me/api/portraits/men/10.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new User
                {
                    FirstName = "Maria",
                    LastName = "Rodriguez",
                    Email = "maria.rodriguez@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "Entrepreneur learning to code to build my own tech startup. Focused on web development.",
                    PhoneNumber = "+1-555-0206",
                    DateOfBirth = new DateTime(1993, 10, 11),
                    ProfilePicture = "https://randomuser.me/api/portraits/women/11.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new User
                {
                    FirstName = "Kevin",
                    LastName = "Lee",
                    Email = "kevin.lee@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "High school student with a passion for programming and game development.",
                    PhoneNumber = "+1-555-0207",
                    DateOfBirth = new DateTime(2005, 3, 28),
                    ProfilePicture = "https://randomuser.me/api/portraits/men/12.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new User
                {
                    FirstName = "Rachel",
                    LastName = "Thompson",
                    Email = "rachel.thompson@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "Graphic designer expanding skills into front-end development and user interface programming.",
                    PhoneNumber = "+1-555-0208",
                    DateOfBirth = new DateTime(1994, 7, 16),
                    ProfilePicture = "https://randomuser.me/api/portraits/women/13.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    UpdatedAt = DateTime.UtcNow.AddHours(-12)
                },
                new User
                {
                    FirstName = "Daniel",
                    LastName = "White",
                    Email = "daniel.white@student.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Student,
                    Bio = "Data analyst learning programming to enhance data processing and automation capabilities.",
                    PhoneNumber = "+1-555-0209",
                    DateOfBirth = new DateTime(1989, 12, 2),
                    ProfilePicture = "https://randomuser.me/api/portraits/men/14.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    UpdatedAt = DateTime.UtcNow.AddHours(-6)
                },

                // Admin (1 user)
                new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@learnify.com",
                    PasswordHash = "hashed_password_here",
                    Role = UserRole.Admin,
                    Bio = "Platform administrator responsible for managing the learning management system and ensuring quality education delivery.",
                    PhoneNumber = "+1-555-0001",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    ProfilePicture = "https://randomuser.me/api/portraits/men/15.jpg",
                    CreatedAt = DateTime.UtcNow.AddDays(-90),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
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
            var michaelBrown = context.Users.First(u => u.Email == "michael.brown@example.com");
            var sarahWilson = context.Users.First(u => u.Email == "sarah.wilson@example.com");
            
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
                    DiscountPrice = 79.99m, // 20% discount
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
                    DiscountPrice = 149.99m, // 25% discount
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
                    InstructorId = janeSmith.Id,
                    CategoryId = webDevCategory.Id,
                    Price = 149.99m,
                    DiscountPrice = 119.99m, // 20% discount
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
                    InstructorId = aliceJohnson.Id,
                    CategoryId = databaseCategory.Id,
                    Price = 179.99m,
                    DiscountPrice = null, // No discount - full price
                    DurationHours = 50,
                    Level = CourseLevel.Intermediate,
                    Language = "English",
                    IsPublished = true,
                    LearningObjectives = "Design efficient databases and write optimized SQL queries",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Title = "DevOps with Azure",
                    Description = "Learn DevOps practices using Microsoft Azure. Covers CI/CD pipelines, containerization, and cloud deployment strategies.",
                    ShortDescription = "DevOps and cloud deployment with Azure",
                    InstructorId = michaelBrown.Id,
                    CategoryId = programmingCategory.Id,
                    Price = 249.99m,
                    DiscountPrice = 199.99m, // 20% discount
                    DurationHours = 45,
                    Level = CourseLevel.Advanced,
                    Language = "English",
                    IsPublished = true,
                    LearningObjectives = "Implement DevOps practices and deploy applications to Azure cloud",
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    UpdatedAt = DateTime.UtcNow.AddHours(-12)
                },
                new()
                {
                    Title = "Mobile App Development with React Native",
                    Description = "Build cross-platform mobile applications using React Native. Learn navigation, state management, and native integrations.",
                    ShortDescription = "Cross-platform mobile development",
                    InstructorId = sarahWilson.Id,
                    CategoryId = webDevCategory.Id,
                    Price = 189.99m,
                    DiscountPrice = 139.99m, // 26% discount - limited time offer
                    DurationHours = 55,
                    Level = CourseLevel.Intermediate,
                    Language = "English",
                    IsPublished = true,
                    LearningObjectives = "Create professional mobile apps for iOS and Android using React Native",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddHours(-6)
                }
            };

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();
        }
    }
}