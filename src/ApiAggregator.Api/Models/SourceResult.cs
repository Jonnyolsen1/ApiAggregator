namespace ApiAggregator.Api.Models;

public record SourceResult(
    string Name, 
    SourceStatus Status,
    int ItemCount,
    long? ElapsedMs,
    string? Error);