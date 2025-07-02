using Caravel.Abstractions;
using Caravel.Core;

namespace Caravel.Tests.Fixtures;

public sealed class SmartJourney : InMemoryJourney
{
    public SmartJourney(
        INode current,
        IGraph graph,
        TimeProvider timeProvider,
        Map map,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, timeProvider, journeyCancellationToken)
    {
        Map = map;
    }

    public Map Map { get; init; }
}
