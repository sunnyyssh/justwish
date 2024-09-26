using Ardalis.Result;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;

namespace Justwish.Users.Application;

public class GetUserByEmailHandler : IQueryHandler<GetUserByEmailQuery, Result<UserDto>>
{
    private readonly IUserRepository _repository;

    public GetUserByEmailHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserByEmailAsync(request.Email, cancellationToken);
        return user.Map(UserDto.FromDomain);
    }
}