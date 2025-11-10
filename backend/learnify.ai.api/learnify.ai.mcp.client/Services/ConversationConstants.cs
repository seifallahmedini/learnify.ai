namespace learnify.ai.mcp.client.Services;

/// <summary>
/// Constants used in conversation processing
/// </summary>
internal static class ConversationConstants
{
    public const int MaxIterations = 25;
    public const float DefaultTemperature = 0.7f;
    
    public const string ErrorExplanationPrompt = 
        "STOP explaining errors! You must use tools to complete the user's request. " +
        "Call the appropriate tool again RIGHT NOW with corrected parameters. " +
        "Generate any missing required information based on the context and user's request. " +
        "DO NOT respond with text - use tools only!";
    
    public const string ActionRequiredPrompt = 
        "The user's request requires using tools to take action. Stop explaining and use the appropriate tools to complete their request. " +
        "Generate any missing information and call the tools now.";
    
    public const string MultipleFailuresPrompt = 
        "You have encountered multiple tool failures. IMPORTANT: Do not give up! Instead of explaining the errors to the user, you must:" +
        "1) ANALYZE the error details carefully" +
        "2) GENERATE the missing required information (like learningObjectives, shortDescription, etc.)" +
        "3) CALL THE TOOL AGAIN with ALL required parameters filled in" +
        "4) Only ask the user for help if you absolutely cannot infer the missing information" +
        "Your goal is to COMPLETE the user's request, not explain why it failed. Use your creativity to fill in reasonable defaults.";
    
    public const string EmptyResponseMessage = 
        "I couldn't generate an appropriate response. Please try rephrasing your request.";
    
    public const string MaxIterationsReachedMessage = 
        "I couldn't complete your request in the allowed time. The request may be too complex or require additional information.";
    
    public const string TechnicalDifficultyMessage = 
        "I'm experiencing technical difficulties. Please try again later.";
}

