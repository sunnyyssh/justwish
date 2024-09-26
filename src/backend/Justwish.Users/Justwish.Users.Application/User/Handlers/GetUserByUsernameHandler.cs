using Ardalis.Result;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;

namespace Justwish.Users.Application;

public class GetUserByUsernameHandler : IQueryHandler<GetUserByUsernameQuery, Result<UserDto>>
{
    private readonly IUserReadRepository _repository;

    public GetUserByUsernameHandler(IUserReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserDto>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserByUsernameAsync(request.Username);
        return user.Map(UserDto.FromDomain);
    }
}
