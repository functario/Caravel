using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Graph.Dijkstra;

namespace InMemoryJourneyDemo.Nodes.UnweightedNodes;

internal sealed class UnweightedJourneySeed
{
    public UnweightedJourneySeed()
    {
        Node1 = new Node1();
        Node2 = new Node2();
        Node3 = new Node3();
        Nodes = [Node1, Node2, Node3];
        RouteFactory = new RouteFactory();
        EdgeFactory = new EdgeFactory();
        Graph = new DijkstraGraph(Nodes, RouteFactory, EdgeFactory);
        CoreFactories = new CoreFactories(TimeProvider.System);
    }

    public Node1 Node1 { get; init; }
    public Node2 Node2 { get; init; }
    public Node3 Node3 { get; init; }
    public INode[] Nodes { get; init; }
    public RouteFactory RouteFactory { get; init; }
    public EdgeFactory EdgeFactory { get; init; }
    public IGraph Graph { get; init; }
    public CoreFactories CoreFactories { get; init; }

    public static Journey CreateInMemoryJourney(
        InMemoryJourneyLegPublisher memoryJourneyLegPublisher
    )
    {
        var seed = new UnweightedJourneySeed();
        return new Journey(
            seed.Node1,
            seed.Graph,
            seed.CoreFactories,
            memoryJourneyLegPublisher,
            memoryJourneyLegPublisher,
            CancellationToken.None
        );
    }
}
