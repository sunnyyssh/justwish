using MediatR;

namespace Justwish.Users.Domain;

public interface ICommand : IRequest;

public interface ICommand<out TResponse> : IRequest<TResponse>;