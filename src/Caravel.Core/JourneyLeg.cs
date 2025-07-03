using Caravel.Abstractions;

namespace Caravel.Core;

public record JourneyLeg(Guid JourneyId, Queue<IEdge> Edges, IRoute PlannedRoute) : IJourneyLeg { }
