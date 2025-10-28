using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using learnify.ai.api.Common.Behaviors;
using learnify.ai.api.Common.Data;
using learnify.ai.api.Common.Middleware;
using learnify.ai.api.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Enforce fixed port 8080 - override any other configuration
Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:8080");
Environment.SetEnvironmentVariable("ASPNETCORE_HTTP_PORTS", "8080");

// Configure fixed port 8080 for the API
builder.WebHost.UseUrls("http://+:8080");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks
builder.Services.AddHealthChecks();

// Add DbContext
builder.Services.AddDbContext<LearnifyDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(conn))
        options.UseNpgsql(conn);
    else
        options.UseInMemoryDatabase("LearnifyDb");
});

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Add pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddRepositories();

var app = builder.Build();

// Apply migrations automatically (best effort) when using Postgres
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LearnifyDbContext>();
    try
    {
        if (db.Database.IsNpgsql())
        {
            db.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var migrationLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbMigrations");
        migrationLogger.LogError(ex, "Database migration failed");
    }
}

// Seed data in development (only for InMemory)
//if (app.Environment.IsDevelopment())
//{
    using var seederScope = app.Services.CreateScope();
    var context = seederScope.ServiceProvider.GetRequiredService<LearnifyDbContext>();
    if (context.Database.IsNpgsql())
    {
        await DataSeeder.SeedAsync(context);
    }
//}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Add global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors();

app.UseAuthorization();

// Add health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

// Log the configured URLs for verification
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Learnify API configured to run on FIXED PORT 8080");
logger.LogInformation("Access the API at: http://localhost:8080");
logger.LogInformation("Access Swagger at: http://localhost:8080/swagger");

app.Run();
