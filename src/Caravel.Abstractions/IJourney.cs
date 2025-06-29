using Caravel.Abstractions.Events;

namespace Caravel.Abstractions;

public interface IJourney
{
    CancellationToken JourneyCancellationToken { get; }
    IGraph Graph { get; }
    INode CurrentNode { get; }

    Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(CancellationToken cancellationToken);

    Task<IJourney> GotoAsync<TDestination>(
        IWaypoints waypoints,
        IExcludedNodes excludeNodes,
        CancellationToken localCancellationToken
    )
        where TDestination : INode;
}
