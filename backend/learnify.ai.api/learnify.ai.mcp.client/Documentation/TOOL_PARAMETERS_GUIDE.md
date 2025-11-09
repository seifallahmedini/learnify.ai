# MCP Tool Parameters and Execution Guide

## Available MCP Methods in LearnifyMcpService

### 1. **Tool Discovery**
```csharp
// Get simple list of tool names
var tools = await mcpService.GetAvailableToolsAsync();

// Get detailed tool information with parameters
var toolsWithParams = await mcpService.GetToolsWithParametersAsync();

// Get parameters for a specific tool
var toolParams = await mcpService.GetToolParametersAsync("create_course");
```

### 2. **Tool Execution**
```csharp
// Execute a tool with parameters
var result = await mcpService.CallToolAsync("create_course", new {
    title = "React Fundamentals",
    description = "Learn React from scratch",
    instructor_id = 123,
    price = 99.99
});

// Execute a tool without parameters
var courses = await mcpService.CallToolAsync("get_courses");
```

### 3. **Resource Access**
```csharp
// Get available resources (if supported by server)
var resources = await mcpService.GetAvailableResourcesAsync();
```

## Azure Function Endpoints

### **GET /api/GetAvailableTools**
Returns simple list of tool names
```bash
curl http://localhost:7071/api/GetAvailableTools
```

### **GET /api/GetToolsWithParameters**
Returns detailed tool information including input schemas
```bash
curl http://localhost:7071/api/GetToolsWithParameters
```

### **GET /api/GetToolParameters?toolName=create_course**
Returns parameters for a specific tool
```bash
curl "http://localhost:7071/api/GetToolParameters?toolName=create_course"
```

### **POST /api/CallTool**
Execute a tool with parameters
```bash
curl -X POST http://localhost:7071/api/CallTool \
  -H "Content-Type: application/json" \
  -d '{
    "toolName": "create_course",
    "arguments": {
      "title": "JavaScript Basics",
      "description": "Learn JavaScript fundamentals"
    }
  }'
```

### **GET /api/GetAvailableResources**
Get available resources (if supported)
```bash
curl http://localhost:7071/api/GetAvailableResources
```

## Expected Tool Parameter Schema Format

When you call `GetToolsWithParametersAsync()`, you'll get responses like:

```json
{
  "create_course": {
    "description": "Create a new course",
    "inputSchema": {
      "type": "object",
      "properties": {
        "title": {
          "type": "string",
          "description": "Course title"
        },
        "description": {
          "type": "string", 
          "description": "Course description"
        },
        "instructor_id": {
          "type": "integer",
          "description": "ID of the instructor"
        },
        "price": {
          "type": "number",
          "description": "Course price"
        }
      },
      "required": ["title", "description"]
    }
  }
}
```

## Common Educational Tools and Their Parameters

Based on your MCP server, you might have tools like:

### **Course Management**
- `get_courses` - No parameters
- `create_course` - title, description, instructor_id, price, etc.
- `update_course` - course_id + fields to update
- `delete_course` - course_id

### **Lesson Management**
- `get_lessons` - Optional: course_id filter
- `create_lesson` - course_id, title, content, order, etc.
- `update_lesson` - lesson_id + fields to update
- `delete_lesson` - lesson_id

### **Category Management**
- `get_categories` - No parameters
- `create_category` - name, description, parent_id
- `update_category` - category_id + fields to update

### **Quiz Management**
- `get_quizzes` - Optional: course_id filter
- `create_quiz` - course_id, title, questions, etc.
- `update_quiz` - quiz_id + fields to update

## Testing Tool Parameters

Use the enhanced TestConsole:

```bash
cd learnify.ai.mcp.test
dotnet run
```

This will:
1. ✅ Initialize MCP connection
2. ✅ Get health status
3. ✅ List all available tools
4. ✅ **Get tool parameters for first 3 tools**
5. ✅ **Execute a test tool**
6. ✅ Check for available resources
7. ✅ Test educational request processing

## Error Handling

The service handles these scenarios:
- **Tool not found**: `ArgumentException`
- **Invalid parameters**: MCP server returns error in response
- **Connection issues**: `InvalidOperationException`
- **Server errors**: Logged and re-thrown with context

## Next Steps

1. **Discover your tools**: Run the test to see what tools are available
2. **Examine parameters**: Check the input schemas for each tool
3. **Test execution**: Try calling tools with appropriate parameters
4. **Build AI workflows**: Use tool results in your educational AI assistant