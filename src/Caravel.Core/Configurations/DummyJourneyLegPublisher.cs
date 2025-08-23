using Caravel.Abstractions.Events;

namespace Caravel.Core.Configurations;

public readonly record struct DummyJourneyLegPublisher : IJourneyLegPublisher
{
    public Task PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;

    public Task PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;

    public Task PublishOnJourneyLegUpdatedAsync(
        IJourneyLegUpdatedEvent journeyLegUpdatedEvent,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;
}
