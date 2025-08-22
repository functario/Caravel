using System.Collections.Concurrent;
using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core;

public class InMemoryJourneyLegStore : IJourneyLegPublisher, IJourneyLegReader
{
    public ConcurrentQueue<IJourneyLegEvent> LegEvents { get; init; } = [];

    public Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
        CancellationToken cancellationToken = default
    )
    {
        var legArray = LegEvents
            .Where(x => x is IJourneyLegCompletedEvent)
            .OrderBy(x => x.Timestamp)
            .Select(x => x.JourneyLeg)
            .ToArray();

        return Task.FromResult<IEnumerable<IJourneyLeg>>(legArray);
    }

    Task IJourneyLegPublisher.PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegCompletedAsync(journeyLegCompletedEvent, cancellationToken);

    Task IJourneyLegPublisher.PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegStartedAsync(journeyLegStartedEvent, cancellationToken);

    Task IJourneyLegPublisher.PublishOnJourneyLegUpdatedAsync(
        IJourneyLegUpdatedEvent journeyLegUpdatedEvent,
        CancellationToken cancellationToken
    ) => PublishOnJourneyLegUpdatedAsync(journeyLegUpdatedEvent, cancellationToken);

    protected Task PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        LegEvents.Enqueue(journeyLegCompletedEvent);
        return Task.CompletedTask;
    }

    protected Task PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    ) => PublishJourneyLegEventAsync(journeyLegStartedEvent, cancellationToken);

    protected Task PublishOnJourneyLegUpdatedAsync(
        IJourneyLegUpdatedEvent journeyLegUpdatedEvent,
        CancellationToken cancellationToken
    ) => PublishJourneyLegEventAsync(journeyLegUpdatedEvent, cancellationToken);

    private Task PublishJourneyLegEventAsync(
        IJourneyLegEvent journeyLegEvent,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        LegEvents.Enqueue(journeyLegEvent);
        return Task.CompletedTask;
    }
}
