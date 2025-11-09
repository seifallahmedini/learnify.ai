# Developer Guide

Guide for developers working on the Learnify.ai MCP Server.

## Prerequisites

- .NET 8.0 SDK or later
- Learnify.ai API running and accessible
- Visual Studio 2022 or VS Code with C# extension

## Setup

### 1. Install .NET SDK

```bash
dotnet --version  # Should return 8.0.x or later
```

### 2. Configure API URL

Edit `appsettings.json`:

```json
{
  "LearnifyApi": {
    "BaseUrl": "http://localhost:5271"
  }
}
```

### 3. Restore and Build

```bash
dotnet restore
dotnet build
```

### 4. Run the Server

```bash
dotnet run
```

Or use startup scripts:
- `start-mcp-server.bat` (Windows)
- `start-mcp-server.ps1` (PowerShell)

## Project Structure

```
learnify.ai.mcp.server/
├── Features/              # Feature modules (vertical slice)
│   ├── Lessons/
│   ├── Courses/
│   ├── Categories/
│   ├── Quizzes/
│   └── Answers/
├── Shared/               # Common infrastructure
│   ├── Models/          # Base API models
│   └── Services/        # BaseApiService
├── Extensions/          # DI extensions
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration
```

## Adding a New Feature

### 1. Create Feature Structure

```
Features/NewFeature/
├── Models/
│   └── NewFeatureModels.cs
├── Services/
│   └── NewFeatureApiService.cs
└── NewFeatureFeature.cs
```

### 2. Create Models

```csharp
// Features/NewFeature/Models/NewFeatureModels.cs
namespace Learnify.Mcp.Server.Features.NewFeature.Models;

public class NewFeatureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### 3. Create Service

```csharp
// Features/NewFeature/Services/NewFeatureApiService.cs
using ModelContextProtocol.Attributes;
using Learnify.Mcp.Server.Shared.Services;

public class NewFeatureApiService : BaseApiService
{
    public NewFeatureApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<NewFeatureApiService> logger)
        : base(httpClient, configuration, logger, nameof(NewFeatureApiService))
    {
    }

    [McpServerToolType(
        name: "GetNewFeatureAsync",
        description: "Get new feature details by ID"
    )]
    public async Task<NewFeatureDto?> GetNewFeatureAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<NewFeatureDto>($"/api/newfeature/{id}", cancellationToken);
    }
}
```

### 4. Create Feature Registration

```csharp
// Features/NewFeature/NewFeatureFeature.cs
public static class NewFeatureFeature
{
    public static IServiceCollection AddNewFeatureFeature(this IServiceCollection services)
    {
        services.AddScoped<NewFeatureApiService>();
        return services;
    }
}
```

### 5. Register in Program.cs

```csharp
builder.Services.AddNewFeatureFeature();
```

## Creating MCP Tools

### Tool Attribute

Use `[McpServerToolType]` to define MCP tools:

```csharp
[McpServerToolType(
    name: "ToolName",
    description: "Tool description"
)]
public async Task<ReturnType> ToolNameAsync(
    ParameterType parameter,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### API Integration

Use `BaseApiService` methods:

```csharp
// GET request
var result = await GetAsync<DtoType>($"/api/endpoint/{id}", cancellationToken);

// POST request
var result = await PostAsync<DtoType>("/api/endpoint", request, cancellationToken);

// PUT request
var result = await PutAsync<DtoType>($"/api/endpoint/{id}", request, cancellationToken);

// DELETE request
var success = await DeleteAsync($"/api/endpoint/{id}", cancellationToken);
```

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "ModelContextProtocol": "Information"
    }
  },
  "LearnifyApi": {
    "BaseUrl": "https://learnify-ai-dev.azurewebsites.net"
  }
}
```

### Environment Variables

Override using environment variables:

```bash
# Windows
set LearnifyApi__BaseUrl=http://localhost:5271

# Linux/macOS
export LearnifyApi__BaseUrl=http://localhost:5271
```

## Claude Desktop Integration

1. Open Claude Desktop config: `%APPDATA%\Claude\claude_desktop_config.json`

2. Add configuration:
```json
{
  "mcpServers": {
    "learnify-ai": {
      "command": "dotnet",
      "args": [
        "run",
        "--project", "C:\\path\\to\\learnify.ai.mcp.server.csproj"
      ]
    }
  }
}
```

3. Restart Claude Desktop

## Code Style

### Naming Conventions

- **Classes**: PascalCase (e.g., `CourseApiService`)
- **Methods**: PascalCase with `Async` suffix (e.g., `GetCourseAsync`)
- **Parameters**: camelCase (e.g., `courseId`)
- **Private fields**: `_camelCase` (e.g., `_httpClient`)

### Best Practices

- Inherit from `BaseApiService` for API services
- Use `[McpServerToolType]` attribute for all MCP tools
- Always use `CancellationToken` in async methods
- Handle errors gracefully with try-catch
- Use structured logging

## Testing

### Run Tests

```bash
dotnet test
```

### Manual Testing

1. Start the server: `dotnet run`
2. Use Claude Desktop to test MCP tools
3. Check logs for errors

## Troubleshooting

### Build Errors

```bash
dotnet clean
dotnet restore
dotnet build
```

### API Connection Failed

- Verify API is running
- Check `appsettings.json` BaseUrl
- Test API connectivity: `curl http://localhost:5271/api/health`

### MCP Server Not Appearing

- Verify Claude Desktop config JSON syntax
- Check file paths are absolute
- Restart Claude Desktop
- Check logs for errors

## Features

- **Lessons** (16 tools) - Lesson content management
- **Courses** (15 tools) - Course lifecycle management
- **Categories** (17 tools) - Category hierarchy management
- **Quizzes** (18 tools) - Quiz and assessment management
- **Answers** (16 tools) - Answer option management

## Resources

- Feature READMEs: `Features/*/README.md`
- Claude Setup Guide: `CLAUDE_SETUP_GUIDE.md`
- .NET 8.0 Docs: https://learn.microsoft.com/dotnet/core/
- MCP Protocol: https://modelcontextprotocol.io/

