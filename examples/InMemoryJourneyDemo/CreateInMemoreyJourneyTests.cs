using AwesomeAssertions;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Graph.Dijkstra;
using Caravel.Mermaid;
using InMemoryJourneyDemo.Nodes.UnweightedNodes;

namespace InMemoryJourneyDemo;

// csharpier-ignore-start
public class CreateInMemoreyJourneyTests
{
    [Fact(DisplayName = "Create a InMemoryJourney")]
    public void Test1()
    {
        // Create the nodes.
        var node1 = new Node1();
        var node2 = new Node2();
        var node3 = new Node3();
        INode[] nodes = [node1, node2, node3];

        // basic factories
        var routeFactory = new RouteFactory();
        var edgeFactory = new EdgeFactory();
        var journeyLegFactory = new JourneyLegFactory();
        var actionMetaDataFactory = new ActionMetaDataFactory();

        // Generate the graph.
        var graph = new DijkstraGraph(nodes, routeFactory, edgeFactory);

        // Create the InMemoryJourney
        var inMemoryJourney = new InMemoryJourney(
            node1,
            graph,
            TimeProvider.System,
            journeyLegFactory,
            actionMetaDataFactory,
            CancellationToken.None
        );


        // Validate the generated graph with a Mermaid graph.
        var mermaidGraph = inMemoryJourney.ToMermaidMarkdown();
        var expectedNavigation =
            """
            graph TD
            Node1 -->|0| Node2
            Node1 -->|0| Node3
            Node2 -->|0| Node3
            Node3 -->|0| Node1
            """.ReplaceLineEndings();

        mermaidGraph.ReplaceLineEndings().Should().Be(expectedNavigation);
    }
}
// csharpier-ignore-end
