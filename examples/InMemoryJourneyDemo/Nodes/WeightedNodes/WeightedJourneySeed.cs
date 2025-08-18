using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Graph.Dijkstra;

namespace InMemoryJourneyDemo.Nodes.WeightedNodes;

internal sealed class WeightedJourneySeed
{
    public WeightedJourneySeed()
    {
        WeightedNode1 = new WeightedNode1();
        WeightedNode2 = new WeightedNode2();
        WeightedNode3 = new WeightedNode3();
        WeightedNodes = [WeightedNode1, WeightedNode2, WeightedNode3];
        WeightedGraph = new DijkstraGraph(WeightedNodes);
    }

    public WeightedNode1 WeightedNode1 { get; init; }
    public WeightedNode2 WeightedNode2 { get; init; }
    public WeightedNode3 WeightedNode3 { get; init; }
    public INode[] WeightedNodes { get; init; }
    public IGraph WeightedGraph { get; init; }

    public static InMemoryJourney CreateInMemoryJourney()
    {
        var seed = new WeightedJourneySeed();
        return new InMemoryJourney(
            seed.WeightedNode1,
            seed.WeightedGraph,
            TimeProvider.System,
            CancellationToken.None
        );
    }
}
