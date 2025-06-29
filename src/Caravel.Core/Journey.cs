using Caravel.Abstractions;
using Caravel.Core.Extensions;

namespace Caravel.Core;

public record Journey : IJourney, IJourneLegPublisher
{
    public Journey(INode current, IGraph graph, CancellationToken journeyCancellationToken)
    {
        ArgumentNullException.ThrowIfNull(current, nameof(current));
        journeyCancellationToken.ThrowExceptionIfCancellationRequested();

        Graph = graph;
        JourneyCancellationToken = journeyCancellationToken;
        CurrentNode = current;
    }

    public INode CurrentNode { get; private set; }
    public IGraph Graph { get; init; }
    public CancellationToken JourneyCancellationToken { get; }

    // Explicit to enfore usage of the virtual method.
    Task IJourneLegPublisher.PublishOnJourneyLegCompletedAsync(
        IJourneyLeg journeyLeg,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegCompletedAsync(journeyLeg, cancellationToken);

    Task<IEnumerable<IJourneyLeg>> IJourney.ReadJourneyLegsAsync(
        CancellationToken cancellationToken
    ) => ReadJourneyLegsAsync(cancellationToken);

    protected virtual Task PublishOnJourneyLegCompletedAsync(
        IJourneyLeg journeyLeg,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;

    protected virtual Task<IEnumerable<IJourneyLeg>> ReadJourneyLegsAsync(
        CancellationToken cancellationToken
    ) => Task.FromResult<IEnumerable<IJourneyLeg>>([]);

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
            .CurrentNode.OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        var originType = CurrentNode.GetType();
        var destinationType = typeof(TDestination);
        var route = Graph.GetRoute(originType, destinationType, waypoints, excludeNodes);

        var edges = route.Edges;

        if (edges.Any(x => x is null))
        {
            throw new InvalidOperationException("Edge should not be null.");
        }

        var legEdges = new Queue<IEdge>();
        foreach (var edge in edges)
        {
            linkedCancellationTokenSource.Token.ThrowExceptionIfCancellationRequested();
            CurrentNode = await edge.MoveNext(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            await this
                .CurrentNode.OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            legEdges.Enqueue(edge);
        }

        var journeyLeg = new JourneyLeg(legEdges);
        await PublishOnJourneyLegCompletedAsync(journeyLeg, localCancellationToken)
            .ConfigureAwait(false);

        if (CurrentNode is not TDestination)
        {
            throw new InvalidOperationException("The last INode is not the destination.");
        }

        await this
            .CurrentNode.OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        return this;
    }
}
