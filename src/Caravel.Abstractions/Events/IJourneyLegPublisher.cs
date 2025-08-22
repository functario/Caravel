namespace Caravel.Abstractions.Events;

public interface IJourneyLegPublisher
{
    public Task PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    );

    public Task PublishOnJourneyLegUpdatedAsync(
        IJourneyLegUpdatedEvent journeyLegUpdatedEvent,
        CancellationToken cancellationToken
    );

    public Task PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    );
}
