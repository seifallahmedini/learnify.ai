// Extension method to register all repositories and services
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Assessments;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Payments;
using learnify.ai.api.Features.Reviews;

namespace learnify.ai.api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Core entity repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        
        // Enrollment system repositories
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        
        // Assessment system repositories
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IAnswerRepository, AnswerRepository>();
        services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
        
        // Business entity repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add repositories
        services.AddRepositories();

        // Add other application services here
        // services.AddScoped<IEmailService, EmailService>();
        // services.AddScoped<IFileStorageService, FileStorageService>();
        // services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        
        return services;
    }
}