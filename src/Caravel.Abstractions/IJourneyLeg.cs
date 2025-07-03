namespace Caravel.Abstractions;

public interface IJourneyLeg
{
    public Guid JourneyId { get; }
    public Queue<IEdge> Edges { get; }
    public IRoute PlannedRoute { get; }
}
