using System.Text.Json;
using learnify.ai.mcp.client.Services.Interfaces;

namespace learnify.ai.mcp.client.Services.Implementations;

/// <summary>
/// Enhances error results with AI-friendly context and suggestions
/// </summary>
public class ErrorEnhancer : IErrorEnhancer
{
    public object EnhanceError(object originalResult, string toolName, object? arguments)
    {
        try
        {
            var resultJson = JsonSerializer.Serialize(originalResult);
            using var document = JsonDocument.Parse(resultJson);
            var root = document.RootElement;

            var enhanced = new Dictionary<string, object>
            {
                ["originalResult"] = originalResult,
                ["toolName"] = toolName,
                ["attemptedArguments"] = arguments ?? new { },
            };

            // Extract error details and provide AI-friendly suggestions
            if (root.TryGetProperty("details", out var detailsElement))
            {
                var details = detailsElement.GetString() ?? "";
                enhanced["errorDetails"] = details;
                enhanced["suggestions"] = GenerateRetrySuggestions(details, toolName);
            }
            else if (root.TryGetProperty("message", out var messageElement))
            {
                var message = messageElement.GetString() ?? "";
                enhanced["errorMessage"] = message;
                enhanced["suggestions"] = GenerateRetrySuggestions(message, toolName);
            }

            enhanced["retryInstructions"] = GetRetryInstructions(toolName);

            return enhanced;
        }
        catch
        {
            return originalResult;
        }
    }

    private static List<string> GenerateRetrySuggestions(string errorDetails, string toolName)
    {
        var suggestions = new List<string>();
        var lowerError = errorDetails.ToLowerInvariant();

        if (lowerError.Contains("learning objectives") && lowerError.Contains("required"))
        {
            suggestions.Add("Generate appropriate learning objectives based on the course title and description");
            suggestions.Add("Example: 'Understand key concepts, Apply practical skills, Analyze real-world scenarios'");
        }

        if (lowerError.Contains("short description") && lowerError.Contains("required"))
        {
            suggestions.Add("Create a brief summary from the main description (1-2 sentences)");
        }

        if (lowerError.Contains("title") && lowerError.Contains("required"))
        {
            suggestions.Add("Ensure a descriptive title is provided");
        }

        if (lowerError.Contains("description") && lowerError.Contains("required"))
        {
            suggestions.Add("Provide a detailed course description");
        }

        if (lowerError.Contains("price") && lowerError.Contains("negative"))
        {
            suggestions.Add("Use a positive price value or 0 for free courses");
        }

        if (lowerError.Contains("level") && lowerError.Contains("invalid"))
        {
            suggestions.Add("Use valid level: 1=Beginner, 2=Intermediate, 3=Advanced, 4=Expert");
        }

        if (lowerError.Contains("instructor") && lowerError.Contains("not found"))
        {
            suggestions.Add("Check if the instructor ID exists or use a valid instructor ID");
        }

        if (lowerError.Contains("category") && lowerError.Contains("not found"))
        {
            suggestions.Add("Check if the category ID exists or use a valid category ID");
        }

        if (suggestions.Count == 0)
        {
            suggestions.Add("Review the error message and correct the invalid parameters");
            suggestions.Add("Ensure all required fields are provided with valid values");
        }

        return suggestions;
    }

    private static string GetRetryInstructions(string toolName)
    {
        return toolName.ToLowerInvariant() switch
        {
            var name when name.Contains("create") && name.Contains("course") =>
                "For course creation, ensure: title, description, instructorId, categoryId, price (≥0), durationHours, level (1-4), language. Generate learningObjectives if missing.",
            
            var name when name.Contains("update") && name.Contains("course") =>
                "For course updates, provide the courseId and any fields to update with valid values.",
            
            var name when name.Contains("create") && name.Contains("lesson") =>
                "For lesson creation, ensure: courseId, title, description, content. Duration and other fields are optional.",
                
            _ => "Check the tool parameters and ensure all required fields are provided with valid values."
        };
    }
}

