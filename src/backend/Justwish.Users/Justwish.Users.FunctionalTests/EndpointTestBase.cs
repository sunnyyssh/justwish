using System.Net.Http.Headers;
using Justwish.Users.Domain;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Justwish.Users.FunctionalTests;

public abstract class EndpointTestBase : IAsyncDisposable
{
    protected HttpClient Client { get; }

    protected TestWebApplicationFactory Factory { get; }

    protected ITestHarness MassTransitTestHarness => Factory.Services.GetTestHarness();

    protected EndpointTestBase()
    {
        Factory = new TestWebApplicationFactory();

        Client = Factory.CreateClient();

    }

    protected async Task SetAuthorizationDefaultHeaderAsync(User user) 
    {
        var tokenPair = await IssueTestTokens(user);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenPair.AccessToken.Token);
    }

    protected Task ClearAuthorizationDefaultHeaderAsync()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        return Task.CompletedTask;
    }

    private async Task<JwtTokenPair> IssueTestTokens(User user) 
    {
        var jwtService = Factory.Services.GetRequiredService<IJwtService>();
        var tokenPair = await jwtService.IssueAsync(user);
        return tokenPair;
    }

    public ValueTask DisposeAsync()
    {
        return Factory.DisposeAsync();
    }
}