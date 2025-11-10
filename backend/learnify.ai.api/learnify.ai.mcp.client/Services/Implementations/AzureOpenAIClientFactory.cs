using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using learnify.ai.mcp.client.Services.Interfaces;

namespace learnify.ai.mcp.client.Services.Implementations;

/// <summary>
/// Factory for creating Azure OpenAI chat clients
/// </summary>
public class AzureOpenAIClientFactory : IAzureOpenAIClientFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureOpenAIClientFactory> _logger;

    public AzureOpenAIClientFactory(
        IConfiguration configuration,
        ILogger<AzureOpenAIClientFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public ChatClient CreateChatClient()
    {
        var endpoint = _configuration["AzureOpenAI:Endpoint"];
        var apiKey = _configuration["AzureOpenAI:Key"];
        var deploymentName = _configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4";

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Azure OpenAI configuration is missing. Endpoint configured: {EndpointConfigured}, ApiKey configured: {KeyConfigured}",
                !string.IsNullOrEmpty(endpoint), !string.IsNullOrEmpty(apiKey));
            throw new InvalidOperationException("Azure OpenAI configuration is missing. Please configure Endpoint and ApiKey in the application settings.");
        }

        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        return client.GetChatClient(deploymentName);
    }
}

