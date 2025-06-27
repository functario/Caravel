using Caravel.Abstractions;

namespace Caravel.History.Mermaid;
public record JourneyHistory : IJourneyHistory
{
    public Queue<IEdge> Edges { get; init; } = new Queue<IEdge>();
}
