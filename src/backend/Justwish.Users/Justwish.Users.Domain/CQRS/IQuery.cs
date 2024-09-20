using MediatR;

namespace Justwish.Users.Domain;

public interface IQuery<out TResponse> : IRequest<TResponse>;