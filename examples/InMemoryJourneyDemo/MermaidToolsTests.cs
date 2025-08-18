using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Core.Extensions;
using Caravel.Mermaid;
using InMemoryJourneyDemo.Nodes;
using InMemoryJourneyDemo.Nodes.UnweightedNodes;
using InMemoryJourneyDemo.Nodes.WeightedNodes;

namespace InMemoryJourneyDemo;

// csharpier-ignore-start
public class MermaidToolsTests
{
    private readonly Node3 _node3;
    private readonly IGraph _graph;
    private readonly InMemoryJourney _unweightedJourney;
    private readonly InMemoryJourney _weightedJourney;

    public MermaidToolsTests()
    {
        var unweightedSeed = new UnweightedJourneySeed();
        _node3 = unweightedSeed.Node3;
        _graph = unweightedSeed.Graph;

        _unweightedJourney = unweightedSeed.CreateInMemoryJourney();

        var weightedSeed = new WeightedJourneySeed();
        _node3 = unweightedSeed.Node3;
        _graph = unweightedSeed.Graph;

        _weightedJourney = weightedSeed.CreateInMemoryJourney();

    }

    [Fact(DisplayName = "Generate a Mermaid graph diagram of the Graph")]
    public void Test1()
    {
        // Generate from the InMemoryJourney...
        var mermaidFromJourney = _unweightedJourney.ToMermaidMarkdown().ReplaceLineEndings();


        // or from from the Graph itself.
        var mermaidFromGraph = _graph.ToMermaidMarkdown().ReplaceLineEndings();

        var expectedGraph =
            """
            graph TD
            Node1 -->|0| Node2
            Node1 -->|0| Node3
            Node2 -->|0| Node3
            Node3 -->|0| Node1
            """.ReplaceLineEndings();

        mermaidFromJourney.Should().Be(mermaidFromGraph);
        mermaidFromJourney.Should().Be(expectedGraph);
    }

    [Fact(DisplayName = "Generate a Mermaid sequence diagram of the navigation from unweighted graph")]
    public async Task Test2()
    {
        await _unweightedJourney
            .GotoAsync<Node3>()
            .GotoAsync<Node1>()
            .GotoAsync<Node2>();

        // Validate the navigation sequence with a Mermaid sequence diagram
        var mermaidNavigationSequence = await _unweightedJourney.ToMermaidSequenceDiagramMarkdownAsync();
        var expectedNavigation =
            """
            sequenceDiagram
            Node1->>Node3:0
            Node3->>Node1:0
            Node1->>Node2:0
            """.ReplaceLineEndings();

        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }

    [Fact(DisplayName = "Generate a Mermaid sequence diagram of the navigation from weighted graph")]
    public async Task Test3()
    {
        await _weightedJourney
            .GotoAsync<WeightedNode3>()
            .GotoAsync<WeightedNode1>()
            .GotoAsync<WeightedNode2>();

        // Validate the navigation sequence with a Mermaid sequence diagram
        // with weight displayed for each edge.
        var mermaidNavigationSequence = await _weightedJourney.ToMermaidSequenceDiagramMarkdownAsync();
        var expectedNavigation =
            """
            sequenceDiagram
            WeightedNode1->>WeightedNode2:10
            WeightedNode2->>WeightedNode3:1
            WeightedNode3->>WeightedNode1:10
            WeightedNode1->>WeightedNode2:10
            """.ReplaceLineEndings();

        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }

    [Fact(DisplayName = "Generate multiple Mermaid sequence diagrams of the navigation")]
    public async Task Test4()
    {
        await _unweightedJourney
            .GotoAsync<Node3>()
            .DoAsync<Node3>((n, ct) => Task.FromResult(_node3))
            .GotoAsync<Node1>()
            .DoAsync<Node1, Node3>((n, ct) => Task.FromResult(_node3))
            .GotoAsync<Node2>();

        // Sequences are splitted into individual Mermaid sequence.
        var splitedNavigationSequences = await _unweightedJourney.ToManyMermaidSequenceDiagramMarkdown();
        Dictionary<int, string> expectedSequences = [];

        // Node1 (starting Node) => GotoAsync<Node3>()
        expectedSequences.Add(
            0,
            """
                sequenceDiagram
                box
                participant Node1
                participant Node3
                end
                Node1->>Node3:0
                """.ReplaceLineEndings());

        // On Node3, DoAsync<Node3>(...)
        expectedSequences.Add(
            1,
            """
                sequenceDiagram
                box
                participant Node3
                participant Node3
                end
                Node3->>Node3:0
                """.ReplaceLineEndings());

        // Node3 => GotoAsync<Node1>()
        expectedSequences.Add(
            2,
            """
                sequenceDiagram
                box
                participant Node3
                participant Node1
                end
                Node3->>Node1:0
                """.ReplaceLineEndings());


        // Node1 => DoAsync<Node1, Node3>(...)
        expectedSequences.Add(
            3,
            """
                sequenceDiagram
                box
                participant Node1
                participant Node3
                end
                Node1->>Node3:0
                """.ReplaceLineEndings());

        // Node3 => GotoAsync<Node2>()
        expectedSequences.Add(
            4,
            """
                sequenceDiagram
                box
                participant Node3
                participant Node1
                participant Node2
                end
                Node3->>Node1:0
                Node1->>Node2:0
                """.ReplaceLineEndings());

        // Validate the sequence
        using var scope = new AssertionScope();

        splitedNavigationSequences.Should().BeEquivalentTo(expectedSequences);

        splitedNavigationSequences.Should().AllSatisfy(
            (keyValuePair) =>
                keyValuePair.Value.Should().BeEquivalentTo(expectedSequences[keyValuePair.Key])
            );
    }

    [Fact(DisplayName = $"Generate a Mermaid graph diagram with {nameof(ActionMetaData)}")]
    public void Test5()
    {
        // Validate the navigation sequence with a Mermaid sequence diagram
        // with weight and metadata description displayed for each edge.
        var mermaidOptions = new MermaidOptions()
        { DisplayDescription = true, DisplayGridPosition = false, GraphDirection = MermaidGraphDirections.TD };

        var mermaidNavigationSequence = _weightedJourney.ToMermaidMarkdown(mermaidOptions);
        var expectedNavigation =
            """
            graph TD
            WeightedNode1 -->|10<br>Open the Node2 from Node1| WeightedNode2
            WeightedNode1 -->|99<br>Open the Node3 from Node1| WeightedNode3
            WeightedNode2 -->|1<br>Open the Node3 from Node2| WeightedNode3
            WeightedNode3 -->|10<br>Open the Node1 from Node3| WeightedNode1
            """.ReplaceLineEndings();

        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }

    [Fact(DisplayName = $"Generate a Mermaid graph diagram with {nameof(INode)}'s {nameof(GridPositionAttribute)}")]
    public void Test6()
    {
        /*
         The INode attribute GridPositionAttribute allows you to reference nodes
         relative to other nodes on a virtual grid.
         This improves the display of Mermaid graphs.

         Example from WeightedNode1.cs decalaration:

         [GridPosition(row: 0, column: 0)]
         internal sealed class WeightedNode1 : INode

         */

        // Validate the navigation sequence with a Mermaid sequence diagram
        // with the Node.GridPosition.
        var mermaidOptions = new MermaidOptions()
        { DisplayDescription = false, DisplayGridPosition = true };

        var mermaidNavigationSequence = _weightedJourney.ToMermaidMarkdown(mermaidOptions);
        var expectedNavigation =
            """
            graph TD
            WeightedNode1 -->|10<br>0,0| WeightedNode2
            WeightedNode1 -->|99<br>0,0| WeightedNode3
            WeightedNode2 -->|1<br>1,0| WeightedNode3
            WeightedNode3 -->|10<br>2,1| WeightedNode1
            """.ReplaceLineEndings();

        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }

    [Fact(DisplayName = "Do action inside a node")]
    public async Task Test7()
    {
        // You need "using Caravel.Core.Extensions;"

        await _weightedJourney
            .DoAsync<WeightedNode1>((node1, cancellationToken) =>
            {
                return Task.FromResult(node1);
            })
            .GotoAsync<WeightedNode2>();

        // Validate the navigation sequence with a Mermaid sequence diagram
        // with default EdgeMetadata description "Journey.DoAsync"
        // and custom EdgeMetadata from WeightedNode1 declaration.
        var mermaidOptions = new MermaidOptions()
        { DisplayDescription = true, DisplayGridPosition = false, GraphDirection = MermaidGraphDirections.TD };

        var mermaidNavigationSequence = await _weightedJourney.ToMermaidSequenceDiagramMarkdownAsync(mermaidOptions);
        var expectedNavigation =
            """
            sequenceDiagram
            WeightedNode1->>WeightedNode1:0<br>Journey.DoAsync
            WeightedNode1->>WeightedNode2:10<br>Open the Node2 from Node1
            """.ReplaceLineEndings();



        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }

    private delegate TResult MyFunc<in T, out TResult>(T arg);
}
// csharpier-ignore-end
