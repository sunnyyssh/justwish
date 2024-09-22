using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed class SignOutHandler : ICommandHandler<SignOutCommand>
{
    private readonly IJwtService _jwtService;

    public SignOutHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
    
    public async Task Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        await _jwtService.InvalidateRefreshTokenAsync(request.Token);
    }
}