using System.Collections.Concurrent;
using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core;

public class InMemoryJourney : Journey
{
    public InMemoryJourney(
        INode startingNode,
        IGraph graph,
        TimeProvider timeProvider,
        CancellationToken journeyCancellationToken
    )
        : base(startingNode, graph, timeProvider, journeyCancellationToken) { }

    public ConcurrentQueue<IJourneyLegEvent> LegEvents { get; init; } = [];

    public sealed override Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
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

    protected override Task PublishOnJourneyLegCompletedAsync(
        IJourneyLegCompletedEvent journeyLegCompletedEvent,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        LegEvents.Enqueue(journeyLegCompletedEvent);
        return Task.CompletedTask;
    }

    protected override Task PublishOnJourneyLegStartedAsync(
        IJourneyLegStartedEvent journeyLegStartedEvent,
        CancellationToken cancellationToken
    ) => PublishJourneyLegEventAsync(journeyLegStartedEvent, cancellationToken);

    protected override Task PublishOnJourneyLegUpdatedAsync(
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
