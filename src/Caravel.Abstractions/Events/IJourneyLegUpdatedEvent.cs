namespace Caravel.Abstractions.Events;

public interface IJourneyLegUpdatedEvent : IJourneyLegEvent
{
    public IEdge Update { get; }
}
