using ApiAggregator.Api.Models;

namespace ApiAggregator.Api.Providers;

public interface IApiProvider
{
    string Name { get; }
    string CacheKeyFor(ProviderRequest request);
    Task<IReadOnlyList<AggregatedItem>> FetchAsync(ProviderRequest request, CancellationToken ct);
}