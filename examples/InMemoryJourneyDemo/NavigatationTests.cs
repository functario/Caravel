using AwesomeAssertions;
using Caravel.Abstractions;
using Caravel.Core;
using Caravel.Core.Extensions;
using Caravel.Mermaid;
using InMemoryJourneyDemo.Nodes.UnweightedNodes;

namespace InMemoryJourneyDemo;

// csharpier-ignore-start
public class NavigatationTests
{
    private readonly InMemoryJourneyLegPublisher _inMemoryJourneyLegPublisher;
    private readonly IJourney _journey;

    public NavigatationTests()
    {
        _inMemoryJourneyLegPublisher = new InMemoryJourneyLegPublisher();
        _journey = UnweightedJourneySeed.CreateInMemoryJourney(_inMemoryJourneyLegPublisher);
    }

    [Fact(DisplayName = "Navigate using GotoAsync")]
    public async Task Test1()
    {
        // You need "using Caravel.Core.Extensions;"

        // Navigate from starting node Node1 to Node3 (Node1 => Node2 => Node3)
        // then continue from Node3 to Node1 (Node3 => Node1)
        // then continue from Node1 to Node2 (Node1 => Node2)
        await _journey
            .GotoAsync<Node3>()
            .GotoAsync<Node1>()
            .GotoAsync<Node2>();

        // Validate the navigation sequence with a Mermaid sequence diagram
        var mermaidNavigationSequence = await _journey.ToMermaidSequenceDiagramMarkdownAsync();
        var expectedNavigation =
            """
            sequenceDiagram
            Node1->>Node3:0
            Node3->>Node1:0
            Node1->>Node2:0
            """.ReplaceLineEndings();

        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }
}
// csharpier-ignore-end
