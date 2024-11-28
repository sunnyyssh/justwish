using FastEndpoints;

namespace Justwish.Users.WebApi;

public class HelloWorldEndpoint : EndpointWithoutRequest<string>
{
    private readonly ILogger _logger;

    public HelloWorldEndpoint(ILogger<HelloWorldEndpoint> logger)
    {
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/hello-world");
        AllowAnonymous();
    }

    public override Task<string> ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Hello-world requested");
        return Task.FromResult("Hello World!");
    }
}