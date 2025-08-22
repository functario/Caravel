using Caravel.Abstractions;
using Caravel.Core;

namespace Caravel.Tests.Fixtures;

public sealed class SmartJourney : Journey
{
    public SmartJourney(
        INode current,
        IGraph graph,
        ICoreFactories factories,
        InMemoryJourneyLegPublisher publisher,
        Map map,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, factories, publisher, publisher, journeyCancellationToken)
    {
        Map = map;
    }

    public Map Map { get; init; }
}
