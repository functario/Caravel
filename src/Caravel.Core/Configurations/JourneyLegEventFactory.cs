using Caravel.Abstractions;
using Caravel.Abstractions.Configurations;
using Caravel.Abstractions.Events;
using Caravel.Core.Events;

namespace Caravel.Core.Configurations;

public sealed class JourneyLegEventFactory : IJourneyLegEventFactory
{
    private readonly TimeProvider _timeProvider;

    public JourneyLegEventFactory(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public IJourneyLegCompletedEvent CreateJourneyLegCompletedEvent(
        IJourneyLeg completedJourneyLeg,
        IEdge finishingEdge
    ) =>
        new JourneyLegCompletedEvent(_timeProvider.GetUtcNow(), completedJourneyLeg, finishingEdge);

    public IJourneyLegStartedEvent CreateJourneyLegStartedEvent(IJourneyLeg journeyLeg) =>
        new JourneyLegStartedEvent(_timeProvider.GetUtcNow(), journeyLeg);

    public IJourneyLegUpdatedEvent CreateJourneyLegUpdatedEvent(
        IEdge update,
        IJourneyLeg journeyLeg
    ) => new JourneyLegUpdatedEvent(_timeProvider.GetUtcNow(), journeyLeg, update);
}
