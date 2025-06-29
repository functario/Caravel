namespace Caravel.Abstractions;

public interface IJourney
{
    CancellationToken JourneyCancellationToken { get; }
    IGraph Graph { get; }
    INode Current { get; }

    Task<IEnumerable<IJourneyLeg>> ReadJourneyLegsAsync(CancellationToken cancellationToken);

    Task<IJourney> GotoAsync<TDestination>(
        IWaypoints waypoints,
        IExcludedNodes excludeNodes,
        CancellationToken localCancellationToken
    )
        where TDestination : INode;
}
