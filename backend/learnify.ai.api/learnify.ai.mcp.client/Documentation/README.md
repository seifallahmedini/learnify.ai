# ?? Learnify.ai MCP Azure Function Client

## Overview

This Azure Function provides a cloud-based interface to the Learnify.ai MCP (Model Context Protocol) server, enabling AI-powered educational management through RESTful APIs. It integrates Azure OpenAI with the comprehensive Learnify.ai MCP server to provide intelligent educational assistance.

## Features

### ?? **Core Capabilities**
- **AI-Powered Educational Assistant**: Natural language interface to all educational operations
- **Azure OpenAI Integration**: Leverages GPT-4 for intelligent responses
- **MCP Tool Integration**: Access to 82 educational management tools
- **RESTful API**: Standard HTTP endpoints for easy integration

### ??? **Available Endpoints**

#### 1. **Health Check** (`GET /api/Health`)
- Monitor service health and connections
- Check Azure OpenAI and MCP server connectivity
- View available tools count

#### 2. **Educational Assistant** (`POST /api/EducationalAssistant`)
- General-purpose educational AI assistant
- Handles any educational request using natural language
- Automatically selects and uses appropriate MCP tools

#### 3. **Educational Analytics** (`POST /api/EducationalAnalytics`)
- Specialized analytics and reporting endpoint
- Performance insights across courses, quizzes, and students
- Data-driven educational recommendations

#### 4. **Create Educational Content** (`POST /api/CreateEducationalContent`)
- AI-assisted content creation
- Creates categories, courses, lessons, quizzes, questions, and answers
- Follows educational best practices

#### 5. **Manage Assessments** (`POST /api/ManageAssessments`)
- Comprehensive assessment management
- Quiz, question, and answer operations
- Assessment validation and optimization

#### 6. **Educational Workflow** (`POST /api/EducationalWorkflow`)
- Complex multi-step educational operations
- Automated workflow execution
- End-to-end educational process management

#### 7. **Get Available Tools** (`GET /api/GetAvailableTools`)
- List all available MCP tools
- Tool categorization and counts
- Development and debugging support

## Architecture

### ??? **Service Structure**

```
learnify.ai.mcp.client/
??? Services/
?   ??? LearnifyMcpService.cs    # Core MCP integration service
??? Models/
?   ??? RequestModels.cs         # Request/response DTOs
??? Function1.cs                 # Azure Function endpoints
??? Program.cs                   # Dependency injection setup
??? local.settings.json          # Configuration
```

### ?? **Integration Flow**

```
Client Request ? Azure Function ? LearnifyMcpService ? Azure OpenAI + MCP Server ? Response
```

1. **Client** sends HTTP request to Azure Function
2. **Azure Function** validates and processes request
3. **LearnifyMcpService** coordinates with Azure OpenAI and MCP server
4. **Azure OpenAI** provides AI reasoning and tool selection
5. **MCP Server** executes educational operations
6. **Response** returned with results and metadata

## Configuration

### ?? **Required Settings**

Update `local.settings.json` with your configuration:

```json
{
  "Values": {
    "AzureOpenAI:Endpoint": "https://YOUR_RESOURCE_NAME.openai.azure.com/",
    "AzureOpenAI:Key": "YOUR_AZURE_OPENAI_KEY",
    "AzureOpenAI:DeploymentName": "gpt-4",
    
    "LearnifyMcp:TransportType": "PROCESS",
    "LearnifyMcp:ServerPath": "../learnify.ai.mcp.server/bin/Debug/net8.0/learnify.ai.mcp.server.exe"
  }
}
```

### ?? **Prerequisites**

1. **Azure OpenAI Resource**
   - Azure OpenAI service deployed
   - GPT-4 model deployed
   - Valid API key and endpoint

2. **Learnify.ai MCP Server**
   - MCP server built and available
   - Learnify.ai API running on `http://localhost:5271`

3. **Development Environment**
   - .NET 8 SDK
   - Azure Functions Core Tools
   - Visual Studio or VS Code

## Usage Examples

### ?? **Educational Assistant**

```bash
POST /api/EducationalAssistant
Content-Type: application/json

{
  "message": "Create a comprehensive JavaScript course with 5 lessons and quizzes",
  "requestType": "ContentCreation"
}
```

### ?? **Educational Analytics**

```bash
POST /api/EducationalAnalytics
Content-Type: application/json

{
  "query": "Show me performance analytics for all Programming courses",
  "type": "CoursePerformance",
  "categoryId": 1
}
```

### ?? **Assessment Management**

```bash
POST /api/ManageAssessments
Content-Type: application/json

{
  "description": "Create a JavaScript fundamentals quiz with 10 multiple choice questions",
  "operation": "Create",
  "parameters": {
    "courseId": 1,
    "questionCount": 10,
    "questionType": "MultipleChoice"
  }
}
```

### ?? **Complex Workflows**

```bash
POST /api/EducationalWorkflow
Content-Type: application/json

{
  "workflowType": "CompleteCourseCreation",
  "description": "Create a Data Science learning path from category to assessments",
  "steps": [
    { "order": 1, "action": "CreateCategory", "parameters": { "name": "Data Science" } },
    { "order": 2, "action": "CreateCourse", "parameters": { "title": "Python for Data Science" } },
    { "order": 3, "action": "CreateLessons", "parameters": { "count": 8 } },
    { "order": 4, "action": "CreateAssessments", "parameters": { "quizCount": 3 } }
  ]
}
```

## Response Format

### ? **Success Response**

```json
{
  "success": true,
  "response": "Course created successfully with ID 1. Added 5 lessons and 3 quizzes with comprehensive assessments.",
  "processingTime": "00:00:03.1234567",
  "toolsUsed": ["CreateCourseAsync", "CreateCourseLessonAsync", "CreateQuizAsync"],
  "metadata": {
    "contentType": "Course",
    "courseId": 1
  }
}
```

### ? **Error Response**

```json
{
  "success": false,
  "errorMessage": "Failed to connect to MCP server",
  "processingTime": "00:00:01.0000000"
}
```

## Deployment

### ?? **Local Development**

1. **Clone and build the solution**
   ```bash
   git clone <repository>
   cd learnify.ai/backend/learnify.ai.api/learnify.ai.mcp.client
   dotnet restore
   dotnet build
   ```

2. **Start dependencies**
   ```bash
   # Start Learnify.ai API
   cd ../learnify.ai.api
   dotnet run
   
   # Build MCP server
   cd ../learnify.ai.mcp.server
   dotnet build
   ```

3. **Configure settings**
   - Update `local.settings.json` with your Azure OpenAI credentials
   - Verify MCP server path

4. **Run Azure Function**
   ```bash
   func start
   ```

### ?? **Azure Deployment**

1. **Create Azure Resources**
   - Azure Function App (.NET 8)
   - Azure OpenAI Service
   - Application Insights (optional)

2. **Configure Application Settings**
   ```bash
   az functionapp config appsettings set \
     --name YourFunctionApp \
     --resource-group YourResourceGroup \
     --settings \
     "AzureOpenAI:Endpoint=https://your-openai.openai.azure.com/" \
     "AzureOpenAI:Key=your-key" \
     "AzureOpenAI:DeploymentName=gpt-4"
   ```

3. **Deploy Function**
   ```bash
   func azure functionapp publish YourFunctionApp
   ```

## Advanced Features

### ?? **Service Health Monitoring**

The service provides comprehensive health checking:

```json
{
  "isHealthy": true,
  "status": "Healthy",
  "openAI": { "connected": true, "status": "Connected" },
  "mcpServer": { "connected": true, "status": "Connected" },
  "availableToolsCount": 82,
  "availableTools": ["GetCoursesAsync", "CreateQuizAsync", ...]
}
```

### ?? **Performance Tracking**

All endpoints include performance metrics:
- Processing time measurement
- Tool usage tracking
- Error rate monitoring
- Request metadata

### ?? **Security**

- Azure Function authorization levels
- API key validation
- Input sanitization
- Error message sanitization

## Integration Examples

### ?? **Frontend Integration**

```javascript
// JavaScript example
const response = await fetch('/api/EducationalAssistant', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    message: "Show me analytics for my JavaScript course",
    requestType: "Analytics"
  })
});

const result = await response.json();
console.log(result.response);
```

### ?? **API Integration**

```csharp
// C# example
var client = new HttpClient();
var request = new {
  message = "Create a quiz about Python variables",
  requestType = "Assessment"
};

var response = await client.PostAsJsonAsync(
  "/api/EducationalAssistant", 
  request
);

var result = await response.Content.ReadAsStringAsync();
```

## Troubleshooting

### ?? **Common Issues**

1. **MCP Server Connection Failed**
   - Verify server path in configuration
   - Ensure MCP server is built
   - Check Learnify.ai API is running

2. **Azure OpenAI Authentication**
   - Verify endpoint URL format
   - Check API key validity
   - Confirm deployment name

3. **Function Authorization**
   - Check authorization level settings
   - Verify function keys

### ?? **Debugging**

Enable detailed logging by updating `local.settings.json`:

```json
{
  "Values": {
    "Logging:LogLevel:Default": "Debug",
    "Logging:LogLevel:learnify.ai.mcp.client": "Debug"
  }
}
```

## Support

- **Documentation**: See feature-specific README files
- **Issues**: Check logs and health endpoint
- **Development**: Use available tools endpoint for debugging

The Azure Function provides a powerful, scalable interface to the comprehensive Learnify.ai educational ecosystem! ????