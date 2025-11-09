# Architecture Documentation

Architecture and design documentation for the Learnify.ai MCP Server.

## Overview

The Learnify.ai MCP Server is a Model Context Protocol (MCP) server that exposes 82 tools for managing the Learnify.ai learning platform. It follows a vertical slice architecture pattern and integrates with the Learnify.ai REST API.

## Architecture Pattern

### Vertical Slice Architecture

The project uses **vertical slice architecture**, organizing code by feature rather than by technical layer. Each feature is self-contained with its own models, services, and registration logic.

```
Features/{FeatureName}/
├── Models/              # Domain models and DTOs
├── Services/           # MCP tools and API integration
└── {Feature}Feature.cs # Dependency injection registration
```

**Benefits:**
- Features are independent and can be developed in parallel
- Easy to locate all code related to a specific feature
- Reduced coupling between features
- Clear boundaries and responsibilities

## System Architecture

### High-Level Diagram

```
┌─────────────────────────────────┐
│   Claude Desktop / AI Client    │
└──────────────┬──────────────────┘
               │ MCP Protocol (stdio)
               ▼
┌─────────────────────────────────┐
│   Learnify.ai MCP Server        │
│  ┌──────────────────────────┐   │
│  │   MCP Server Core        │   │
│  │  - Tool Discovery        │   │
│  │  - Request Routing       │   │
│  └──────────┬───────────────┘   │
│             │                    │
│  ┌──────────┴───────────────┐   │
│  │   Feature Services       │   │
│  │  - LessonApiService      │   │
│  │  - CourseApiService      │   │
│  │  - CategoryApiService    │   │
│  │  - QuizApiService        │   │
│  │  - AnswerApiService      │   │
│  └──────────┬───────────────┘   │
└─────────────┼────────────────────┘
              │ HTTP
              ▼
┌─────────────────────────────────┐
│   Learnify.ai REST API          │
│  - Business Logic               │
│  - Data Persistence             │
└─────────────────────────────────┘
```

### Component Layers

#### 1. Application Layer
- **Program.cs**: Application startup, service registration, MCP server initialization
- **Configuration**: appsettings.json, environment variables

#### 2. Feature Layer
- **Features/**: Feature-specific business logic, MCP tools, API integration
- Each feature is a vertical slice with models, services, and registration

#### 3. Shared Layer
- **Shared/Models/**: Common API response models
- **Shared/Services/**: BaseApiService for HTTP communication

#### 4. Extension Layer
- **Extensions/**: Dependency injection extensions, service configuration

## Technology Stack

### Core Technologies

- **.NET 8.0**: Runtime framework
- **ModelContextProtocol 0.4.0-preview.2**: MCP protocol implementation
- **Microsoft.Extensions.Hosting**: Application hosting
- **Microsoft.Extensions.DependencyInjection**: Dependency injection
- **System.Text.Json**: JSON serialization
- **HttpClient**: HTTP client for API communication

### Dependencies

```xml
<PackageReference Include="ModelContextProtocol" Version="0.4.0-preview.2" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.8" />
<PackageReference Include="System.Text.Json" Version="9.0.8" />
```

## Design Patterns

### 1. Vertical Slice Architecture
Organizes code by feature, promoting high cohesion and loose coupling.

### 2. Dependency Injection
All services are registered through extension methods and injected via constructor.

### 3. Base Service Pattern
All feature services inherit from `BaseApiService`, providing:
- HTTP client management
- Configuration access
- Logging
- Error handling
- JSON serialization

### 4. MCP Tool Pattern
Tools are defined using `[McpServerToolType]` attribute and automatically discovered.

## Feature Structure

### Feature Components

Each feature consists of:

1. **Models** (`Models/{Feature}Models.cs`)
   - Domain models (DTOs)
   - Request/response models
   - Enums and value objects

2. **Services** (`Services/{Feature}ApiService.cs`)
   - MCP tools with `[McpServerToolType]` attribute
   - API integration methods
   - Business logic

3. **Feature Registration** (`{Feature}Feature.cs`)
   - Dependency injection registration
   - Service configuration

## Feature Relationships

### Dependency Graph

```
Categories
    ├── Courses (uses categoryId)
    │       ├── Lessons (uses courseId)
    │       │       └── Quizzes (uses lessonId)
    │       └── Quizzes (uses courseId)
    │               └── Questions
    │                       └── Answers (uses questionId)
```

### Integration Points

- **Category → Course**: `GetCategoryCoursesAsync`
- **Course → Lesson**: `GetCourseLessonsAsync`, `CreateCourseLessonAsync`
- **Course → Quiz**: `GetCourseQuizzesAsync`, `CreateQuizAsync`
- **Lesson → Quiz**: `GetLessonQuizzesAsync`, `CreateQuizAsync`
- **Quiz → Question**: `GetQuizQuestionsAsync`, `AddQuestionToQuizAsync`
- **Question → Answer**: `GetQuestionAnswersAsync`, `CreateAnswerAsync`

## Data Flow

### Request Flow

1. **Client Request** → Claude Desktop sends MCP request via stdio
2. **MCP Server** → Receives and parses request
3. **Tool Discovery** → MCP server finds tool by name using reflection
4. **Service Invocation** → Calls corresponding service method
5. **API Call** → Service makes HTTP request to Learnify.ai API
6. **Response Processing** → Service processes API response
7. **MCP Response** → Returns result to client via stdio

### Error Handling Flow

1. **API Error** → Caught by `BaseApiService`
2. **Error Extraction** → Detailed error information extracted
3. **Logging** → Error logged with context
4. **Exception** → Exception thrown with meaningful message
5. **MCP Error** → MCP server converts to MCP error response
6. **Client Notification** → Error returned to client

## Configuration Architecture

### Configuration Sources

1. **appsettings.json** - Base configuration
2. **appsettings.Development.json** - Development overrides
3. **Environment Variables** - Runtime overrides
4. **Command Line Arguments** - Launch-time overrides

### Configuration Structure

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
  },
  "McpServer": {
    "Name": "Learnify.ai Lesson Management Server",
    "Version": "1.0.0"
  }
}
```

## Logging Architecture

### Logging Configuration

- **Output**: stderr (MCP protocol requirement)
- **Levels**: Trace, Debug, Information, Warning, Error, Critical
- **Format**: Structured logging with context

### Logging Strategy

- **Information**: Service initialization, tool invocations
- **Warning**: Non-critical errors, missing resources
- **Error**: API failures, exceptions
- **Debug**: Detailed request/response information

## MCP Protocol Integration

### Tool Discovery

Tools are automatically discovered using reflection:
- Scan assembly for methods with `[McpServerToolType]` attribute
- Register tools with MCP server
- Expose tool metadata (name, description, parameters)

### Communication

- **Transport**: stdio (standard input/output)
- **Protocol**: JSON-RPC over stdio
- **Encoding**: UTF-8

## Scalability Considerations

### Stateless Design

- No server-side state
- All state managed by Learnify.ai API
- Supports horizontal scaling

### Performance

- HTTP client reuse per service
- Async/await for non-blocking I/O
- Cancellation token support
- Connection pooling

### Future Enhancements

- Response caching
- Request rate limiting
- Health checks and metrics
- Distributed tracing

## Security Architecture

### Security Considerations

- API authentication (future: API key or OAuth)
- Input validation
- Error message sanitization
- HTTPS for API communication
- Secure logging (no sensitive data)

## Deployment Architecture

### Deployment Options

1. **Local Development**: `dotnet run`
2. **Standalone Executable**: `dotnet publish`
3. **Docker Container**: Containerized deployment (future)
4. **Cloud Deployment**: Azure/AWS deployment (future)

### Deployment Process

1. Build: `dotnet build`
2. Publish: `dotnet publish -c Release`
3. Configure: Update appsettings.json
4. Deploy: Copy published files to deployment location
5. Run: Execute the application

## Features Overview

### Current Features (5)

1. **Lessons** (16 tools) - Lesson content management
2. **Courses** (15 tools) - Course lifecycle management
3. **Categories** (17 tools) - Category hierarchy management
4. **Quizzes** (18 tools) - Quiz and assessment management
5. **Answers** (16 tools) - Answer option management

**Total: 82 MCP tools**

### Feature Capabilities

Each feature provides:
- CRUD operations
- Filtering and pagination
- Relationship management
- Analytics and statistics
- Utility operations

## Design Decisions

### Why Vertical Slice Architecture?

- Promotes feature independence
- Simplifies code navigation
- Reduces coupling between features
- Enables parallel development

### Why MCP Protocol?

- Standard protocol for AI assistants
- Enables natural language interaction
- Tool discovery and metadata
- Protocol compliance

### Why BaseApiService?

- Code reuse across features
- Consistent error handling
- Centralized HTTP client management
- Unified logging

## Future Architecture Enhancements

### Planned Improvements

1. **Authentication**: API key/OAuth support
2. **Caching**: Response caching layer
3. **Rate Limiting**: Request rate limiting
4. **Monitoring**: Health checks and metrics
5. **Testing**: Comprehensive test suite
6. **Documentation**: Auto-generated API docs
7. **Docker**: Containerized deployment
8. **CI/CD**: Automated deployment pipeline

## Resources

- Feature READMEs: `Features/*/README.md`
- MCP Protocol: https://modelcontextprotocol.io/
- .NET 8.0 Docs: https://learn.microsoft.com/dotnet/core/

