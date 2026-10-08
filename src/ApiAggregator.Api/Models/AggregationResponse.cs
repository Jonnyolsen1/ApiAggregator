namespace ApiAggregator.Api.Models;

public record AggregationResponse(
    string Query,
    string City,
    DateTimeOffset GeneratedAt,
    IReadOnlyList<SourceResult> Sources,
    IReadOnlyList<AggregatedItem> Items);