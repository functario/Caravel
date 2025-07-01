using System.Collections.Concurrent;
using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core;

public sealed class InMemoryJourney : Journey
{
    public InMemoryJourney(
        INode current,
        IGraph graph,
        TimeProvider timeProvider,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, timeProvider, journeyCancellationToken) { }

    public ConcurrentQueue<IJourneyLegEvent> LegEvents { get; init; } = [];

    public override Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
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

    public override Task PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        LegEvents.Enqueue(journeyLegCompletedEvent);
        return Task.CompletedTask;
    }

    public override Task PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    ) => PublishJourneyLegEventAsync(journeyLegStartedEvent, cancellationToken);

    public override Task PublishOnJourneyLegUpdatedAsync(
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
