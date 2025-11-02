namespace learnify.ai.api.Common.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
