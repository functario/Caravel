using Caravel.Abstractions;

namespace Caravel.Core;
public record JourneyLeg(Queue<IEdge> Edges) : IJourneyLeg
{
}
