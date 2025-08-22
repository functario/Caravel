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
        IJourneyLegFactory journeyLegFactory,
        IActionMetaDataFactory actionMetaDataFactory,
        CancellationToken journeyCancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(startingNode, nameof(startingNode));
        journeyCancellationToken.ThrowIfCancellationRequested();

        _isJourneyStarted = false;
        Graph = graph;
        _timeProvider = timeProvider;
        JourneyLegFactory = journeyLegFactory;
        ActionMetaDataFactory = actionMetaDataFactory;
        JourneyCancellationToken = journeyCancellationToken;
        CurrentNode = startingNode;
    }

    public INode CurrentNode { get; private set; }
    public IGraph Graph { get; init; }
    public CancellationToken JourneyCancellationToken { get; }

    public IJourneyLegFactory JourneyLegFactory { get; init; }
    public IActionMetaDataFactory ActionMetaDataFactory { get; init; }
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
        CancellationToken scopedCancellationToken
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(waypoints, nameof(waypoints));

        await GotoAsync(typeof(TDestination), waypoints, excludedNodes, scopedCancellationToken)
            .ConfigureAwait(false);

        return this;
    }

    public async Task<IJourney> GotoAsync(
        Type destinationType,
        IWaypoints waypoints,
        IExcludedNodes excludedNodes,
        CancellationToken scopedCancellationToken
    )
    {
        // Prevent setting starting node once the journey is started
        _isJourneyStarted = true;
        ArgumentNullException.ThrowIfNull(waypoints, nameof(waypoints));

        using var linkedCancellationTokenSource = this.LinkJourneyAndLocalCancellationTokens(
            scopedCancellationToken
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
        CancellationToken scopedCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode
    {
        // Prevent setting starting node once the journey is started
        _isJourneyStarted = true;
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        using var linkedCancellationTokenSource = this.LinkJourneyAndLocalCancellationTokens(
            scopedCancellationToken
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

            (var outNode, var actionMetaData) = GetNodeIfWrapped(funcNode);

            // Change the current node.
            CurrentNode = outNode;

            // Ensure the navigation from DoAsync is registered as JourneyLeg.
            var doJourneyLeg = await AddDoAsyncToJourneyLegAsync(
                    current,
                    outNode,
                    Id,
                    actionMetaData,
                    linkedCancellationTokenSource.Token
                )
                .ConfigureAwait(false);

            linkedCancellationTokenSource.Token.ThrowIfCancellationRequested();
            await outNode
                .OnNodeOpenedAsync(this, linkedCancellationTokenSource.Token)
                .ConfigureAwait(false);

            return this;
        }

        throw new UnexpectedNodeException(CurrentNode.GetType(), typeof(TCurrentNode));
    }

    private static (INode outNode, IActionMetaData? actionMetaData) GetNodeIfWrapped<TNodeOut>(
        TNodeOut funcNode
    )
        where TNodeOut : INode
    {
        ArgumentNullException.ThrowIfNull(funcNode, nameof(funcNode));

        if (!funcNode.IsAssignableToIEnrichedNode())
        {
            return (funcNode, null);
        }

        // Need to unwrapp the values from EnrichedNode.
        funcNode.TryGetPropertyValue<INode>(x => x.NodeToEnrich, out var nodeToEnrich);

        var unwrappedNode =
            nodeToEnrich
            ?? throw new InvalidOperationException(
                $"{nameof(IEnrichedNode<INode>)} property returned null"
            );

        funcNode.TryGetPropertyValue<IActionMetaData>(
            x => x.ActionMetaData,
            out var actionMetaData
        );

        return (unwrappedNode, actionMetaData);
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

    private async Task<IJourneyLeg> AddDoAsyncToJourneyLegAsync(
        INode currentNode,
        INode nodeOut,
        Guid journeyId,
        IActionMetaData? actionMetaData,
        CancellationToken linkedCancellationToken
    )
    {
        // Publish start and end of navigation.
        actionMetaData ??= ActionMetaDataFactory.CreateActionMetaData(
            ActionMetaDataFactory.DefaultDoAsyncDescription
        );

        var journeyLeg = JourneyLegFactory.CreateJourneyLeg(
            currentNode,
            nodeOut,
            journeyId,
            Graph.RouteFactory,
            Graph.EdgeFactory,
            actionMetaData
        );

        await PublishOnJourneyLegStartedAsync(
                new JourneyLegStartedEvent(_timeProvider.GetUtcNow(), journeyLeg),
                linkedCancellationToken
            )
            .ConfigureAwait(false);

        await CompleteJourneyLegAsync(
                nodeOut.GetType(),
                journeyLeg,
                journeyLeg.Edges.Last(),
                linkedCancellationToken
            )
            .ConfigureAwait(false);

        return journeyLeg;
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
}
