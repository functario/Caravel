namespace Caravel.Abstractions;

public interface IJourney
{
    CancellationToken JourneyCancellationToken { get; }
    IGraph Graph { get; }
    IJourneyLog Log { get; }
    INode Current { get; }
    Task<IJourney> GotoAsync<TDestination>(CancellationToken localCancellationToken)
        where TDestination : INode;
}
