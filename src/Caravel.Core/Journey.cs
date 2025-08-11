using Caravel.Abstractions;
using Caravel.Abstractions.Events;
using Caravel.Abstractions.Exceptions;
using Caravel.Core.Events;
using Caravel.Core.Extensions;

namespace Caravel.Core;

public abstract class Journey : IJourney, IJourneyLegPublisher
{
    private readonly TimeProvider _timeProvider;
    private bool _isJourneyStarted;

    protected Journey(
        INode startingNode,
        IGraph graph,
        TimeProvider timeProvider,
        CancellationToken journeyCancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(startingNode, nameof(startingNode));
        journeyCancellationToken.ThrowIfCancellationRequested();

        _isJourneyStarted = false;
        Graph = graph;
        _timeProvider = timeProvider;
        JourneyCancellationToken = journeyCancellationToken;
        CurrentNode = startingNode;
    }

    public INode CurrentNode { get; private set; }
    public IGraph Graph { get; init; }
    public CancellationToken JourneyCancellationToken { get; }
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public IJourney SetStartingNode(INode node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));

        if (_isJourneyStarted)
        {
            throw new CannotChangeStartingNodeException(
                CurrentNode.GetType().FullName,
                node.GetType().FullName
            );
        }

        CurrentNode = node;
        return this;
    }

    public TJourney OfType<TJourney>()
        where TJourney : IJourney
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
        IExcludedNodes excludedNodes,
        CancellationToken localCancellationToken
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(waypoints, nameof(waypoints));

        await GotoAsync(typeof(TDestination), waypoints, excludedNodes, localCancellationToken)
            .ConfigureAwait(false);

        return this;
    }

    public async Task<IJourney> GotoAsync(
        Type destinationType,
        IWaypoints waypoints,
        IExcludedNodes excludedNodes,
        CancellationToken localCancellationToken
    )
    {
        // Prevent setting starting node once the journey is started
        _isJourneyStarted = true;
        ArgumentNullException.ThrowIfNull(waypoints, nameof(waypoints));

        using var linkedCancellationTokenSource = this.LinkJourneyAndLocalCancellationTokens(
            localCancellationToken
        );

        linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();

        await CurrentNode
            .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        var originType = CurrentNode.GetType();
        var route = GetRoute(originType, destinationType, waypoints, excludedNodes);
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

        await CompleteJourneyLegAsync(
                destinationType,
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
        // Prevent setting starting node once the journey is started
        _isJourneyStarted = true;
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

    private IRoute GetRoute(
        Type originType,
        Type destinationType,
        IWaypoints waypoints,
        IExcludedNodes excludedNodes
    )
    {
        // Cannot allow explicit self reference.
        if (HasExplicitEdgeToItself(CurrentNode))
        { // TODO: Document that a Node cannot declare itself via a edge.
            // But navigation to itself is possible otherwise via Goto.
            throw new InvalidEdgeException(InvalidEdgeReasons.NodeHasEdgeToItself);
        }

        if (OriginIsAlsoAWaypoint(originType, waypoints))
        { // TODO: Document that origin cannot be in Waypoints too
            throw new InvalidRouteException(
                InvalidRouteReasons.OriginIsAlsoWaypoint,
                originType,
                destinationType
            );
        }

        if (DestinationIsAlsoAWaypoint(destinationType, waypoints))
        { // TODO: Document that destination cannot be in Waypoints too
            // (because will become origin at the end).
            throw new InvalidRouteException(
                InvalidRouteReasons.DestinationIsAlsoWaypoint,
                originType,
                destinationType
            );
        }

        // Manage cases where origin is destination without self reference or waypoint.
        if (originType == destinationType && waypoints.Count == 0)
        {
            return Graph.GetSelfRoute(CurrentNode);
        }

        return Graph.GetRoute(originType, destinationType, waypoints, excludedNodes);
    }

    private static bool HasExplicitEdgeToItself(INode node) =>
        node.GetEdges().Where(x => x.Origin == x.Neighbor).FirstOrDefault() is not null;

    private static bool OriginIsAlsoAWaypoint(Type originType, IWaypoints waypoints) =>
        waypoints.Any(x => x == originType);

    private static bool DestinationIsAlsoAWaypoint(Type destinationType, IWaypoints waypoints) =>
        waypoints.Any(x => x == destinationType);

    Task IJourneyLegPublisher.PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegCompletedAsync(journeyLegCompletedEvent, cancellationToken);

    Task IJourneyLegPublisher.PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegStartedAsync(journeyLegStartedEvent, cancellationToken);

    Task IJourneyLegPublisher.PublishOnJourneyLegUpdatedAsync(
        IJourneyLegUpdatedEvent journeyLegUpdatedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegUpdatedAsync(journeyLegUpdatedEvent, cancellationToken);

    protected abstract Task PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    );

    protected abstract Task PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    );

    protected abstract Task PublishOnJourneyLegUpdatedAsync(
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

        await CompleteJourneyLegAsync(
                typeof(TNodeOut),
                journeyLeg,
                journeyLeg.Edges.Last(),
                linkedCancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task CompleteJourneyLegAsync(
        Type destinationType,
        IJourneyLeg completedJourneyLeg,
        IEdge finishingEdge,
        CancellationToken cancellationToken
    )
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

        if (CurrentNode.GetType() != destinationType)
        {
            throw new UnexpectedNodeException(destinationType, CurrentNode.GetType());
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
