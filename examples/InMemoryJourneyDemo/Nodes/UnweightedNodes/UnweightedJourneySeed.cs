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
        JourneyLegFactory = new JourneyLegFactory();
        ActionMetaDataFactory = new ActionMetaDataFactory();
        Graph = new DijkstraGraph(Nodes, RouteFactory, EdgeFactory);
    }

    public Node1 Node1 { get; init; }
    public Node2 Node2 { get; init; }
    public Node3 Node3 { get; init; }
    public INode[] Nodes { get; init; }
    public RouteFactory RouteFactory { get; init; }
    public EdgeFactory EdgeFactory { get; init; }
    public JourneyLegFactory JourneyLegFactory { get; init; }
    public ActionMetaDataFactory ActionMetaDataFactory { get; init; }
    public IGraph Graph { get; init; }

    public static InMemoryJourney CreateInMemoryJourney()
    {
        var seed = new UnweightedJourneySeed();
        return new InMemoryJourney(
            seed.Node1,
            seed.Graph,
            TimeProvider.System,
            seed.JourneyLegFactory,
            seed.ActionMetaDataFactory,
            CancellationToken.None
        );
    }
}
