using Microsoft.Extensions.Http.Resilience;
namespace ApiAggregator.Api.Providers;

public static class ProviderRegistration
{
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

        services.AddTransient<IApiProvider>(sp => sp.GetRequiredService<TProvider>());
        return services;
    }
}