using System.Collections.Concurrent;
using Caravel.Abstractions;
using Caravel.Abstractions.Events;
using Caravel.Core;

namespace Caravel.Tests.Fixtures;

public sealed record TestingJourney : Journey
{
    public TestingJourney(
        INode current,
        IGraph graph,
        TimeProvider timeProvider,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, timeProvider, journeyCancellationToken) { }

    public ConcurrentQueue<IJourneyLegEvent> LegEvents { get; init; } = [];

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

    protected override Task<IEnumerable<IJourneyLeg>> GetCompletedJourneyLegsAsync(
        CancellationToken cancellationToken = default
    )
    {
        var legArray = LegEvents
            .Where(x => x is IJourneyLegCompletedEvent)
            .OrderBy(x => x.EventDateTimeOffset)
            .Select(x => x.JourneyLeg)
            .ToArray();

        return Task.FromResult<IEnumerable<IJourneyLeg>>(legArray);
    }
}
