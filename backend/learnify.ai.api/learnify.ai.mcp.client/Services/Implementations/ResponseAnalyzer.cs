using learnify.ai.mcp.client.Services.Interfaces;

namespace learnify.ai.mcp.client.Services.Implementations;

/// <summary>
/// Analyzes AI responses to determine if they are error explanations
/// </summary>
public class ResponseAnalyzer : IResponseAnalyzer
{
    public bool IsErrorExplanation(string response)
    {
        var lowerResponse = response.ToLowerInvariant();
        var explanatoryPhrases = new[]
        {
            "failed because", "you need to", "please provide", "is required",
            "to proceed", "would you like to", "here are some example",
            "missing", "cannot be", "error", "problem", "issue"
        };

        var actionPhrases = new[]
        {
            "i've created", "successfully", "here is", "completed", "i'll",
            "i have created", "creation was successful", "i created", "done"
        };

        var hasExplanatoryPhrases = explanatoryPhrases.Any(lowerResponse.Contains);
        var hasActionPhrases = actionPhrases.Any(lowerResponse.Contains);

        // Consider it an explanation if it has explanatory phrases but no action phrases
        // OR if it's asking questions instead of providing results
        var isAskingQuestions = lowerResponse.Contains("would you like") || 
                               lowerResponse.Contains("do you want") || 
                               lowerResponse.Contains("please specify") ||
                               lowerResponse.Contains("?");

        return (hasExplanatoryPhrases && !hasActionPhrases) || isAskingQuestions;
    }
}

