using System.Collections.Concurrent;
using Caravel.Abstractions;
using Caravel.Abstractions.Events;

namespace Caravel.Core;

public class InMemoryJourney : Journey
{
    private static CoreFactories CreateDefaultCoreFactories(TimeProvider timeProvider) =>
        new(timeProvider);

    /// <summary>
    /// Build an <see cref="InMemoryJourney"/> with default <see cref="CoreFactories"/>.
    /// </summary>
    /// <param name="startingNode">The starting <see cref="INode"/>.</param>
    /// <param name="graph">The <see cref="IGraph"/>.</param>
    /// <param name="timeProvider">A <see cref="TimeProvider"/> used to create events timestamp.</param>
    /// <param name="journeyCancellationToken">The <see cref="IJourney"/> <see cref="CancellationToken"/>.</param>
    public InMemoryJourney(
        INode startingNode,
        IGraph graph,
        TimeProvider timeProvider,
        CancellationToken journeyCancellationToken
    )
        : base(
            startingNode,
            graph,
            CreateDefaultCoreFactories(timeProvider),
            journeyCancellationToken
        ) { }

    /// <summary>
    /// Build an <see cref="InMemoryJourney"/> with default <see cref="CoreFactories"/>.
    /// </summary>
    /// <param name="startingNode">The starting <see cref="INode"/>.</param>
    /// <param name="graph">The <see cref="IGraph"/>.</param>
    /// <param name="factories">The <see cref="ICoreFactories"/>.</param>
    /// <param name="journeyCancellationToken">The <see cref="IJourney"/> <see cref="CancellationToken"/>.</param>
    public InMemoryJourney(
        INode startingNode,
        IGraph graph,
        ICoreFactories factories,
        CancellationToken journeyCancellationToken
    )
        : base(startingNode, graph, factories, journeyCancellationToken) { }

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
