using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Core;

public record Journey : IJourney
{
    public Journey(INode current, IGraph graph, CancellationToken journeyCancellationToken)
    {
        ArgumentNullException.ThrowIfNull(current, nameof(current));
        journeyCancellationToken.ThrowExceptionIfCancellationRequested();

        Graph = graph;
        JourneyCancellationToken = journeyCancellationToken;
        Current = current;
        Log = new JourneyLog();
    }

    public INode Current { get; private set; }
    public IGraph Graph { get; init; }
    public CancellationToken JourneyCancellationToken { get; }
    public IJourneyLog Log { get; init; }

    public async Task<IJourney> GotoAsync<TDestination>(
        IWaypoints waypoints,
        IExcludedNodes excludeNodes,
        CancellationToken localCancellationToken
    )
        where TDestination : INode
    {
        using var linkedCancellationTokenSource = this.LinkJourneyAndLocalCancellationTokens(
            localCancellationToken
        );

        linkedCancellationTokenSource.Token.ThrowExceptionIfCancellationRequested();

        await this
            .Current.ThrowExceptionIfNodeAuditFails(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        var originType = Current.GetType();
        var destinationType = typeof(TDestination);
        var shortestRoute = Graph.GetShortestRoute(
            originType,
            destinationType,
            waypoints,
            excludeNodes
        );
        var edges = shortestRoute.Edges;

        if (edges.Any(x => x is null))
        {
            throw new InvalidOperationException("Edge should not be null.");
        }

        foreach (var edge in edges)
        {
            linkedCancellationTokenSource.Token.ThrowExceptionIfCancellationRequested();
            Current = await edge.MoveNext(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);
            Log.History.Enqueue(Current.GetType());
        }

        if (Current is not TDestination)
        {
            throw new InvalidOperationException("The last INode is not the destination.");
        }

        await this
            .Current.ThrowExceptionIfNodeAuditFails(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        return this;
    }
}
