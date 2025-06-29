using System.Collections.Concurrent;
using Caravel.Abstractions;
using Caravel.Core;

namespace Caravel.Tests.Fixtures;

public sealed record TestingJourney : Journey
{
    public TestingJourney(INode current, IGraph graph, CancellationToken journeyCancellationToken)
        : base(current, graph, journeyCancellationToken) { }

    public ConcurrentQueue<IJourneyLeg> Legs { get; init; } = [];

    protected override Task PublishOnJourneyLegCompletedAsync(
        IJourneyLeg journeyLeg,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        Legs.Enqueue(journeyLeg);
        return Task.CompletedTask;
    }

    protected override Task<IEnumerable<IJourneyLeg>> ReadJourneyLegsAsync(
        CancellationToken cancellationToken = default
    )
    {
        var legArray = Legs.ToArray();
        return Task.FromResult<IEnumerable<IJourneyLeg>>(legArray);
    }
}
