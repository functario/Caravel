namespace Caravel.Abstractions.Events;
public interface IJourneyLeg
{
    public Guid JourneyId { get; }
    public Queue<IEdge> Edges { get; }
}
