﻿namespace Caravel.Abstractions;

public interface IJourney
{
    CancellationToken JourneyCancellationToken { get; }
    IGraph Graph { get; }
    INode CurrentNode { get; }

    public Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
        CancellationToken cancellationToken
    );

    public TJourney OfType<TJourney>()
        where TJourney : IJourney;

    public Task<IJourney> GotoAsync<TDestination>(
        IWaypoints waypoints,
        IExcludedNodes excludedNodes,
        CancellationToken localCancellationToken = default
    )
        where TDestination : INode;

    public Task<IJourney> DoAsync<TCurrentNode, TNodeOut>(
        Func<IJourney, TCurrentNode, CancellationToken, Task<TNodeOut>> func,
        CancellationToken localCancellationToken = default
    )
        where TCurrentNode : INode
        where TNodeOut : INode;
}
