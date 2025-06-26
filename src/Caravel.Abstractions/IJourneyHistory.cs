namespace Caravel.Abstractions;

public interface IJourneyHistory
{
    Queue<IEdge> Edges { get; }
}
