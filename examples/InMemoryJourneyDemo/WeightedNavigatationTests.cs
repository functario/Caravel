using AwesomeAssertions;
using Caravel.Abstractions;
using Caravel.Core.Extensions;
using Caravel.Mermaid;
using InMemoryJourneyDemo.Nodes.WeightedNodes;

namespace InMemoryJourneyDemo;

// csharpier-ignore-start
public class WeightedNavigatationTests
{
    private readonly IJourney _journey;

    public WeightedNavigatationTests()
    {
        _journey = WeightedJourneySeed.CreateJourney();
    }

    [Fact(DisplayName = $"Navigate a weighted graph using {nameof(IJourney.GotoAsync)}")]
    public async Task Test1()
    {
        // You need "using Caravel.Core.Extensions;"

        /* WeightedNode1's edges:
         WeightedNode1->>WeightedNode2:10
         WeightedNode1->>WeightedNode3:99
         */

        /* WeightedNode2's edges:
         WeightedNode2->>WeightedNode3:1
         */

        /* WeightedNode3's edges:
         WeightedNode3->>WeightedNode1:10
         */

        // Because WeightedNode1->>WeightedNode3 has a weight of 99
        // the shortest is WeightedNode1->>WeightedNode2 (weight = 10),
        // then WeightedNode2->>WeightedNode3 (weight = 1)
        await _journey
            .GotoAsync<WeightedNode3>();

        // Validate the navigation sequence with a Mermaid sequence diagram
        var mermaidNavigationSequence = await _journey.ToMermaidSequenceDiagramMarkdownAsync();
        var expectedNavigation =
            """
            sequenceDiagram
            WeightedNode1->>WeightedNode2:10
            WeightedNode2->>WeightedNode3:1
            """.ReplaceLineEndings();

        mermaidNavigationSequence.ReplaceLineEndings().Should().Be(expectedNavigation);
    }
}
// csharpier-ignore-end
