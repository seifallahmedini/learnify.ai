using System.Text.Json.Serialization;

namespace Learnify.Mcp.Server.Shared.Models;

/// <summary>
/// Base API response wrapper following Learnify.ai API patterns
/// </summary>
public record ApiResponse<T>(
    [property: JsonPropertyName("data")] T Data,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("success")] bool Success = true,
    [property: JsonPropertyName("errors")] string[]? Errors = null
);

/// <summary>
/// Paginated response for list endpoints
/// </summary>
public record PaginatedResponse<T>(
    [property: JsonPropertyName("items")] IEnumerable<T> Items,
    [property: JsonPropertyName("totalCount")] int TotalCount,
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("pageSize")] int PageSize,
    [property: JsonPropertyName("totalPages")] int TotalPages,
    [property: JsonPropertyName("hasNextPage")] bool HasNextPage,
    [property: JsonPropertyName("hasPreviousPage")] bool HasPreviousPage
);

/// <summary>
/// Standard error response format
/// </summary>
public record ErrorResponse(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("details")] string? Details = null,
    [property: JsonPropertyName("timestamp")] DateTime Timestamp = default
)
{
    public ErrorResponse(string error, string? details = null) : this(error, details, DateTime.UtcNow) { }
}

/// <summary>
/// Filter request base class for consistency
/// </summary>
public abstract record BaseFilterRequest(
    [property: JsonPropertyName("page")] int Page = 1,
    [property: JsonPropertyName("pageSize")] int PageSize = 10,
    [property: JsonPropertyName("searchTerm")] string? SearchTerm = null
);