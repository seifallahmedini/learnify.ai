using learnify.ai.api.Common.Extensions;
using learnify.ai.api.Common.Infrastructure.Data;
using learnify.ai.api.Common.Infrastructure.Middleware;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Azure Web Service compatibility: Use PORT environment variable if available, otherwise default to 8080
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var urls = $"http://+:{port}";

// Configure port for the API (Azure-compatible)
builder.WebHost.UseUrls(urls);

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

// Configure forwarded headers for Azure (HTTPS forwarding)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Apply migrations and seed data asynchronously (non-blocking for Azure warm-up)
_ = Task.Run(async () =>
{
    await Task.Delay(TimeSpan.FromSeconds(2)); // Give app time to start
    
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<LearnifyDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        if (db.Database.IsNpgsql())
        {
            logger.LogInformation("Starting database migrations...");
            db.Database.Migrate();
            logger.LogInformation("Database migrations completed successfully");
            
            logger.LogInformation("Starting data seeding...");
            await DataSeeder.SeedAsync(scope.ServiceProvider);
            await IdentitySeeder.SeedAsync(scope.ServiceProvider);
            logger.LogInformation("Data seeding completed successfully");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration or seeding failed");
    }
});

// Configure the HTTP request pipeline.
// Forward HTTPS headers in Azure (required for proper HTTPS handling)
var isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
if (isAzure)
{
    app.UseForwardedHeaders();
}

app.UseSwagger();
app.UseSwaggerUI();

// Add global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Add health check endpoint (simple endpoint for Azure warm-up)
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new { status = "ok", message = "Learnify API is running" }))
    .WithName("Root")
    .WithTags("Health");

app.MapControllers();

// Log the configured URLs for verification
var appLogger = app.Services.GetRequiredService<ILogger<Program>>();
var baseUrl = isAzure 
    ? $"https://{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}" 
    : $"http://localhost:{port}";

appLogger.LogInformation("Learnify API configured to run on PORT {Port}", port);
appLogger.LogInformation("Environment: {Environment}", isAzure ? "Azure" : "Local");
appLogger.LogInformation("Access the API at: {BaseUrl}", baseUrl);
appLogger.LogInformation("Access Swagger at: {SwaggerUrl}", $"{baseUrl}/swagger");

app.Run();
