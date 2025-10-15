using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Learnify.Mcp.Server.Shared.Models;

namespace Learnify.Mcp.Server.Shared.Services;

/// <summary>
/// Base HTTP service for API communication following Learnify.ai patterns
/// </summary>
public abstract class BaseApiService
{
    protected readonly HttpClient _httpClient;
    protected readonly IConfiguration _configuration;
    protected readonly ILogger _logger;
    protected readonly string _baseUrl;
    protected readonly JsonSerializerOptions _jsonOptions;

    protected BaseApiService(
        HttpClient httpClient, 
        IConfiguration configuration, 
        ILogger logger,
        string serviceName)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _baseUrl = configuration.GetValue<string>("LearnifyApi:BaseUrl") ?? "http://localhost:5271";
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        _logger.LogInformation("Initialized {ServiceName} with base URL: {BaseUrl}", serviceName, _baseUrl);
    }

    /// <summary>
    /// Generic GET request handler with proper error handling
    /// </summary>
    protected async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var url = BuildUrl(endpoint);
            _logger.LogDebug("GET request to: {Url}", url);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Resource not found: {Url}", url);
                return null;
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
            return apiResponse?.Data;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during GET request to {Endpoint}", endpoint);
            throw new InvalidOperationException($"Failed to get data from {endpoint}: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error for GET {Endpoint}", endpoint);
            throw new InvalidOperationException($"Failed to deserialize response from {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Generic POST request handler
    /// </summary>
    protected async Task<T> PostAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var url = BuildUrl(endpoint);
            _logger.LogDebug("POST request to: {Url}", url);

            HttpContent? httpContent = null;
            if (data != null)
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.PostAsync(url, httpContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
            
            if (apiResponse?.Data == null)
                throw new InvalidOperationException($"Failed to deserialize response from {endpoint}");
                
            return apiResponse.Data;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during POST request to {Endpoint}", endpoint);
            throw new InvalidOperationException($"Failed to post data to {endpoint}: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON error during POST request to {Endpoint}", endpoint);
            throw new InvalidOperationException($"Failed to serialize/deserialize data for {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Generic PUT request handler
    /// </summary>
    protected async Task<T?> PutAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var url = BuildUrl(endpoint);
            _logger.LogDebug("PUT request to: {Url}", url);

            HttpContent? httpContent = null;
            if (data != null)
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.PutAsync(url, httpContent, cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Resource not found for PUT: {Url}", url);
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
            
            return apiResponse?.Data;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during PUT request to {Endpoint}", endpoint);
            throw new InvalidOperationException($"Failed to update data at {endpoint}: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON error during PUT request to {Endpoint}", endpoint);
            throw new InvalidOperationException($"Failed to serialize/deserialize data for {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Generic DELETE request handler
    /// </summary>
    protected async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = BuildUrl(endpoint);
            _logger.LogDebug("DELETE request to: {Url}", url);

            var response = await _httpClient.DeleteAsync(url, cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Resource not found for DELETE: {Url}", url);
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during DELETE request to {Endpoint}", endpoint);
            throw new InvalidOperationException($"Failed to delete resource at {endpoint}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Build full URL from endpoint
    /// </summary>
    private string BuildUrl(string endpoint)
    {
        return endpoint.StartsWith("http") ? endpoint : $"{_baseUrl.TrimEnd('/')}{endpoint}";
    }

    /// <summary>
    /// Build query parameters from object properties
    /// </summary>
    protected static string BuildQueryString(object? parameters)
    {
        if (parameters == null) return string.Empty;

        var queryParams = new List<string>();
        var properties = parameters.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(parameters);
            if (value != null)
            {
                var paramName = char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
                queryParams.Add($"{paramName}={Uri.EscapeDataString(value.ToString()!)}");
            }
        }

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }
}