using System.Net.Http.Json;
using ApiAggregator.Api.Models;
using Microsoft.Extensions.Options;

namespace ApiAggregator.Api.Providers;

public class NewsApiProvider(HttpClient http, IOptionsMonitor<ProviderSettings> settings) : IApiProvider
{
    public string Name => "NewsApi";

    public string CacheKeyFor(ProviderRequest request) => request.Keyword.Trim().ToLowerInvariant();

    public async Task<IReadOnlyList<AggregatedItem>> FetchAsync(
        ProviderRequest request, CancellationToken ct)
    {
        // News API wants the key in a header, so build the request message yourself
        var apiKey = settings.Get(Name).ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("News API key is not configured");

        var url = $"v2/everything?q={Uri.EscapeDataString(request.Keyword)}"
                + "&language=en&sortBy=publishedAt&pageSize=20";
        using var message = new HttpRequestMessage(HttpMethod.Get, url);
        message.Headers.Add("X-Api-Key", apiKey);

        using var response = await http.SendAsync(message, ct);
        response.EnsureSuccessStatusCode();   // non-2xx → HttpRequestException
        var body = await response.Content.ReadFromJsonAsync<NewsApiResponse>(ct);

        return body?.Articles
            .Where(a => a.Title is not null && a.Title != "[Removed]")
            .Select(a => new AggregatedItem(Name, a.Title!, a.Url, a.Description, a.PublishedAt, "news", 0))
            .ToList() ?? [];
    }
}

// DTOs
public record NewsApiResponse(List<NewsArticle> Articles);
public record NewsArticle(NewsSource? Source, string? Title, string? Description, string? Url,
                          DateTimeOffset PublishedAt);
public record NewsSource(string? Name);