using Caravel.Abstractions;

namespace Caravel.Core.Extensions;

public static class JourneyExtensions
{
    public static Lazy<IJourney> LazyJourney => new();

    public static async Task<IJourney> GotoAsync<TDestination>(
        this Task<IJourney> journeyTask,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));

        var journey = await journeyTask.ConfigureAwait(false);

        using var linkedCancellationTokenSource = journey.LinkJourneyAndLocalCancellationTokens(
            localCancellationToken
        );

        linkedCancellationTokenSource.Token.ThrowExceptionIfCancellationRequested();

        return await journey.GotoAsync<TDestination>(localCancellationToken).ConfigureAwait(false);
    }

    public static async Task<IJourney> DoAsync<TCurrentNode>(
        this Task<IJourney> journeyTask,
        Func<TCurrentNode, CancellationToken, Task<TCurrentNode>> func,
        CancellationToken localCancellationToken = default
    )
        where TCurrentNode : INode
    {
        ArgumentNullException.ThrowIfNull(journeyTask, nameof(journeyTask));
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        var journey = await journeyTask.ConfigureAwait(false);

        using var linkedCancellationTokenSource = journey.LinkJourneyAndLocalCancellationTokens(
            localCancellationToken
        );

        linkedCancellationTokenSource.Token.ThrowExceptionIfCancellationRequested();

        if (journey.Current is TCurrentNode current)
        {
            var funcNode = await func(current, localCancellationToken).ConfigureAwait(false);
            return funcNode is null
                ? throw new InvalidOperationException(
                    "The current node has been changed after function called."
                )
                : journey;
        }

        throw new InvalidOperationException("The current node is not the expected one.");
    }

    public static CancellationTokenSource LinkJourneyAndLocalCancellationTokens(
        this IJourney journey,
        CancellationToken localCancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(journey, nameof(journey));
        var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            journey.JourneyCancellationToken,
            localCancellationToken
        );

        return linkedCancellationTokenSource;
    }
}
