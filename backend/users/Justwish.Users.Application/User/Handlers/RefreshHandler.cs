using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public sealed class RefreshHandler : ICommandHandler<RefreshCommand, Result<JwtTokenPair>>
{
    private readonly IJwtService _jwtService;

    public RefreshHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
    
    public async Task<Result<JwtTokenPair>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        return await _jwtService.RefreshAsync(request.RefreshToken);
    }
}