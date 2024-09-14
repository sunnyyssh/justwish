using System.Windows.Input;
using MediatR;

namespace Justwish.Users.Domain;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
    where TCommand : ICommand<TResponse>;