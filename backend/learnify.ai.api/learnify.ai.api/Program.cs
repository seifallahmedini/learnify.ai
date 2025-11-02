using learnify.ai.api.Common.Extensions;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Infrastructure.Middleware;
using learnify.ai.api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Enforce fixed port 8080 - override any other configuration
Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://+:8080");
Environment.SetEnvironmentVariable("ASPNETCORE_HTTP_PORTS", "8080");

// Configure fixed port 8080 for the API
builder.WebHost.UseUrls("http://+:8080");

// Add services using extension methods for better organization
builder.Services
    .AddApiServices()
    .AddDatabaseServices(builder.Configuration)
    .AddIdentityServices()
    .AddAuthenticationServices(builder.Configuration)
    .AddAuthorizationServices()
    .AddMediatRServices()
    .AddCorsServices()
    .AddApplicationServices();

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
    await DataSeeder.SeedAsync(seederScope.ServiceProvider);
}
await IdentitySeeder.SeedAsync(seederScope.ServiceProvider);
//}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// Add global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors();

app.UseAuthentication();
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
