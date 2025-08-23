using Caravel.Abstractions;
using Caravel.Core;

namespace Caravel.Tests.Fixtures;

public sealed class SmartJourney : Journey
{
    public SmartJourney(
        INode current,
        IGraph graph,
        IJourneyConfiguration journeyConfiguration,
        Map map,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, journeyConfiguration, journeyCancellationToken)
    {
        Map = map;
    }

    public Map Map { get; init; }
}
