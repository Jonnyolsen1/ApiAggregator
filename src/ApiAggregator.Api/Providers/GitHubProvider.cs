using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ApiAggregator.Api.Models;
namespace ApiAggregator.Api.Providers;

public class GitHubProvider(HttpClient http) : IApiProvider
{
    public string Name => "GitHub";
    public string CacheKeyFor(ProviderRequest request) => request.Keyword.Trim().ToLowerInvariant();
    public async Task<IReadOnlyList<AggregatedItem>> FetchAsync(
    ProviderRequest request, CancellationToken ct)
    {
        var url = $"search/repositories?q={Uri.EscapeDataString(request.Keyword)}&sort=stars&per_page=20"; 
        var response = await http.GetFromJsonAsync<GitHubSearchResponse>(url, ct);
        return response?.Items
        .Select(r => new AggregatedItem(
        Source: Name,
        Title: r.FullName,
        Url: r.HtmlUrl,
        Description: r.Description,
        PublishedAt: r.CreatedAt,
        Category: "code",
        Score: r.StargazersCount))
        .ToList() ?? [];
    }
}
// DTOs: JSON names are matched case-insensitively;
// snake_case names need [JsonPropertyName].
public record GitHubSearchResponse(List<GitHubRepo> Items);
public record GitHubRepo(
[property: JsonPropertyName("full_name")] string FullName,
[property: JsonPropertyName("html_url")] string HtmlUrl,
string? Description,
[property: JsonPropertyName("stargazers_count")] int StargazersCount,
[property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);