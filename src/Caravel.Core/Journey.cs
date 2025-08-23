using Caravel.Abstractions;
using Caravel.Abstractions.Configurations;
using Caravel.Abstractions.Events;
using Caravel.Abstractions.Exceptions;
using Caravel.Core.Extensions;

namespace Caravel.Core;

public class Journey : IJourney
{
    private readonly IActionMetaDataFactory _actionMetaDataFactory;
    private readonly IJourneyLegFactory _journeyLegFactory;
    private readonly IJourneyLegEventFactory _journeyLegEventFactory;
    private readonly IJourneyLegPublisher _journeyLegPublisher;
    private bool _isJourneyStarted;

    public Journey(
        INode startingNode,
        IGraph graph,
        IJourneyConfiguration journeyConfiguration,
        CancellationToken journeyCancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(startingNode, nameof(startingNode));
        ArgumentNullException.ThrowIfNull(journeyConfiguration, nameof(journeyConfiguration));
        ArgumentNullException.ThrowIfNull(
            journeyConfiguration.JourneyLegReader,
            nameof(journeyConfiguration.JourneyLegReader)
        );

        ArgumentNullException.ThrowIfNull(
            journeyConfiguration.JourneyLegPublisher,
            nameof(journeyConfiguration.JourneyLegPublisher)
        );

        journeyCancellationToken.ThrowIfCancellationRequested();

        _isJourneyStarted = false;
        Graph = graph;
        JourneyLegReader = journeyConfiguration.JourneyLegReader;
        _journeyLegEventFactory = journeyConfiguration.JourneyLegEventFactory;
        _actionMetaDataFactory = journeyConfiguration.ActionMetaDataFactory;
        _journeyLegFactory = journeyConfiguration.JourneyLegFactory;
        JourneyCancellationToken = journeyCancellationToken;
        _journeyLegPublisher = journeyConfiguration.JourneyLegPublisher;
        CurrentNode = startingNode;
    }

    public INode CurrentNode { get; private set; }
    public IGraph Graph { get; init; }
    public CancellationToken JourneyCancellationToken { get; }
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public IJourneyLegReader JourneyLegReader { get; init; }

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
        IExcludedWaypoints excludedWaypoints,
        CancellationToken scopedCancellationToken
    )
        where TDestination : INode
    {
        ArgumentNullException.ThrowIfNull(waypoints, nameof(waypoints));

        await GotoAsync(typeof(TDestination), waypoints, excludedWaypoints, scopedCancellationToken)
            .ConfigureAwait(false);

        return this;
    }

    public async Task<IJourney> GotoAsync(
        Type destinationType,
        IWaypoints waypoints,
        IExcludedWaypoints excludedWaypoints,
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
        var route = GetRoute(originType, destinationType, waypoints, excludedWaypoints);
        var legEdges = new Queue<IEdge>();
        var journeyLeg = _journeyLegFactory.CreateJourneyLeg(Id, legEdges, route);

        await _journeyLegPublisher
            .PublishOnJourneyLegStartedAsync(
                _journeyLegEventFactory.CreateJourneyLegStartedEvent(journeyLeg),
                linkedCancellationTokenSource.Token
            )
            .ConfigureAwait(false);

        await NavigateAsync(route.Edges, journeyLeg, linkedCancellationTokenSource.Token)
            .ConfigureAwait(false);

        await CompleteJourneyLegAsync(
                destinationType,
                journeyLeg,
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
        IExcludedWaypoints excludedWaypoints
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

        return Graph.GetRoute(originType, destinationType, waypoints, excludedWaypoints);
    }

    private static bool HasExplicitEdgeToItself(INode node) =>
        node.GetEdges().Where(x => x.Origin == x.Neighbor).FirstOrDefault() is not null;

    private static bool OriginIsAlsoAWaypoint(Type originType, IWaypoints waypoints) =>
        waypoints.Any(x => x == originType);

    private static bool DestinationIsAlsoAWaypoint(Type destinationType, IWaypoints waypoints) =>
        waypoints.Any(x => x == destinationType);

    private async Task NavigateAsync(
        ICollection<IEdge> edges,
        IJourneyLeg journeyLeg,
        CancellationToken cancellationToken
    )
    {
        foreach (var edge in edges)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CurrentNode = await edge
                .NeighborNavigator.MoveNext(this, cancellationToken)
                .ConfigureAwait(false);

            await CurrentNode.OnNodeOpenedAsync(this, cancellationToken).ConfigureAwait(false);

            journeyLeg.Edges.Enqueue(edge);
            await _journeyLegPublisher
                .PublishOnJourneyLegUpdatedAsync(
                    _journeyLegEventFactory.CreateJourneyLegUpdatedEvent(edge, journeyLeg),
                    cancellationToken
                )
                .ConfigureAwait(false);
        }
    }

    private async Task<IJourneyLeg> AddDoAsyncToJourneyLegAsync(
        INode currentNode,
        INode nodeOut,
        Guid journeyId,
        IActionMetaData? actionMetaData,
        CancellationToken linkedCancellationToken
    )
    {
        // Publish start and end of navigation.
        actionMetaData ??= _actionMetaDataFactory.CreateActionMetaData(
            _actionMetaDataFactory.DefaultDoAsyncDescription
        );

        var journeyLeg = _journeyLegFactory.CreateJourneyLeg(
            currentNode,
            nodeOut,
            journeyId,
            Graph.RouteFactory,
            Graph.EdgeFactory,
            actionMetaData
        );

        await _journeyLegPublisher
            .PublishOnJourneyLegStartedAsync(
                _journeyLegEventFactory.CreateJourneyLegStartedEvent(journeyLeg),
                linkedCancellationToken
            )
            .ConfigureAwait(false);

        await CompleteJourneyLegAsync(nodeOut.GetType(), journeyLeg, linkedCancellationToken)
            .ConfigureAwait(false);

        return journeyLeg;
    }

    private async Task CompleteJourneyLegAsync(
        Type destinationType,
        IJourneyLeg completedJourneyLeg,
        CancellationToken cancellationToken
    )
    {
        await _journeyLegPublisher
            .PublishOnJourneyLegCompletedAsync(
                _journeyLegEventFactory.CreateJourneyLegCompletedEvent(completedJourneyLeg),
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
