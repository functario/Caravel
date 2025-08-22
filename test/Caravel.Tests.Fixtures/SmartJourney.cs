using Caravel.Abstractions;
using Caravel.Core;

namespace Caravel.Tests.Fixtures;

public sealed class SmartJourney : Journey
{
    public SmartJourney(
        INode current,
        IGraph graph,
        IJourneyCoreOptions journeyCoreOptions,
        Map map,
        CancellationToken journeyCancellationToken
    )
        : base(current, graph, journeyCoreOptions, journeyCancellationToken)
    {
        Map = map;
    }

    public Map Map { get; init; }
}
