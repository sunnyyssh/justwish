using FastEndpoints;
using Justwish.Users.WebApi.ApiKeyAuth;

namespace Justwish.Users.WebApi;

public class HelloWorldEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/hello-world");
        Policies(ApiKeyConstants.PolicyName);
    }

    public override Task<string> ExecuteAsync(CancellationToken ct)
    {
        return Task.FromResult("Hello World!");
    }
}