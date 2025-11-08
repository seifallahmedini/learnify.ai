using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.Collections.Concurrent;
using System.Reflection;
using System.ComponentModel;
using System.Text.Json;

namespace Learnify.Mcp.Server.Services;

/// <summary>
/// In-process MCP tool invoker that discovers and executes tools without stdio/process communication
/// </summary>
public class InProcessMcpToolInvoker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InProcessMcpToolInvoker> _logger;
    
    // Cache for discovered tools to avoid reflection overhead
    private readonly ConcurrentDictionary<string, ToolInfo> _tools = new();
    private bool _isDiscovered = false;
    private readonly object _discoveryLock = new();

    public InProcessMcpToolInvoker(IServiceProvider serviceProvider, ILogger<InProcessMcpToolInvoker> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Discover all available tools from registered services
    /// </summary>
    public async Task<List<string>> GetAvailableToolsAsync()
    {
        await EnsureToolsDiscoveredAsync();
        return _tools.Keys.ToList();
    }

    /// <summary>
    /// Get tools with their parameter schemas
    /// </summary>
    public async Task<Dictionary<string, object>> GetToolsWithParametersAsync()
    {
        await EnsureToolsDiscoveredAsync();
        
        var result = new Dictionary<string, object>();
        
        foreach (var kvp in _tools)
        {
            var toolInfo = kvp.Value;
            var toolData = new Dictionary<string, object>
            {
                ["description"] = toolInfo.Description,
                ["inputSchema"] = GenerateInputSchema(toolInfo.Method)
            };
            result[kvp.Key] = toolData;
        }
        
        return result;
    }

    /// <summary>
    /// Call a tool by name with the provided arguments
    /// </summary>
    public async Task<string> CallToolAsync(string toolName, object? arguments = null)
    {
        await EnsureToolsDiscoveredAsync();
        
        if (!_tools.TryGetValue(toolName, out var toolInfo))
        {
            var errorResponse = new { success = false, message = $"Tool '{toolName}' not found" };
            return JsonSerializer.Serialize(errorResponse);
        }

        try
        {
            _logger.LogInformation("Calling tool: {ToolName} with arguments: {Arguments}", 
                toolName, JsonSerializer.Serialize(arguments ?? new object()));

            // Resolve the service instance
            var serviceInstance = _serviceProvider.GetRequiredService(toolInfo.ServiceType);
            
            // Convert arguments to method parameters
            var parameters = ConvertArgumentsToParameters(toolInfo.Method, arguments);
            
            // Invoke the method
            var result = toolInfo.Method.Invoke(serviceInstance, parameters);
            
            // Handle async methods
            if (result is Task task)
            {
                await task;
                
                // Get the result from Task<T>
                if (task.GetType().IsGenericType)
                {
                    var property = task.GetType().GetProperty("Result");
                    result = property?.GetValue(task);
                }
                else
                {
                    result = ""; // Task without return value
                }
            }
            
            // Ensure result is a string (should be JSON from our tools)
            return result?.ToString() ?? JsonSerializer.Serialize(new { success = false, message = "Tool returned null result" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling tool: {ToolName}", toolName);
            var errorResponse = new { success = false, message = ex.Message };
            return JsonSerializer.Serialize(errorResponse);
        }
    }

    private async Task EnsureToolsDiscoveredAsync()
    {
        if (_isDiscovered) return;
        
        lock (_discoveryLock)
        {
            if (_isDiscovered) return;
            
            DiscoverTools();
            _isDiscovered = true;
        }
        
        await Task.CompletedTask;
    }

    private void DiscoverTools()
    {
        _logger.LogInformation("Discovering MCP tools...");
        
        // Get all registered service types that have the McpServerToolType attribute
        var assembly = Assembly.GetExecutingAssembly();
        var toolServiceTypes = assembly.GetTypes()
            .Where(type => type.GetCustomAttribute<McpServerToolTypeAttribute>() != null)
            .ToList();

        foreach (var serviceType in toolServiceTypes)
        {
            try
            {
                // Get all methods with McpServerTool attribute
                var toolMethods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(method => method.GetCustomAttribute<McpServerToolAttribute>() != null)
                    .ToList();

                foreach (var method in toolMethods)
                {
                    var toolName = method.Name;
                    var description = method.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "No description available";
                    
                    _tools[toolName] = new ToolInfo(serviceType, method, description);
                    _logger.LogDebug("Discovered tool: {ToolName} from service: {ServiceType}", toolName, serviceType.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to discover tools from service type: {ServiceType}", serviceType.Name);
            }
        }
        
        _logger.LogInformation("Discovered {ToolCount} tools", _tools.Count);
    }

    private static object?[] ConvertArgumentsToParameters(MethodInfo method, object? arguments)
    {
        var parameters = method.GetParameters();
        var result = new object?[parameters.Length];
        
        // Handle case where no arguments provided
        if (arguments == null)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                if (param.HasDefaultValue)
                {
                    result[i] = param.DefaultValue;
                }
                else if (param.ParameterType == typeof(CancellationToken))
                {
                    result[i] = CancellationToken.None;
                }
                else if (param.ParameterType.IsValueType)
                {
                    result[i] = Activator.CreateInstance(param.ParameterType);
                }
                else
                {
                    result[i] = null;
                }
            }
            return result;
        }

        // Convert arguments (expected to be a dictionary or JSON object)
        Dictionary<string, object?>? argumentsDict = null;
        
        if (arguments is JsonElement jsonElement)
        {
            argumentsDict = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonElement.GetRawText());
        }
        else if (arguments is Dictionary<string, object?> dict)
        {
            argumentsDict = dict;
        }
        else
        {
            // Try to deserialize as JSON
            var json = JsonSerializer.Serialize(arguments);
            argumentsDict = JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
        }
        
        // Map arguments to parameters by name
        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var paramName = param.Name!;
            
            if (argumentsDict != null && argumentsDict.TryGetValue(paramName, out var argValue))
            {
                result[i] = ConvertArgumentValue(argValue, param.ParameterType);
            }
            else if (param.HasDefaultValue)
            {
                result[i] = param.DefaultValue;
            }
            else if (param.ParameterType == typeof(CancellationToken))
            {
                result[i] = CancellationToken.None;
            }
            else if (param.ParameterType.IsValueType)
            {
                result[i] = Activator.CreateInstance(param.ParameterType);
            }
            else
            {
                result[i] = null;
            }
        }
        
        return result;
    }

    private static object? ConvertArgumentValue(object? value, Type targetType)
    {
        if (value == null)
            return null;

        // Handle CancellationToken
        if (targetType == typeof(CancellationToken))
            return CancellationToken.None;

        // Handle nullable types
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType)!;
            return ConvertArgumentValue(value, underlyingType);
        }

        // If types match, return as-is
        if (targetType.IsAssignableFrom(value.GetType()))
            return value;

        // Handle JsonElement conversion
        if (value is JsonElement jsonElement)
        {
            return JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType);
        }

        // Try direct conversion
        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            // If conversion fails, try JSON serialization/deserialization
            var json = JsonSerializer.Serialize(value);
            return JsonSerializer.Deserialize(json, targetType);
        }
    }

    private static object GenerateInputSchema(MethodInfo method)
    {
        var parameters = method.GetParameters()
            .Where(p => p.ParameterType != typeof(CancellationToken))
            .ToList();

        var properties = new Dictionary<string, object>();
        var required = new List<string>();

        foreach (var param in parameters)
        {
            var description = param.GetCustomAttribute<DescriptionAttribute>()?.Description ?? $"Parameter {param.Name}";
            var isRequired = !param.HasDefaultValue && !param.ParameterType.IsGenericType;

            var paramSchema = new Dictionary<string, object>
            {
                ["description"] = description,
                ["type"] = GetJsonSchemaType(param.ParameterType)
            };

            properties[param.Name!] = paramSchema;

            if (isRequired)
            {
                required.Add(param.Name!);
            }
        }

        return new
        {
            type = "object",
            properties,
            required = required.ToArray()
        };
    }

    private static string GetJsonSchemaType(Type type)
    {
        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type)!;
        }

        return Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => "boolean",
            TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or 
            TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 => "integer",
            TypeCode.Single or TypeCode.Double or TypeCode.Decimal => "number",
            TypeCode.String or TypeCode.Char => "string",
            _ => "string" // Default to string for complex types
        };
    }

    private record ToolInfo(Type ServiceType, MethodInfo Method, string Description);
}