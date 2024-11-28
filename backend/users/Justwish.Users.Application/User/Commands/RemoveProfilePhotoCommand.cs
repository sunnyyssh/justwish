using Ardalis.Result;
using Justwish.Users.Domain;

namespace Justwish.Users.Application;

public record class RemoveProfilePhotoCommand(Guid UserId) : ICommand<Result>;
