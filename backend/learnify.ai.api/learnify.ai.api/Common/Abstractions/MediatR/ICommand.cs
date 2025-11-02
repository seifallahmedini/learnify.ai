using MediatR;

namespace learnify.ai.api.Common.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest
{
}
