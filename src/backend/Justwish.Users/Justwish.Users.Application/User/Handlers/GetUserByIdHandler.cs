using Ardalis.Result;
using Justwish.Users.Domain;
using Justwish.Users.Domain.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Justwish.Users.Application;

public sealed class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _repository;

    public GetUserByIdHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserByIdAsync(request.Id, cancellationToken);
        return user.Map(UserDto.FromDomain);
    }
}