namespace ApiAggregator.Api.Models;

public record AggregatedItem(
    string Source,
    string Title,
    string? Url,
    string? Description,
    DateTimeOffset PublishedAt,
    string Category,
    double Score);