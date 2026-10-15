# ApiAggregator

ASP.NET Core (.NET 10) Web API that aggregates data from external APIs
(GitHub, News API, OpenWeatherMap) behind one endpoint.

## Status

**Done (this branch):** solution structure, provider abstraction (`IApiProvider`),
one-line provider registration with typed `HttpClient`s, GitHub provider,
response models, a test endpoint.

**Added after submission** on the `post-work` branch: News API and OpenWeatherMap providers.

**Not done yet:** parallel aggregation endpoint, filtering/sorting, resilience,
caching, request statistics, unit tests.

## Run

```bash
dotnet run --project src/ApiAggregator.Api
```

Then call:

```
GET https://localhost:7198/api/test?provider=GitHub&keyword=dotnet
```

API docs (Development only): https://localhost:7198/scalar

## API keys

GitHub needs no key. Keys for the other providers are stored with user-secrets. From `src/ApiAggregator.Api`:

```bash
dotnet user-secrets set "Providers:NewsApi:ApiKey" "<your key>"
dotnet user-secrets set "Providers:OpenWeatherMap:ApiKey" "<your key>"