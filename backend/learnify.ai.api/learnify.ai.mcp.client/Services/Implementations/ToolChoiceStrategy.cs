using learnify.ai.mcp.client.Services.Interfaces;

namespace learnify.ai.mcp.client.Services.Implementations;

/// <summary>
/// Determines when tool usage should be forced based on user messages
/// </summary>
public class ToolChoiceStrategy : IToolChoiceStrategy
{
    public bool ShouldForceToolUsage(string userMessage)
    {
        var lowerMessage = userMessage.ToLowerInvariant();
        var educationalKeywords = new[]
        {
            "course", "cours", "lesson", "leçon", "quiz", "test", "category", "catégorie",
            "create", "créer", "update", "modifier", "delete", "supprimer", "list", "lister",
            "answer", "réponse", "student", "étudiant", "instructor", "professeur"
        };
        return educationalKeywords.Any(lowerMessage.Contains);
    }
}

