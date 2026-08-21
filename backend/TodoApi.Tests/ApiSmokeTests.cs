using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TodoApi.Tests;

public class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateClient_StartsApplication()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        // No root endpoint yet — app should still start and respond.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
