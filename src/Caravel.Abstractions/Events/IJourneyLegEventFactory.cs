namespace Caravel.Abstractions.Events;

public interface IJourneyLegEventFactory
{
    public IJourneyLegStartedEvent CreateJourneyLegStartedEvent(IJourneyLeg journeyLeg);

    public IJourneyLegUpdatedEvent CreateJourneyLegUpdatedEvent(
        IEdge update,
        IJourneyLeg journeyLeg
    );

    public IJourneyLegCompletedEvent CreateJourneyLegCompletedEvent(
        IJourneyLeg completedJourneyLeg,
        IEdge finishingEdge
    );
}
