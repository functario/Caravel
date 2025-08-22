using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core.Events;

public sealed class JourneyLegPublisherFactory : IJourneyLegPublisherFactory
{
    private readonly TimeProvider _timeProvider;

    public JourneyLegPublisherFactory(TimeProvider timeProvider)
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
