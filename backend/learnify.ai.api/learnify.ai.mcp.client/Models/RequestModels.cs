using System.ComponentModel.DataAnnotations;

namespace learnify.ai.mcp.client.Models;

/// <summary>
/// Request model for educational queries
/// </summary>
public class EducationalRequest
{
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public string? SystemPrompt { get; set; }
    
    public EducationalRequestType RequestType { get; set; } = EducationalRequestType.General;
    
    public Dictionary<string, object>? Context { get; set; }
}

/// <summary>
/// Response model for educational queries
/// </summary>
public class EducationalResponse
{
    public bool Success { get; set; }
    
    public string Response { get; set; } = string.Empty;
    
    public List<string>? ToolsUsed { get; set; }
    
    public TimeSpan ProcessingTime { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request model for analytics queries
/// </summary>
public class AnalyticsRequest
{
    [Required]
    public string Query { get; set; } = string.Empty;
    
    public AnalyticsType Type { get; set; } = AnalyticsType.General;
    
    public int? CourseId { get; set; }
    
    public int? CategoryId { get; set; }
    
    public int? QuizId { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// Request model for content creation
/// </summary>
public class ContentCreationRequest
{
    [Required]
    public string Description { get; set; } = string.Empty;
    
    public ContentType ContentType { get; set; }
    
    public int? ParentId { get; set; }
    
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// Request model for assessment management
/// </summary>
public class AssessmentRequest
{
    [Required]
    public string Description { get; set; } = string.Empty;
    
    public AssessmentOperation Operation { get; set; }
    
    public int? QuizId { get; set; }
    
    public int? QuestionId { get; set; }
    
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// Service health status response
/// </summary>
public class ServiceHealthResponse
{
    public bool IsHealthy { get; set; }
    
    public string Status { get; set; } = string.Empty;
    
    public DateTime LastChecked { get; set; }
    
    public ConnectionStatus OpenAI { get; set; } = new();
    
    public ConnectionStatus McpServer { get; set; } = new();
    
    public int AvailableToolsCount { get; set; }
    
    public List<string>? AvailableTools { get; set; }
    
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Connection status model
/// </summary>
public class ConnectionStatus
{
    public bool Connected { get; set; }
    
    public string Status { get; set; } = string.Empty;
    
    public DateTime? LastConnected { get; set; }
    
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Bulk operation request model
/// </summary>
public class BulkOperationRequest
{
    [Required]
    public string Operation { get; set; } = string.Empty;
    
    [Required]
    public List<int> EntityIds { get; set; } = new();
    
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// Educational workflow request model
/// </summary>
public class WorkflowRequest
{
    [Required]
    public string WorkflowType { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    public List<WorkflowStep>? Steps { get; set; }
    
    public Dictionary<string, object>? Configuration { get; set; }
}

/// <summary>
/// Workflow step model
/// </summary>
public class WorkflowStep
{
    public int Order { get; set; }
    
    public string Action { get; set; } = string.Empty;
    
    public Dictionary<string, object>? Parameters { get; set; }
    
    public bool IsRequired { get; set; } = true;
}

#region Enums

/// <summary>
/// Educational request types
/// </summary>
public enum EducationalRequestType
{
    General = 0,
    Analytics = 1,
    ContentCreation = 2,
    Assessment = 3,
    CourseManagement = 4,
    StudentProgress = 5,
    Reporting = 6
}

/// <summary>
/// Analytics types
/// </summary>
public enum AnalyticsType
{
    General = 0,
    CoursePerformance = 1,
    StudentProgress = 2,
    QuizAnalytics = 3,
    AnswerAnalytics = 4,
    CategoryAnalytics = 5,
    LessonEffectiveness = 6
}

/// <summary>
/// Content types for creation
/// </summary>
public enum ContentType
{
    Category = 0,
    Course = 1,
    Lesson = 2,
    Quiz = 3,
    Question = 4,
    Answer = 5,
    CompleteLearningPath = 6
}

/// <summary>
/// Assessment operations
/// </summary>
public enum AssessmentOperation
{
    Create = 0,
    Update = 1,
    Delete = 2,
    Validate = 3,
    Analyze = 4,
    Optimize = 5,
    BulkOperation = 6
}

#endregion