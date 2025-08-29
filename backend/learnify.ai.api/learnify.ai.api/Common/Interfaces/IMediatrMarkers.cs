using MediatR;

namespace learnify.ai.api.Common.Interfaces;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest
{
}