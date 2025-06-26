using Caravel.Abstractions;

namespace Caravel.Core;
public record JourneyHistory : IJourneyHistory
{
    public Queue<IEdge> Edges { get; init; } = new Queue<IEdge>();
}
