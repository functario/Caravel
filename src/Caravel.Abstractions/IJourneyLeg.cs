namespace Caravel.Abstractions;
public interface IJourneyLeg
{
    Queue<IEdge> Edges { get; }
}
