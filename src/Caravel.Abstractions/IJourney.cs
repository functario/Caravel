namespace Caravel.Abstractions;

public interface IJourney
{
    IGraph Graph { get; }
    IJourneyLog Log { get; }
    INode Current { get; }
    Task<IJourney> GotoAsync<TDestination>(CancellationToken methodCancellationToken)
        where TDestination : INode;
}
