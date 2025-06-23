using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class JourneyExtensions
{
    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        CancellationToken methodCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        var journey = await journeyTask.ConfigureAwait(false);
        return await journey.GotoAsync<TDestination>(methodCancellationToken).ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this Task<IJourney> journeyTask,
        Func<TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken methodCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        ArgumentNullException.ThrowIfNull(func, nameof(func));
        var journey = await journeyTask.ConfigureAwait(false);
        if (journey.Current is TCurrentNode current)
        {
            var funcNode = await func(current, methodCancellationToken).ConfigureAwait(false);
            return funcNode is null
                ? throw new InvalidOperationException(
                    "The current node has been changed after function called."
                )
                : journey;
        }

        throw new InvalidOperationException("The current node is not the expected one.");
    }
}
