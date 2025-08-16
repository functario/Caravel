using AwesomeAssertions;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Core.Extensions;
using Caravel.Graph.Dijkstra;
using Caravel.Mermaid;
using InMemoryJourneyDemo.Nodes.UnweightedNodes;

namespace InMemoryJourneyDemo;

// csharpier-ignore-start
public class ActionsInNodesTests
{

    private readonly Node1 _node1;
    private readonly Node2 _node2;
    private readonly Node3 _node3;
    private readonly INode[] _nodes;
    private readonly IGraph _graph;
    private readonly InMemoryJourney _inMemoryJourney;

    public ActionsInNodesTests()
    {
        _node1 = new Node1();
        _node2 = new Node2();
        _node3 = new Node3();
        _nodes = [_node1, _node2, _node3];
        _graph = new DijkstraGraph(_nodes);

        _inMemoryJourney = new InMemoryJourney(
            _node1,
            _graph,
            TimeProvider.System,
            CancellationToken.None
        );

    }

    [Fact(DisplayName = "Do action inside a node")]
    public async Task Test1()
    {
        // You need "using Caravel.Core.Extensions;"

        await _inMemoryJourney
            .DoAsync<Node1>((node1, cancellationToken) =>
            {
                // Do something on the current node (Node1)
                return Task.FromResult(node1);
            })
            .DoAsync<Node1>((journey, node1, cancellationToken) =>
            {
                // The journey can be access too.
                journey.CurrentNode.Should().Be(node1);
                return Task.FromResult(node1);
            });

        //Validate the navigation sequence with a Mermaid sequence diagram
        var mermaidNavigationSequence = await _inMemoryJourney.ToMermaidSequenceDiagramMarkdownAsync();
        var expectedNavigation =
            """
            sequenceDiagram
            Node1->>Node1:0
            Node1->>Node1:0
            """.ReplaceLineEndings();

        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }

    [Fact(DisplayName = "Do action from a node and return another node.")]
    public async Task Test2()
    {
        // You need "using Caravel.Core.Extensions;"

        await _inMemoryJourney
            .DoAsync<Node1, Node3>((node1, cancellationToken) =>
            {
                // Do something that will change the current node to Node3 and return it.
                return Task.FromResult(_node3);
            })
            .DoAsync<Node3, Node2>((journey, node1, cancellationToken) =>
            {
                // The journey can be access too.
                journey.CurrentNode.Should().Be(_node3);
                return Task.FromResult(_node2);
            });

        //Validate the navigation sequence with a Mermaid sequence diagram
        var mermaidNavigationSequence = await _inMemoryJourney.ToMermaidSequenceDiagramMarkdownAsync();
        var expectedNavigation =
            """
            sequenceDiagram
            Node1->>Node3:0
            Node3->>Node2:0
            """.ReplaceLineEndings();

        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }
}
// csharpier-ignore-end
