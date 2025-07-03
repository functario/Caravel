using Caravel.Abstractions;
using Caravel.Abstractions.Events;
using Caravel.Abstractions.Exceptions;
using Caravel.Core.Events;
using Caravel.Core.Extensions;

namespace Caravel.Core;

public abstract class Journey : IJourney, IJourneyLegPublisher
{
    private readonly TimeProvider _timeProvider;

    protected Journey(
        INode current,
        IGraph graph,
        TimeProvider timeProvider,
        CancellationToken journeyCancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(current, nameof(current));
        journeyCancellationToken.ThrowIfCancellationRequested();

        Graph = graph;
        _timeProvider = timeProvider;
        JourneyCancellationToken = journeyCancellationToken;
        CurrentNode = current;
    }

    public INode CurrentNode { get; private set; }
    public IGraph Graph { get; init; }
    public CancellationToken JourneyCancellationToken { get; }
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public TJourney OfType<TJourney>()
    {
        if (this is TJourney journey)
        {
            return journey;
        }

        throw new InvalidCastException(
            $"Could not cast '{this.GetType()}' to '{typeof(TJourney)}'."
        );
    }

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

        linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();

        await CurrentNode
            .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        var originType = CurrentNode.GetType();
        var destinationType = typeof(TDestination);
        var route = Graph.GetRoute(originType, destinationType, waypoints, excludeNodes);
        var legEdges = new Queue<IEdge>();
        var journeyLeg = new JourneyLeg(Id, legEdges, route);
        await PublishOnJourneyLegStartedAsync(
                new JourneyLegStartedEvent(_timeProvider.GetUtcNow(), journeyLeg),
                linkedCancellationTokenSource.Token
            )
            .ConfigureAwait(false);

        foreach (var edge in route.Edges)
        {
            linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();
            CurrentNode = await edge
                .NeighborNavigator.MoveNext(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            await CurrentNode
                .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            journeyLeg.Edges.Enqueue(edge);
            await PublishOnJourneyLegUpdatedAsync(
                    new JourneyLegUpdatedEvent(_timeProvider.GetUtcNow(), journeyLeg, edge),
                    linkedCancellationTokenSource.Token
                )
                .ConfigureAwait(false);
        }

        await CompleteJourneyLegAsync<TDestination>(
                journeyLeg,
                journeyLeg.Edges.Last(),
                linkedCancellationTokenSource.Token
            )
            .ConfigureAwait(false);

        return this;
    }

    public async Task<IJourney> DoAsync<TCurrentNode, TNodeOut>(
        Func<IJourney, TCurrentNode, CancellationToken, Task<TNodeOut>> func,
        CancellationToken localCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        using var linkedCancellationTokenSource = this.LinkJourneyAndLocalCancellationTokens(
            localCancellationToken
        );

        linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();

        await CurrentNode
            .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        // Validate the CurrentNode at each steps.
        if (CurrentNode is TCurrentNode current)
        {
            var funcNode = await func(this, current, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            // Ensure the navigation from DoAsync is registered.
            await SetNavigationFromDoAsync<TCurrentNode, TNodeOut>(
                    current,
                    funcNode,
                    Id,
                    linkedCancellationTokenSource.Token
                )
                .ConfigureAwait(false);

            linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();
            await funcNode
                .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            return this;
        }

        throw new UnexpectedNodeException(CurrentNode.GetType(), typeof(TCurrentNode));
    }

    public abstract Task PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    );

    public abstract Task PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    );

    public abstract Task PublishOnJourneyLegUpdatedAsync(
        IJourneyLegUpdatedEvent journeyLegUpdatedEvent,
        CancellationToken cancellationToken
    );

    public abstract Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
        CancellationToken cancellationToken
    );

    private async Task SetNavigationFromDoAsync<TCurrentNode, TNodeOut>(
        TCurrentNode currentNode,
        TNodeOut nodeOut,
        Guid journeyId,
        CancellationToken linkedCancellationToken
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        // Change the current node.
        CurrentNode = nodeOut;

        // Publish start and end of navigation.
        var journeyLeg = DynamicJourneyLeg<TCurrentNode, TNodeOut>(currentNode, journeyId);

        await PublishOnJourneyLegStartedAsync(
                new JourneyLegStartedEvent(_timeProvider.GetUtcNow(), journeyLeg),
                linkedCancellationToken
            )
            .ConfigureAwait(false);

        await CompleteJourneyLegAsync<TNodeOut>(
                journeyLeg,
                journeyLeg.Edges.Last(),
                linkedCancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task CompleteJourneyLegAsync<TDestination>(
        IJourneyLeg completedJourneyLeg,
        IEdge finishingEdge,
        CancellationToken cancellationToken
    )
        where TDestination : INode
    {
        await PublishOnJourneyLegCompletedAsync(
                new JourneyLegCompletedEvent(
                    _timeProvider.GetUtcNow(),
                    completedJourneyLeg,
                    finishingEdge
                ),
                cancellationToken
            )
            .ConfigureAwait(false);

        if (CurrentNode is not TDestination)
        {
            throw new UnexpectedNodeException(typeof(TDestination), CurrentNode.GetType());
        }

        await CurrentNode.OnNodeOpenedAsync(this, cancellationToken).ConfigureAwait(false);
    }

    private static JourneyLeg DynamicJourneyLeg<TCurrentNode, TNodeOut>(
        TCurrentNode currentNode,
        Guid journeyId
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        if (journeyId.Version != 7)
            throw new InvalidOperationException("Id must be Guid.V7.");

        var neighborNavigator = new NeighborNavigator(
            MoveNext(currentNode),
            $"{nameof(Journey)}.{nameof(DoAsync)}"
        );

        var legEdges = new Queue<IEdge>(
            [new Edge(typeof(TCurrentNode), typeof(TNodeOut), neighborNavigator)]
        );

        var doRoute = new DoRoute([.. legEdges]);
        return new JourneyLeg(journeyId, legEdges, doRoute);
    }

    private static Func<IJourney, CancellationToken, Task<INode>> MoveNext(INode node)
    {
        Task<INode> Func(IJourney journey, CancellationToken cancellationToken) =>
            Task.FromResult(node);
        return Func;
    }
}
