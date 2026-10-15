using ApiAggregator.Api.Models;
using ApiAggregator.Api.Providers;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

public class OpenWeatherMapProvider(HttpClient http, IOptionsMonitor<ProviderSettings> settings)
: IApiProvider
{
    public string Name => "OpenWeatherMap";
    // Weather depends on the city only, so a new keyword reuses the cached forecast
    public string CacheKeyFor(ProviderRequest request) => request.City.Trim().ToLowerInvariant();
    public async Task<IReadOnlyList<AggregatedItem>> FetchAsync(
    ProviderRequest request, CancellationToken ct)
    {
        var apiKey = settings.Get(Name).ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenWeatherMap API key is not configured");
        var url = $"data/2.5/forecast?q={Uri.EscapeDataString(request.City)}&units=metric&appid={apiKey}";
        var forecast = await http.GetFromJsonAsync<OwmForecastResponse>(url, ct);
        if (forecast is null) return [];
        return forecast.List
        .Select(f => (Entry: f, Time: DateTimeOffset.FromUnixTimeSeconds(f.Dt)))
        .Where(x => x.Time.Hour == 12) // one entry per day, midday UTC
        .Select(x => new AggregatedItem(
        Source: Name,
        Title: $"{forecast.City.Name}: {x.Entry.Weather.FirstOrDefault()?.Description}, "
        + $"{x.Entry.Main.Temp:F0}°C",
        Url: $"https://openweathermap.org/city/{forecast.City.Id}",
        Description: $"Feels like {x.Entry.Main.FeelsLike:F0}°C, humidity {x.Entry.Main.Humidity}%",
        PublishedAt: x.Time,
        Category: "weather",
        Score: 0))
        .ToList();
    }
}
public record OwmForecastResponse(List<OwmEntry> List, OwmCity City);
public record OwmEntry(long Dt, OwmMain Main, List<OwmWeather> Weather);
public record OwmMain(double Temp,
    [property: JsonPropertyName("feels_like")] double FeelsLike, int Humidity);
public record OwmWeather(string Description);
public record OwmCity(int Id, string Name);