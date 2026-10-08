using Microsoft.Extensions.Http.Resilience;
namespace ApiAggregator.Api.Providers;

public static class ProviderRegistration
{
    // One line per API in Program.cs: this is the "easy to add a new API" story.
    public static IServiceCollection AddApiProvider<TProvider>(
    this IServiceCollection services, string name, IConfiguration config)
    where TProvider : class, IApiProvider
    {
        var section = config.GetSection($"Providers:{name}");
        services.Configure<ProviderSettings>(name, section); // "named options": one per provider
        var baseUrl = section["BaseUrl"]
        ?? throw new InvalidOperationException($"Missing Providers:{name}:BaseUrl");
        var client = services.AddHttpClient<TProvider>(c =>
        {
            c.BaseAddress = new Uri(baseUrl);
            c.DefaultRequestHeaders.UserAgent.ParseAdd("ApiAggregator/1.0"); // GitHub + News API need it
        });
        // Day 2: resilience + statistics handlers get attached to `client` here
        services.AddTransient<IApiProvider>(sp => sp.GetRequiredService<TProvider>());
        return services;
    }
}