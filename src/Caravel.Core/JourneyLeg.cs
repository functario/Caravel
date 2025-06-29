using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core;
public record JourneyLeg(Guid JourneyId, Queue<IEdge> Edges) : IJourneyLeg
{
}
