using System.Reflection;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using learnify.ai.api.Common.Abstractions;
using learnify.ai.api.Common.Infrastructure;
using learnify.ai.api.Common.Infrastructure.Behaviors;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Domain.Entities;
using learnify.ai.api.Features.Users;
using learnify.ai.api.Features.Courses;
using learnify.ai.api.Features.Enrollments;
using learnify.ai.api.Features.Assessments;
using learnify.ai.api.Features.Payments;
using learnify.ai.api.Features.Reviews;

namespace learnify.ai.api.Common.Extensions;

public static class DependencyInjection
{
    /// <summary>
    /// Adds API-related services (Controllers, Swagger, Health Checks)
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        
        return services;
    }

    /// <summary>
    /// Adds database services (DbContext)
    /// </summary>
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<LearnifyDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseNpgsql(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase("LearnifyDb");
            }
        });

        return services;
    }

    /// <summary>
    /// Adds ASP.NET Core Identity services
    /// </summary>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services
            .AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<LearnifyDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Adds JWT Bearer authentication services
    /// </summary>
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] ?? "dev_super_secret_key_change_me_minimum_32_bytes_long_key_for_security";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "learnify-api";
        var jwtAudience = configuration["Jwt:Audience"] ?? "learnify-clients";

        // Ensure key is at least 32 bytes (256 bits) for HS256
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        if (keyBytes.Length < 32)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            keyBytes = sha256.ComputeHash(keyBytes);
        }

        var signingKey = new SymmetricSecurityKey(keyBytes);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
            });

        return services;
    }

    /// <summary>
    /// Adds authorization policies
    /// </summary>
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireInstructor", policy => policy.RequireRole("Instructor", "Admin"));
        });

        return services;
    }

    /// <summary>
    /// Adds MediatR and FluentValidation services
    /// </summary>
    public static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds CORS configuration
    /// </summary>
    public static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }

    /// <summary>
    /// Adds all repositories to the service collection
    /// </summary>
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

    /// <summary>
    /// Adds application-specific services (repositories, JWT service, etc.)
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add repositories
        services.AddRepositories();

        // Add JWT Token Service
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Add other application services here
        // services.AddScoped<IEmailService, EmailService>();
        // services.AddScoped<IFileStorageService, FileStorageService>();
        // services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        
        return services;
    }
}
