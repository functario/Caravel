namespace Caravel.Abstractions.Events;

public interface IJourneyLegCompletedEvent : IJourneyLegEvent
{
    public IEdge FinishingEdge { get; }
}
