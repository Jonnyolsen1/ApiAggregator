using ApiAggregator.Api.Providers;
using Microsoft.AspNetCore.Mvc;

namespace ApiAggregator.Api.Controllers;

// TEMPORARY: for checking providers one at a time. Delete in Block F.
[ApiController]
[Route("api/[controller]")]                      // → /api/test
public class TestController(IEnumerable<IApiProvider> providers) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        string provider = "GitHub",
        string keyword = "dotnet",
        string city = "Athens",
        CancellationToken ct = default)
    {
        var selected = providers.FirstOrDefault(p =>
            p.Name.Equals(provider, StringComparison.OrdinalIgnoreCase));

        if (selected is null)
            return NotFound($"No provider named '{provider}'");

        var items = await selected.FetchAsync(new ProviderRequest(keyword, city), ct);
        return Ok(items);
    }
}