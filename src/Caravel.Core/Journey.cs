using Caravel.Abstractions;
using Caravel.Abstractions.Events;
using Caravel.Core.Events;
using Caravel.Core.Extensions;

namespace Caravel.Core;

public record Journey : IJourney, IJourneLegPublisher
{
    private readonly TimeProvider _timeProvider;

    public Journey(
        INode current,
        IGraph graph,
        TimeProvider timeProvider,
        CancellationToken journeyCancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(current, nameof(current));
        journeyCancellationToken.ThrowExceptionIfCancellationRequested();

        Graph = graph;
        _timeProvider = timeProvider;
        JourneyCancellationToken = journeyCancellationToken;
        CurrentNode = current;
    }

    public INode CurrentNode { get; private set; }
    public IGraph Graph { get; init; }
    public CancellationToken JourneyCancellationToken { get; }
    public Guid Id { get; init; } = Guid.CreateVersion7();

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

        await CurrentNode
            .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        var originType = CurrentNode.GetType();
        var destinationType = typeof(TDestination);
        var route = Graph.GetRoute(originType, destinationType, waypoints, excludeNodes);

        if (route.Edges.Any(x => x is null))
        {
            throw new InvalidOperationException("Edge should not be null.");
        }

        var legEdges = new Queue<IEdge>();
        var journeyLeg = new JourneyLeg(Id, legEdges);
        await PublishOnJourneyLegStartedAsync(
                new JourneyLegStartedEvent(_timeProvider.GetUtcNow(), journeyLeg),
                linkedCancellationTokenSource.Token
            )
            .ConfigureAwait(false);

        foreach (var edge in route.Edges)
        {
            linkedCancellationTokenSource.Token.ThrowExceptionIfCancellationRequested();
            CurrentNode = await edge
                .NeighborNavigator.MoveNext(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            await CurrentNode
                .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            journeyLeg.Edges.Enqueue(edge);
            await PublishOnJourneyLegUpdatedAsync(
                    new JourneyLegUpdatedEvent(_timeProvider.GetUtcNow(), journeyLeg),
                    linkedCancellationTokenSource.Token
                )
                .ConfigureAwait(false);
        }

        await PublishOnJourneyLegCompletedAsync(
                new JourneyLegCompletedEvent(_timeProvider.GetUtcNow(), journeyLeg),
                localCancellationToken
            )
            .ConfigureAwait(false);

        if (CurrentNode is not TDestination)
        {
            throw new InvalidOperationException("The last INode is not the destination.");
        }

        await CurrentNode
            .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        return this;
    }

    // Explicit to enforce usage of the virtual method.
    Task IJourneLegPublisher.PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegCompletedAsync(journeyLegCompletedEvent, cancellationToken);

    Task IJourneLegPublisher.PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegStartedAsync(journeyLegStartedEvent, cancellationToken);

    Task IJourneLegPublisher.PublishOnJourneyLegUpdatedAsync(
        IJourneyLegUpdatedEvent journeyLegUpdatedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegUpdatedAsync(journeyLegUpdatedEvent, cancellationToken);

    protected virtual Task PublishOnJourneyLegUpdatedAsync(
        IJourneyLegUpdatedEvent journeyLegUpdatedEvent,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;

    protected virtual Task PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;

    protected virtual Task PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;

    Task<IEnumerable<IJourneyLeg>> IJourney.GetCompletedJourneyLegsAsync(
        CancellationToken cancellationToken
    ) => GetCompletedJourneyLegsAsync(cancellationToken);

    /// <summary>
    /// Returns completed journey legs. Must be overridden by subclasses; base implementation will throw.
    /// </summary>
    protected virtual Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
        CancellationToken cancellationToken
    ) =>
        throw new NotImplementedException(
            $"{GetType().Name} must override {nameof(GetCompletedJourneyLegsAsync)}."
        );
}
