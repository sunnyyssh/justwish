using Justwish.Users.WebApi.ApiKeyAuth;
using MassTransit.Testing;

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
        Client.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName, TestConstants.ApiKey);
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
        Client.Dispose();
    }
}