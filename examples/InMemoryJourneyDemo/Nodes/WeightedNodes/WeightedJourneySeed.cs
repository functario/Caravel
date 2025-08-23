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
        RouteFactory = new RouteFactory();
        EdgeFactory = new EdgeFactory();
        WeightedGraph = new DijkstraGraph(WeightedNodes, RouteFactory, EdgeFactory);
        CoreFactories = new JourneyConfiguration(TimeProvider.System);
    }

    public WeightedNode1 WeightedNode1 { get; init; }
    public WeightedNode2 WeightedNode2 { get; init; }
    public WeightedNode3 WeightedNode3 { get; init; }
    public INode[] WeightedNodes { get; init; }
    public RouteFactory RouteFactory { get; private set; }
    public EdgeFactory EdgeFactory { get; private set; }
    public IGraph WeightedGraph { get; init; }
    public JourneyConfiguration CoreFactories { get; init; }

    public static Journey CreateJourney()
    {
        var seed = new WeightedJourneySeed();
        return new Journey(
            seed.WeightedNode1,
            seed.WeightedGraph,
            seed.CoreFactories,
            CancellationToken.None
        );
    }
}
